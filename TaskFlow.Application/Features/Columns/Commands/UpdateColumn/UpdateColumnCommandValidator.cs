using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Columns.Commands.UpdateColumn;

public class UpdateColumnCommandValidator : AbstractValidator<UpdateColumnCommand>
{
    public UpdateColumnCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ColumnId).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la columna es obligatorio.")
            .MinimumLength(2).WithMessage("Debe tener al menos 2 caracteres.")
            .MaximumLength(100).WithMessage("No puede superar los 100 caracteres.");

        RuleFor(x => x.Position)
            .GreaterThan(0).WithMessage("La posición debe ser mayor a 0.")
            .When(x => x.Position.HasValue);
    }
}