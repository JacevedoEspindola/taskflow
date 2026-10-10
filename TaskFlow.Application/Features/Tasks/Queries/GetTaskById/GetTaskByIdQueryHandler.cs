using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetTaskByIdQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TaskDetailDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Debes iniciar sesión.");

        var task = await _db.TaskItems
            .Where(t => t.Id == request.TaskId)
            .Where(t => t.Column.Project.Members.Any(m => m.UserId == userId))
            .Select(t => new TaskDetailDto(
                t.Id,
                t.Title,
                t.Description,
                t.Priority,
                t.DueDate,
                t.Position,
                t.ColumnId,
                t.Column.Name,
                t.Column.ProjectId,
                t.AssigneeId,
                t.Assignee != null ? t.Assignee.FullName : null,
                t.Assignee != null ? t.Assignee.Email : null,
                t.CreatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (task is null)
            throw new KeyNotFoundException($"Tarea '{request.TaskId}' no encontrada o sin acceso.");

        return task;
    }
}