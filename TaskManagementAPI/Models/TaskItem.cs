namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AssignedBy { get; set; }
        public int AssignedTo { get; set; }
        public string Priority { get; set; } = "Medium"; // Low, Medium, High
        public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public User? AssignedByUser { get; set; }
        public User? AssignedToUser { get; set; }
        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }
}
