using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Projects.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateProjectCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ProjectDetailDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que el usuario está autenticado
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión para crear un proyecto.");

        // 2. Crear el proyecto
        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        // 3. Crear las 3 columnas default (To Do, In Progress, Done)
        var defaultColumns = new List<BoardColumn>
        {
            new() { Name = "To Do",       Position = 1, ProjectId = project.Id },
            new() { Name = "In Progress", Position = 2, ProjectId = project.Id },
            new() { Name = "Done",        Position = 3, ProjectId = project.Id }
        };

        // 4. Agregar al usuario como Owner del proyecto (ProjectMember)
        var member = new ProjectMember
        {
            UserId = userId,
            ProjectId = project.Id,
            Role = ProjectRole.Owner,
            JoinedAt = DateTime.UtcNow
        };

        // 5. Guardar todo
        _db.Projects.Add(project);
        _db.BoardColumns.AddRange(defaultColumns);
        _db.ProjectMembers.Add(member);

        await _db.SaveChangesAsync(cancellationToken);

        // 6. Devolver el DTO
        return new ProjectDetailDto(
            project.Id,
            project.Name,
            project.Description,
            project.CreatedAt,
            project.OwnerId,
            defaultColumns.Select(c => new ColumnDto(c.Id, c.Name, c.Position, 0))
        );
    }
}