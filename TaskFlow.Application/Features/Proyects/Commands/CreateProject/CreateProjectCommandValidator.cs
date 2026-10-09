using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del proyecto es obligatorio.")
            .MinimumLength(3).WithMessage("Debe tener al menos 3 caracteres.")
            .MaximumLength(150).WithMessage("No puede superar los 150 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("La descripción no puede superar los 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}