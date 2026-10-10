using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Features.Tasks.Dtos;

public record MoveTaskRequest(
    Guid TargetColumnId,
    int? Position
);