using System;
using System.Collections.Generic;
using System.Text;
namespace TaskFlow.Application.Features.Auth.Dtos;

public record RegisterRequest(
    string Email,
    string Password,
    string FullName
);