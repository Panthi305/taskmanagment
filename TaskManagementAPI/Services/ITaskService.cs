using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllTasksAsync(string? userRole, int? userId);
    Task<IEnumerable<TaskDto>> GetMyTasksAsync(int userId);
    Task<IEnumerable<TaskDto>> GetCompletedTasksAsync(string? userRole, int? userId);
    Task<IEnumerable<TaskDto>> GetPendingTasksAsync(string? userRole, int? userId);
    Task<IEnumerable<TaskDto>> GetTasksCreatedByMeAsync(int? userId, string? userRole);
    Task<TaskDto?> CreateTaskAsync(CreateTaskDto dto, int userId, string userRole);
    Task<bool> StartTaskAsync(int taskId, int userId, string userRole);
    Task<bool> CompleteTaskAsync(int taskId, int userId, string userRole);
    Task<PaginatedResponse<TaskDto>> GetTasksPagedAsync(int page, int pageSize, string? userRole, int? userId);
    Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(string? status, string? priority, int? assignedTo, int? assignedBy, string? userRole, int? userId);
}
