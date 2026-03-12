namespace TaskManagementAPI.DTOs
{
    public class TaskProgressUpdateDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateProgressUpdateDto
    {
        public string Description { get; set; } = string.Empty;
    }
}
