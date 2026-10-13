using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Columns.Commands.UpdateColumn;

public class UpdateColumnCommandValidator : AbstractValidator<UpdateColumnCommand>
{
    public UpdateColumnCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty().WithMessage("Project id is required.");
        RuleFor(x => x.ColumnId).NotEmpty().WithMessage("Column id is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Column name is required.")
            .MinimumLength(2).WithMessage("Column name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Column name cannot exceed 100 characters.");
        RuleFor(x => x.Position)
            .GreaterThan(0).WithMessage("Position must be greater than 0.")
            .When(x => x.Position.HasValue);
    }
}