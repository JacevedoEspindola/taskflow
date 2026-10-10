using System;
using System.Collections.Generic;
using System.Text;
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

    public CreateTaskCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TaskDetailDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        // 1. Cargar la columna + su proyecto
        var column = await _db.BoardColumns
            .Include(c => c.Project)
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId, cancellationToken);

        if (column is null)
            throw new KeyNotFoundException($"Columna '{request.ColumnId}' no encontrada.");

        // 2. Verificar acceso al proyecto
        var hasAccess = await _db.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == column.ProjectId && pm.UserId == userId, cancellationToken);

        if (!hasAccess)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        // 3. Calcular la siguiente posición
        var maxPosition = await _db.TaskItems
            .Where(t => t.ColumnId == request.ColumnId)
            .Select(t => (int?)t.Position)
            .MaxAsync(cancellationToken) ?? 0;

        // 4. Crear la tarea
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

        return new TaskDetailDto(
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
    }
}