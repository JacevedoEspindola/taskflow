using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("Debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("Debe contener al menos una mayúscula.")
            .Matches("[a-z]").WithMessage("Debe contener al menos una minúscula.")
            .Matches("[0-9]").WithMessage("Debe contener al menos un número.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(150);
    }
}