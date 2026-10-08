using System;
using System.Collections.Generic;
using System.Text;
namespace TaskFlow.Application.Features.Auth.Dtos;

public record AuthResponse(
    Guid UserId,
    string Email,
    string FullName,
    string Token
);