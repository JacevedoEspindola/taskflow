using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTasksByColumn;

public class GetTasksByColumnQueryHandler : IRequestHandler<GetTasksByColumnQuery, IEnumerable<TaskDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetTasksByColumnQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<TaskDto>> Handle(GetTasksByColumnQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        // Cargar la columna para saber a qué proyecto pertenece
        var column = await _db.BoardColumns
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId, cancellationToken);

        if (column is null)
            throw new KeyNotFoundException($"Columna '{request.ColumnId}' no encontrada.");

        // Verificar acceso al proyecto
        var hasAccess = await _db.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == column.ProjectId && pm.UserId == userId, cancellationToken);

        if (!hasAccess)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        var tasks = await _db.TaskItems
            .Where(t => t.ColumnId == request.ColumnId)
            .OrderBy(t => t.Position)
            .Select(t => new TaskDto(
                t.Id,
                t.Title,
                t.Description,
                t.Priority,
                t.DueDate,
                t.Position,
                t.ColumnId,
                t.AssigneeId,
                t.Assignee != null ? t.Assignee.FullName : null,
                t.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return tasks;
    }
}