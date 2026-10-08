using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.Dtos;

namespace TaskFlow.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IApplicationDbContext db,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // 1. Buscar usuario por email
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        // 2. Verificar contraseña
        var passwordOk = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!passwordOk)
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        // 3. Generar token
        var token = _jwtService.GenerateToken(user);

        // 4. Devolver respuesta
        return new AuthResponse(
            user.Id,
            user.Email,
            user.FullName,
            token
        );
    }
}