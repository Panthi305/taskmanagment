namespace TaskManagementAPI.DTOs
{
    public class TaskEditRequestDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public int RequestedByUserId { get; set; }
        public string RequestedByUserName { get; set; } = string.Empty;
        public string RequestMessage { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int? ReviewedByUserId { get; set; }
        public string? ReviewedByUserName { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }

    public class CreateTaskEditRequestDto
    {
        public string RequestMessage { get; set; } = string.Empty;
    }
}
