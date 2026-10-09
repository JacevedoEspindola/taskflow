using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Projects.Dtos;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetProjectByIdQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ProjectDetailDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        var project = await _db.Projects
            .Where(p => p.Id == request.ProjectId)
            .Where(p => p.Members.Any(m => m.UserId == userId))
            .Select(p => new ProjectDetailDto(
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAt,
                p.OwnerId,
                p.Columns
                    .OrderBy(c => c.Position)
                    .Select(c => new ColumnDto(
                        c.Id,
                        c.Name,
                        c.Position,
                        c.Tasks.Count
                    ))
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
            throw new KeyNotFoundException($"Proyecto '{request.ProjectId}' no encontrado o sin acceso.");

        return project;
    }
}