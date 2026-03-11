namespace TaskManagementAPI.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AssignedTo { get; set; }
        public string Priority { get; set; } = "Medium";
    }
}
