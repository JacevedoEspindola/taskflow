using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteProjectCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        // 1. Buscar el proyecto
        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project is null)
            throw new KeyNotFoundException($"Proyecto '{request.ProjectId}' no encontrado.");

        // 2. Verificar que el usuario es miembro y tiene rol Owner o Admin
        var membership = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == project.Id && pm.UserId == userId, cancellationToken);

        if (membership is null)
            throw new UnauthorizedAccessException("No tienes acceso a este proyecto.");

        if (membership.Role == ProjectRole.Member)
            throw new UnauthorizedAccessException("Solo Owners y Admins pueden eliminar el proyecto.");

        // 3. Borrar (el cascade borrará columnas, tareas y miembros)
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}