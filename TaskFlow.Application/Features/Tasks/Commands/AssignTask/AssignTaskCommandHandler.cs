using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.AssignTask;

public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, TaskDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    private readonly IRealTimeNotifier _notifier;

    public AssignTaskCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IRealTimeNotifier notifier)
    {
        _db = db;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task<TaskDetailDto> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("You must be logged in.");

        // 1. Cargar la tarea
        var task = await _db.TaskItems
            .Include(t => t.Column)
            .Include(t => t.Assignee)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task is null)
            throw new KeyNotFoundException($"Task '{request.TaskId}' not found");

        // 2. Verificar acceso al proyecto
        var hasAccess = await _db.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == task.Column.ProjectId && pm.UserId == userId, cancellationToken);

        if (!hasAccess)
            throw new UnauthorizedAccessException("You do not have access to this project.");

        // 3. Si viene AssigneeId, verificar que sea miembro del proyecto
        if (request.AssigneeId.HasValue)
        {
            var isMember = await _db.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == task.Column.ProjectId
                             && pm.UserId == request.AssigneeId.Value,
                         cancellationToken);

            if (!isMember)
                throw new InvalidOperationException("The assigned user is not a member of this project.");
        }

        task.AssigneeId = request.AssigneeId;
        await _db.SaveChangesAsync(cancellationToken);

        // Recargar el assignee
        var assignee = task.AssigneeId.HasValue
            ? await _db.Users.FirstOrDefaultAsync(u => u.Id == task.AssigneeId.Value, cancellationToken)
            : null;

        var dto = new TaskDetailDto(
                task.Id,
                task.Title,
                task.Description,
                task.Priority,
                task.DueDate,
                task.Position,
                task.ColumnId,
                task.Column.Name,
                task.Column.ProjectId,
                task.AssigneeId,
                assignee?.FullName,
                assignee?.Email,
                task.CreatedAt
            );

        // 🔔 Notificar la asignación
        await _notifier.NotifyTaskAssignedAsync(task.Column.ProjectId, dto);

        return dto;
    }
}