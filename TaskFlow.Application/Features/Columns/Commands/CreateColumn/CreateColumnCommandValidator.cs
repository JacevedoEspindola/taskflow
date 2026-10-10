using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Columns.Commands.CreateColumn;

public class CreateColumnCommandValidator : AbstractValidator<CreateColumnCommand>
{
    public CreateColumnCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("El Id del proyecto es obligatorio.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la columna es obligatorio.")
            .MinimumLength(2).WithMessage("Debe tener al menos 2 caracteres.")
            .MaximumLength(100).WithMessage("No puede superar los 100 caracteres.");
    }
}