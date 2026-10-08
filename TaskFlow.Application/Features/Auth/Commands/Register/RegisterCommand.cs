using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Auth.Dtos;

namespace TaskFlow.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FullName
) : IRequest<AuthResponse>;