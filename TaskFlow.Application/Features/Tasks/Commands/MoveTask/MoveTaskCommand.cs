using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Commands.MoveTask;

public record MoveTaskCommand(
    Guid TaskId,
    Guid TargetColumnId,
    int? Position
) : IRequest<TaskDetailDto>;