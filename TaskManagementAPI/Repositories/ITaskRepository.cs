using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<IEnumerable<TaskItem>> GetByAssignedToAsync(int userId);
        Task<IEnumerable<TaskItem>> GetByAssignedByAsync(int userId);
        Task<IEnumerable<TaskItem>> GetByStatusAsync(string status);
        Task<IEnumerable<TaskItem>> GetByPriorityAsync(string priority);
        Task<IEnumerable<TaskItem>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<TaskItem>> GetFilteredAsync(string? status, string? priority, int? assignedTo, int? assignedBy);
        Task<TaskItem?> GetByIdAsync(int id);
        Task<TaskItem?> GetByIdAndAssignedToAsync(int id, int userId);
        Task<TaskItem> CreateAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task<int> GetTotalCountAsync();
    }
}
