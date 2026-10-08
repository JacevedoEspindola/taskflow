using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

   
    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
}