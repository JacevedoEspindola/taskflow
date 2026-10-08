using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Auth.Dtos;

namespace TaskFlow.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponse>;
