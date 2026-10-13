using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteTaskCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("You must be logged in.");

        var task = await _db.TaskItems
            .Include(t => t.Column)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task is null)
            throw new KeyNotFoundException($"Task '{request.TaskId}' not found.");

        var hasAccess = await _db.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == task.Column.ProjectId && pm.UserId == userId, cancellationToken);

        if (!hasAccess)
            throw new UnauthorizedAccessException("You do not have access to this project.");

        _db.TaskItems.Remove(task);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}