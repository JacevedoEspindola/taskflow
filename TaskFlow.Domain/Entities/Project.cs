using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public User Owner { get; set; } = null!;
        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
        public ICollection<BoardColumn> Columns { get; set; } = new List<BoardColumn>();
    }
}
