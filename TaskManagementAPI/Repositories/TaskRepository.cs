using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByAssignedToAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.AssignedTo == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByAssignedByAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.AssignedBy == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByStatusAsync(string status)
        {
            return await _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByPriorityAsync(string priority)
        {
            return await _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.Priority == priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetFilteredAsync(string? status, string? priority, int? assignedTo, int? assignedBy)
        {
            var query = _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(t => t.Priority == priority);

            if (assignedTo.HasValue)
                query = query.Where(t => t.AssignedTo == assignedTo.Value);

            if (assignedBy.HasValue)
                query = query.Where(t => t.AssignedBy == assignedBy.Value);

            return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem?> GetByIdAndAssignedToAsync(int id, int userId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id && t.AssignedTo == userId);
        }

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return (await GetByIdAsync(task.Id))!;
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Tasks.CountAsync();
        }
    }
}
