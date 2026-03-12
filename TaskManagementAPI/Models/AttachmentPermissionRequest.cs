namespace TaskManagementAPI.Models
{
public class AttachmentPermissionRequest
        {
            public int Id { get; set; }
            public int TaskId { get; set; }
            public int RequestedByUserId { get; set; }
            public string RequestType { get; set; } = "Attachment"; // "Attachment" or "Edit"
            public string Message { get; set; } = string.Empty;
            public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public int? ReviewedByUserId { get; set; }
            public DateTime? ReviewedAt { get; set; }

            public TaskItem? Task { get; set; }
            public User? RequestedByUser { get; set; }
            public User? ReviewedByUser { get; set; }
        }

}
