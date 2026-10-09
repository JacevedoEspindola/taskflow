using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Projects.Dtos;

namespace TaskFlow.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateProjectCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ProjectDetailDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        // 1. Buscar el proyecto con sus columnas
        var project = await _db.Projects
            .Include(p => p.Columns)
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project is null)
            throw new KeyNotFoundException($"Proyecto '{request.ProjectId}' no encontrado.");

        // 2. Verificar que el usuario tiene permiso (Owner o Admin)
        var membership = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == project.Id && pm.UserId == userId, cancellationToken);

        if (membership is null)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        if (membership.Role == Domain.Entities.ProjectRole.Member)
            throw new UnauthorizedAccessException("Solo Owners y Admins pueden editar el proyecto.");

        // 3. Actualizar los campos
        project.Name = request.Name.Trim();
        project.Description = request.Description?.Trim();

        await _db.SaveChangesAsync(cancellationToken);

        // 4. Devolver DTO
        return new ProjectDetailDto(
            project.Id,
            project.Name,
            project.Description,
            project.CreatedAt,
            project.OwnerId,
            project.Columns
                .OrderBy(c => c.Position)
                .Select(c => new ColumnDto(c.Id, c.Name, c.Position, 0))
        );
    }
}