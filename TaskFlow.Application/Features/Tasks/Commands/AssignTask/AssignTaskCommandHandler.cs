using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Commands.AssignTask;

public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, TaskDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AssignTaskCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TaskDetailDto> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        // 1. Cargar la tarea
        var task = await _db.TaskItems
            .Include(t => t.Column)
            .Include(t => t.Assignee)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task is null)
            throw new KeyNotFoundException($"Tarea '{request.TaskId}' no encontrada.");

        // 2. Verificar acceso al proyecto
        var hasAccess = await _db.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == task.Column.ProjectId && pm.UserId == userId, cancellationToken);

        if (!hasAccess)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        // 3. Si viene AssigneeId, verificar que sea miembro del proyecto
        if (request.AssigneeId.HasValue)
        {
            var isMember = await _db.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == task.Column.ProjectId
                             && pm.UserId == request.AssigneeId.Value,
                         cancellationToken);

            if (!isMember)
                throw new InvalidOperationException("El usuario asignado no es miembro del proyecto.");
        }

        task.AssigneeId = request.AssigneeId;
        await _db.SaveChangesAsync(cancellationToken);

        // Recargar el assignee
        var assignee = task.AssigneeId.HasValue
            ? await _db.Users.FirstOrDefaultAsync(u => u.Id == task.AssigneeId.Value, cancellationToken)
            : null;

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
            assignee?.FullName,
            assignee?.Email,
            task.CreatedAt
        );
    }
}