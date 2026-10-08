using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Domain.Entities
{
    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; set; }
        public int Position { get; set; }
        public Guid ColumnId { get; set; }
        public Guid? AssigneeId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public BoardColumn Column { get; set; } = null!;
        public User? Assignee { get; set; }
    }
}
