using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Columns.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Columns.Commands.CreateColumn;

public class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, ColumnDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateColumnCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ColumnDetailDto> Handle(CreateColumnCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        // 1. Verificar que el proyecto existe y el usuario es Owner/Admin
        var membership = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == request.ProjectId && pm.UserId == userId, cancellationToken);

        if (membership is null)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        if (membership.Role == ProjectRole.Member)
            throw new UnauthorizedAccessException("Solo Owners y Admins pueden crear columnas.");

        // 2. Calcular la siguiente posición
        var maxPosition = await _db.BoardColumns
            .Where(c => c.ProjectId == request.ProjectId)
            .Select(c => (int?)c.Position)
            .MaxAsync(cancellationToken) ?? 0;

        // 3. Crear la columna
        var column = new BoardColumn
        {
            Name = request.Name.Trim(),
            Position = maxPosition + 1,
            ProjectId = request.ProjectId,
            CreatedAt = DateTime.UtcNow
        };

        _db.BoardColumns.Add(column);
        await _db.SaveChangesAsync(cancellationToken);

        return new ColumnDetailDto(
            column.Id,
            column.Name,
            column.Position,
            column.ProjectId,
            0,
            column.CreatedAt
        );
    }
}