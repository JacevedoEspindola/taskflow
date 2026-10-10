using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    Guid ColumnId,
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate
) : IRequest<TaskDetailDto>;