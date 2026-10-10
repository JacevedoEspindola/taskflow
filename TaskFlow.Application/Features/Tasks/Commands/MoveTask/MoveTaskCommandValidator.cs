using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Tasks.Commands.MoveTask;

public class MoveTaskCommandValidator : AbstractValidator<MoveTaskCommand>
{
    public MoveTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.TargetColumnId).NotEmpty();

        RuleFor(x => x.Position)
            .GreaterThan(0).WithMessage("La posición debe ser mayor a 0.")
            .When(x => x.Position.HasValue);
    }
}