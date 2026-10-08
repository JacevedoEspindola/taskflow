using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(
        IApplicationDbContext db,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // 1. Verificar si el email ya existe
        var emailExists = await _db.Users
            .AnyAsync(u => u.Email == email, cancellationToken);

        if (emailExists)
            throw new InvalidOperationException($"El email '{email}' ya está registrado.");

        // 2. Crear el usuario
        var user = new User
        {
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        // 3. Guardar en la BD
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        // 4. Generar token JWT
        var token = _jwtService.GenerateToken(user);

        // 5. Devolver respuesta
        return new AuthResponse(
            user.Id,
            user.Email,
            user.FullName,
            token
        );
    }
}