using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                return Unauthorized();
            }

            IQueryable<TaskItem> query = _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser);

            if (userRole == "Manager")
            {
                query = query.Where(t => t.AssignedBy == userId.Value);
            }
            else if (userRole == "Employee")
            {
                query = query.Where(t => t.AssignedTo == userId.Value);
            }

            var tasks = await query
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedBy = t.AssignedBy,
                    AssignedByName = t.AssignedByUser!.Name,
                    AssignedTo = t.AssignedTo,
                    AssignedToName = t.AssignedToUser!.Name,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    StartDate = t.StartDate,
                    CompletedDate = t.CompletedDate
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var tasks = await _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.AssignedTo == userId.Value)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedBy = t.AssignedBy,
                    AssignedByName = t.AssignedByUser!.Name,
                    AssignedTo = t.AssignedTo,
                    AssignedToName = t.AssignedToUser!.Name,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    StartDate = t.StartDate,
                    CompletedDate = t.CompletedDate
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                return Unauthorized();
            }

            if (userRole != "Admin" && userRole != "Manager")
            {
                return Forbid();
            }

            var task = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                AssignedBy = userId.Value,
                AssignedTo = createTaskDto.AssignedTo,
                Priority = createTaskDto.Priority,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var createdTask = await _context.Tasks
                .Include(t => t.AssignedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.Id == task.Id)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedBy = t.AssignedBy,
                    AssignedByName = t.AssignedByUser!.Name,
                    AssignedTo = t.AssignedTo,
                    AssignedToName = t.AssignedToUser!.Name,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    StartDate = t.StartDate,
                    CompletedDate = t.CompletedDate
                })
                .FirstOrDefaultAsync();

            return Ok(createdTask);
        }

        [HttpPut("start/{id}")]
        public async Task<IActionResult> StartTask(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.AssignedTo == userId.Value)
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound(new { message = "Task not found or not assigned to you" });
            }

            if (task.Status != "Pending")
            {
                return BadRequest(new { message = "Task can only be started from Pending status" });
            }

            task.Status = "In Progress";
            task.StartDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Task started successfully" });
        }

        [HttpPut("complete/{id}")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.AssignedTo == userId.Value)
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound(new { message = "Task not found or not assigned to you" });
            }

            if (task.Status != "In Progress")
            {
                return BadRequest(new { message = "Task can only be completed from In Progress status" });
            }

            task.Status = "Completed";
            task.CompletedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Task completed successfully" });
        }
    }
}
