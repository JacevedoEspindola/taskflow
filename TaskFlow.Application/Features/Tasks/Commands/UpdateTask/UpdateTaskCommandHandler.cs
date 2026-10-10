using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateTaskCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TaskDetailDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        var task = await _db.TaskItems
            .Include(t => t.Column)
            .Include(t => t.Assignee)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task is null)
            throw new KeyNotFoundException($"Tarea '{request.TaskId}' no encontrada.");

        // Verificar acceso al proyecto
        var hasAccess = await _db.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == task.Column.ProjectId && pm.UserId == userId, cancellationToken);

        if (!hasAccess)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        // Actualizar campos
        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;

        await _db.SaveChangesAsync(cancellationToken);

        return new TaskDetailDto(
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
            task.Assignee?.FullName,
            task.Assignee?.Email,
            task.CreatedAt
        );
    }
}