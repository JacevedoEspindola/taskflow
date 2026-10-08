using System;
using System.Collections.Generic;
using System.Text;
namespace TaskFlow.Application.Features.Auth.Dtos;

public record LoginRequest(
    string Email,
    string Password
);