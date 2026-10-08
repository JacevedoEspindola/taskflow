using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Project> Projects { get; }
    DbSet<ProjectMember> ProjectMembers { get; }
    DbSet<BoardColumn> BoardColumns { get; }
    DbSet<TaskItem> TaskItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}