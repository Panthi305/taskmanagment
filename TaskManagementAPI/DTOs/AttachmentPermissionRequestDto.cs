namespace TaskManagementAPI.DTOs
{
    public class AttachmentPermissionRequestDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int RequestedByUserId { get; set; }
        public string RequestedByUserName { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int? ReviewedByUserId { get; set; }
        public string? ReviewedByUserName { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }

    public class CreateAttachmentPermissionRequestDto
    {
        public string RequestType { get; set; } = "Attachment"; // "Attachment" or "Edit"
        public string Message { get; set; } = string.Empty;
    }
}
