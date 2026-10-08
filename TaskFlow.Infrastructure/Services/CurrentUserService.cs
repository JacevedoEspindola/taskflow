using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null) return null;

            var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? user.FindFirst("sub")?.Value;

            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Email
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null) return null;

            return user.FindFirst(ClaimTypes.Email)?.Value
                ?? user.FindFirst("email")?.Value;
        }
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}