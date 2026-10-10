using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Columns.Dtos;

namespace TaskFlow.Application.Features.Columns.Queries.GetColumns;

public class GetColumnsQueryHandler : IRequestHandler<GetColumnsQuery, IEnumerable<ColumnDetailDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetColumnsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<ColumnDetailDto>> Handle(GetColumnsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        // Verificar acceso al proyecto
        var hasAccess = await _db.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == request.ProjectId && pm.UserId == userId, cancellationToken);

        if (!hasAccess)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        var columns = await _db.BoardColumns
            .Where(c => c.ProjectId == request.ProjectId)
            .OrderBy(c => c.Position)
            .Select(c => new ColumnDetailDto(
                c.Id,
                c.Name,
                c.Position,
                c.ProjectId,
                c.Tasks.Count,
                c.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return columns;
    }
}