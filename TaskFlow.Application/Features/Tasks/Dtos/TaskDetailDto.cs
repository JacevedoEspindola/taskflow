using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Dtos;

public record TaskDetailDto(
    Guid Id,
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    int Position,
    Guid ColumnId,
    string ColumnName,
    Guid ProjectId,
    Guid? AssigneeId,
    string? AssigneeName,
    string? AssigneeEmail,
    DateTime CreatedAt
);