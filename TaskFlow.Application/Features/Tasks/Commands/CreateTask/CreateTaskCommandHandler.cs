using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IRealTimeNotifier _notifier;

    public CreateTaskCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IRealTimeNotifier notifier)
    {
        _db = db;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task<TaskDetailDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("You must be logged in.");


        var column = await _db.BoardColumns
            .Include(c => c.Project)
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId, cancellationToken);

        if (column is null)
            throw new KeyNotFoundException($"Column '{request.ColumnId}' not found.");

        var hasAccess = await _db.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == column.ProjectId && pm.UserId == userId, cancellationToken);

        if (!hasAccess)
            throw new UnauthorizedAccessException("You do not have access to this project.");

        var maxPosition = await _db.TaskItems
            .Where(t => t.ColumnId == request.ColumnId)
            .Select(t => (int?)t.Position)
            .MaxAsync(cancellationToken) ?? 0;

        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Priority = request.Priority,
            DueDate = request.DueDate,
            Position = maxPosition + 1,
            ColumnId = request.ColumnId,
            CreatedAt = DateTime.UtcNow
        };

        _db.TaskItems.Add(task);
        await _db.SaveChangesAsync(cancellationToken);

        var dto = new TaskDetailDto(
            task.Id,
            task.Title,
            task.Description,
            task.Priority,
            task.DueDate,
            task.Position,
            task.ColumnId,
            column.Name,
            column.ProjectId,
            task.AssigneeId,
            null,
            null,
            task.CreatedAt
        );

        // 🔔 Notificar en tiempo real
        await _notifier.NotifyTaskCreatedAsync(column.ProjectId, dto);

        return dto;
    }
}