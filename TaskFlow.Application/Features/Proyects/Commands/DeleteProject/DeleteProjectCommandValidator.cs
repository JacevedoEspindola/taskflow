using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("El Id del proyecto es obligatorio.");
    }
}