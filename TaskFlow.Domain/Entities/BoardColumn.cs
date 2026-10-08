using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Domain.Entities
{
    public class BoardColumn
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public int Position { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Project Project { get; set; } = null!;
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
