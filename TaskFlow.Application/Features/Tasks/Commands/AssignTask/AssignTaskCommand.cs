using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Commands.AssignTask;

public record AssignTaskCommand(
    Guid TaskId,
    Guid? AssigneeId
) : IRequest<TaskDetailDto>;