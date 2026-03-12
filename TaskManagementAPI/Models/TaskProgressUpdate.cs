namespace TaskManagementAPI.Models
{
    public class TaskProgressUpdate
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? FilePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public TaskItem? Task { get; set; }
        public User? User { get; set; }
    }
}
