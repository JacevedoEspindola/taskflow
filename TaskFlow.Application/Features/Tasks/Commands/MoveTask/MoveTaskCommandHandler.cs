using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Commands.MoveTask;

public class MoveTaskCommandHandler : IRequestHandler<MoveTaskCommand, TaskDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MoveTaskCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TaskDetailDto> Handle(MoveTaskCommand request, CancellationToken cancellationToken)
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

        // 2. Cargar la columna destino
        var targetColumn = await _db.BoardColumns
            .FirstOrDefaultAsync(c => c.Id == request.TargetColumnId, cancellationToken);

        if (targetColumn is null)
            throw new KeyNotFoundException($"Columna destino '{request.TargetColumnId}' no encontrada.");

        // 3. Verificar acceso al proyecto
        var hasAccess = await _db.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == targetColumn.ProjectId && pm.UserId == userId, cancellationToken);

        if (!hasAccess)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        // 4. Validar que source y target están en el MISMO proyecto
        if (task.Column.ProjectId != targetColumn.ProjectId)
            throw new InvalidOperationException("No se puede mover una tarea a un proyecto diferente.");

        var oldColumnId = task.ColumnId;
        var oldPosition = task.Position;

        // 5. Reordenar según el caso
        if (oldColumnId == targetColumn.Id)
        {
            // Mover dentro de la misma columna
            var newPosition = request.Position ?? await GetMaxPositionAsync(targetColumn.Id, cancellationToken);

            if (newPosition < oldPosition)
            {
                var toShift = await _db.TaskItems
                    .Where(t => t.ColumnId == targetColumn.Id
                             && t.Id != task.Id
                             && t.Position >= newPosition
                             && t.Position < oldPosition)
                    .ToListAsync(cancellationToken);

                foreach (var t in toShift) t.Position++;
            }
            else if (newPosition > oldPosition)
            {
                var toShift = await _db.TaskItems
                    .Where(t => t.ColumnId == targetColumn.Id
                             && t.Id != task.Id
                             && t.Position > oldPosition
                             && t.Position <= newPosition)
                    .ToListAsync(cancellationToken);

                foreach (var t in toShift) t.Position--;
            }

            task.Position = newPosition;
        }
        else
        {
            // Mover a otra columna
            // a) "Cerrar el hueco" en la columna origen
            var sourceTasksToShift = await _db.TaskItems
                .Where(t => t.ColumnId == oldColumnId && t.Position > oldPosition)
                .ToListAsync(cancellationToken);

            foreach (var t in sourceTasksToShift) t.Position--;

            // b) "Abrir hueco" en la columna destino
            var newPosition = request.Position
                ?? (await GetMaxPositionAsync(targetColumn.Id, cancellationToken) + 1);

            var targetTasksToShift = await _db.TaskItems
                .Where(t => t.ColumnId == targetColumn.Id && t.Position >= newPosition)
                .ToListAsync(cancellationToken);

            foreach (var t in targetTasksToShift) t.Position++;

            task.ColumnId = targetColumn.Id;
            task.Position = newPosition;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new TaskDetailDto(
            task.Id,
            task.Title,
            task.Description,
            task.Priority,
            task.DueDate,
            task.Position,
            task.ColumnId,
            targetColumn.Name,
            targetColumn.ProjectId,
            task.AssigneeId,
            task.Assignee?.FullName,
            task.Assignee?.Email,
            task.CreatedAt
        );
    }

    private async Task<int> GetMaxPositionAsync(Guid columnId, CancellationToken ct)
    {
        return await _db.TaskItems
            .Where(t => t.ColumnId == columnId)
            .Select(t => (int?)t.Position)
            .MaxAsync(ct) ?? 0;
    }
}