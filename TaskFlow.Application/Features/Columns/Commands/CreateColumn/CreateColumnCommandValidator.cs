using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Columns.Commands.CreateColumn;

public class CreateColumnCommandValidator : AbstractValidator<CreateColumnCommand>
{
    public CreateColumnCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty().WithMessage("Project id is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Column name is required.")
            .MinimumLength(2).WithMessage("Column name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Column name cannot exceed 100 characters.");
    }
}