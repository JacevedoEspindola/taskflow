using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Tasks.Commands.MoveTask;

public class MoveTaskCommandValidator : AbstractValidator<MoveTaskCommand>
{
    public MoveTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty().WithMessage("Task id is required.");
        RuleFor(x => x.TargetColumnId).NotEmpty().WithMessage("Target column id is required.");
        RuleFor(x => x.Position)
            .GreaterThan(0).WithMessage("Position must be greater than 0.")
            .When(x => x.Position.HasValue);
    }
}