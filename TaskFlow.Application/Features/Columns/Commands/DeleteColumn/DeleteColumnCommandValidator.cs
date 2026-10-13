using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Columns.Commands.DeleteColumn;

public class DeleteColumnCommandValidator : AbstractValidator<DeleteColumnCommand>
{
    public DeleteColumnCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty().WithMessage("Project id is required.");
        RuleFor(x => x.ColumnId).NotEmpty().WithMessage("Column id is required.");
    }
}