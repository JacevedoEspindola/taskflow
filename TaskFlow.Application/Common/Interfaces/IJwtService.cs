using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}