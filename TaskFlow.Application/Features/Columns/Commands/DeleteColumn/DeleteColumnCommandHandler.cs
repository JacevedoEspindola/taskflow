using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Columns.Commands.DeleteColumn;

public class DeleteColumnCommandHandler : IRequestHandler<DeleteColumnCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteColumnCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteColumnCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        // 1. Verificar acceso y rol
        var membership = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == request.ProjectId && pm.UserId == userId, cancellationToken);

        if (membership is null)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        if (membership.Role == ProjectRole.Member)
            throw new UnauthorizedAccessException("Solo Owners y Admins pueden eliminar columnas.");

        // 2. Buscar la columna
        var column = await _db.BoardColumns
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId && c.ProjectId == request.ProjectId, cancellationToken);

        if (column is null)
            throw new KeyNotFoundException($"Columna '{request.ColumnId}' no encontrada.");

        // 3. Borrar (las tareas se borran en cascade)
        _db.BoardColumns.Remove(column);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}