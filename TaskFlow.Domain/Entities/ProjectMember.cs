using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Domain.Entities
{
    public enum ProjectRole
    {
        Owner = 1,
        Admin = 2,
        Member = 3
    }

    public class ProjectMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
        public ProjectRole Role { get; set; } = ProjectRole.Member;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public User User { get; set; } = null!;
        public Project Project { get; set; } = null!;
    }
}
