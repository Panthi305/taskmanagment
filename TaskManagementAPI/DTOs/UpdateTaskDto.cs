namespace TaskManagementAPI.DTOs
{
    public class UpdateTaskDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? AssignedTo { get; set; }
        public string? Priority { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
