using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Columns.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Columns.Commands.UpdateColumn;

public class UpdateColumnCommandHandler : IRequestHandler<UpdateColumnCommand, ColumnDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateColumnCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ColumnDetailDto> Handle(UpdateColumnCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        // 1. Verificar acceso y rol
        var membership = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == request.ProjectId && pm.UserId == userId, cancellationToken);

        if (membership is null)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        if (membership.Role == ProjectRole.Member)
            throw new UnauthorizedAccessException("Solo Owners y Admins pueden editar columnas.");

        // 2. Buscar la columna
        var column = await _db.BoardColumns
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId && c.ProjectId == request.ProjectId, cancellationToken);

        if (column is null)
            throw new KeyNotFoundException($"Columna '{request.ColumnId}' no encontrada.");

        // 3. Actualizar nombre
        column.Name = request.Name.Trim();

        // 4. Si se pide reordenar, actualizar Position y reajustar las demás
        if (request.Position.HasValue && request.Position.Value != column.Position)
        {
            var newPosition = request.Position.Value;
            var oldPosition = column.Position;

            var columns = await _db.BoardColumns
                .Where(c => c.ProjectId == request.ProjectId && c.Id != column.Id)
                .ToListAsync(cancellationToken);

            if (newPosition < oldPosition)
            {
                // Mover hacia arriba: incrementar las que están entre newPosition y oldPosition-1
                foreach (var c in columns.Where(c => c.Position >= newPosition && c.Position < oldPosition))
                    c.Position++;
            }
            else
            {
                // Mover hacia abajo: decrementar las que están entre oldPosition+1 y newPosition
                foreach (var c in columns.Where(c => c.Position > oldPosition && c.Position <= newPosition))
                    c.Position--;
            }

            column.Position = newPosition;
        }

        await _db.SaveChangesAsync(cancellationToken);

        var taskCount = await _db.TaskItems.CountAsync(t => t.ColumnId == column.Id, cancellationToken);

        return new ColumnDetailDto(
            column.Id,
            column.Name,
            column.Position,
            column.ProjectId,
            taskCount,
            column.CreatedAt
        );
    }
}