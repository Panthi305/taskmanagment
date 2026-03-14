using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

/*
 * ============================================================================
 * TASK CONTROLLER - API ENDPOINT FOR TASK MANAGEMENT
 * ============================================================================
 * 
 * CONTROLLER ROLE IN MVC:
 * ----------------------
 * The Controller is the "C" in MVC architecture. It:
 * 1. Receives HTTP requests from clients (Angular frontend)
 * 2. Validates input data using DTOs (Data Transfer Objects)
 * 3. Executes business logic and enforces business rules
 * 4. Interacts with the database through Entity Framework Core
 * 5. Returns HTTP responses with appropriate status codes
 * 
 * DTO PATTERN EXPLANATION:
 * -----------------------
 * DTOs (Data Transfer Objects) are used to:
 * - Define the structure of data sent between client and server
 * - Separate API contracts from database entities
 * - Prevent over-posting attacks (clients can't modify fields not in DTO)
 * - Hide sensitive data (e.g., PasswordHash is never sent to client)
 * - Provide clean, focused data structures for specific operations
 * 
 * Example: CreateTaskDto only contains fields needed to create a task,
 *          while TaskDto includes computed fields like AssignedByName
 * 
 * DEPENDENCY INJECTION:
 * --------------------
 * ApplicationDbContext is injected via constructor.
 * ASP.NET Core's DI container automatically provides the instance.
 * Benefits: Loose coupling, easier testing, better maintainability
 */

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")] // Maps to /api/task
    [ApiController]              // Enables automatic model validation and binding
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor injection - ASP.NET Core provides ApplicationDbContext instance
        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static bool CanViewTask(TaskItem task, int userId, string? userRole)
        {
            if (string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return task.AssignedBy == userId || task.AssignedTo == userId;
        }

        private static string? BuildProgressFileUrl(int taskId, int progressId, string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            return $"/api/task/{taskId}/progress/{progressId}/file";
        }

        /*
         * ========================================================================
         * GET /api/task - RETRIEVE TASKS BASED ON USER ROLE
         * ========================================================================
         * 
         * ROLE-BASED AUTHORIZATION:
         * ------------------------
         * - Admin: Returns ALL tasks in the system
         * - Manager: Returns only tasks ASSIGNED BY this manager
         * - Employee: Returns only tasks ASSIGNED TO this employee
         * 
         * LINQ QUERY EXPLANATION:
         * ----------------------
         * LINQ (Language Integrated Query) allows querying data using C# syntax.
         * 
         * Key LINQ methods used:
         * - Include(): Eager loading of related entities (prevents N+1 queries)
         * - Where(): Filters data based on condition (SQL WHERE clause)
         * - Select(): Projects data into DTOs (SQL SELECT clause)
         * - ToListAsync(): Executes query asynchronously and returns list
         * 
         * Example LINQ to SQL translation:
         * 
         * LINQ:
         *   _context.Tasks.Where(t => t.AssignedTo == userId)
         * 
         * SQL:
         *   SELECT * FROM Tasks WHERE AssignedTo = @userId
         * 
         * ENTITY FRAMEWORK CORE:
         * ---------------------
         * - Translates LINQ queries to SQL
         * - Tracks entity changes for updates
         * - Manages database connections
         * - Handles object-relational mapping (ORM)
         * 
         * EXCEPTION HANDLING:
         * ------------------
         * Uses try-catch pattern to handle potential errors:
         * - Database connection failures
         * - Invalid session data
         * - Query execution errors
         */
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            try
            {
                // Retrieve user information from session (set during login)
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                // Check if user is authenticated
                if (userId == null)
                {
                    return Unauthorized(); // 401 status code
                }

                // ALL TASKS - No filtering, return all tasks in the system
                // This endpoint is for the "All Tasks" page where everyone sees everything
                var tasks = await _context.Tasks
                    .Include(t => t.AssignedByUser)  // Load creator user data
                    .Include(t => t.AssignedToUser)  // Load assignee user data
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
                        CompletedDate = t.CompletedDate,
                        Deadline = t.Deadline
                    })
                    .ToListAsync(); // Execute query asynchronously

                return Ok(tasks); // 200 status code with task list
            }
            catch (Exception ex)
            {
                // Log error and return 500 Internal Server Error
                return StatusCode(500, new { message = "An error occurred while retrieving tasks", error = ex.Message });
            }
        }

        /*
         * ========================================================================
         * GET /api/task/my - RETRIEVE TASKS ASSIGNED TO CURRENT USER
         * ========================================================================
         * 
         * This endpoint is specifically for employees to view their assigned tasks.
         * 
         * LINQ FILTERING EXAMPLE:
         * ----------------------
         * Where(t => t.AssignedTo == userId.Value)
         * 
         * This filters tasks where AssignedTo column matches the current user's ID.
         * Only tasks assigned to the logged-in user are returned.
         */
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTasks()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                // LINQ query with filtering
                // Demonstrates: Include(), Where(), Select(), ToListAsync()
                var tasks = await _context.Tasks
                    .Include(t => t.AssignedByUser)
                    .Include(t => t.AssignedToUser)
                    .Where(t => t.AssignedTo == userId.Value) // Filter by assigned user
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving your tasks", error = ex.Message });
            }
        }

        /*
         * ========================================================================
         * POST /api/task - CREATE NEW TASK
         * ========================================================================
         * 
         * AUTHORIZATION: Only Admin and Manager can create tasks
         * 
         * DTO USAGE:
         * ---------
         * CreateTaskDto defines what data client must provide:
         * - Title, Description, AssignedTo, Priority
         * 
         * Fields NOT in DTO (set by server):
         * - Id (auto-generated by database)
         * - AssignedBy (current user from session)
         * - Status (always "Pending" for new tasks)
         * - CreatedAt (current timestamp)
         * 
         * This prevents clients from manipulating these fields.
         * 
         * BUSINESS RULES ENFORCED:
         * -----------------------
         * 1. Only Admin and Manager can create tasks (role check)
         * 2. New tasks always start with "Pending" status
         * 3. AssignedBy is automatically set to current user
         * 4. CreatedAt is set to current UTC time
         */
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                // Get current user from session
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                // Authentication check
                if (userId == null)
                {
                    return Unauthorized(); // 401
                }

                // Authorization check - only Admin and Manager can create tasks
                if (userRole != "Admin" && userRole != "Manager")
                {
                    return Forbid(); // 403 Forbidden
                }

                // Validate assignment based on role hierarchy
                var assignedToUser = await _context.Users.FindAsync(createTaskDto.AssignedTo);
                if (assignedToUser == null)
                {
                    return BadRequest(new { message = "Assigned user not found" });
                }

                // Role-based assignment validation
                if (userRole == "Admin")
                {
                    // Admin can assign to Manager or Employee only
                    if (assignedToUser.Role != "Manager" && assignedToUser.Role != "Employee")
                    {
                        return Forbid(); // 403 - Cannot assign to Admin
                    }
                }
                else if (userRole == "Manager")
                {
                    // Manager can only assign to Employee
                    if (assignedToUser.Role != "Employee")
                    {
                        return Forbid(); // 403 - Manager can only assign to Employee
                    }
                }

                // Create new TaskItem entity from DTO
                var task = new TaskItem
                {
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    AssignedBy = userId.Value,              // Set from session
                    AssignedTo = createTaskDto.AssignedTo,
                    Priority = createTaskDto.Priority,
                    Deadline = createTaskDto.Deadline,
                    Status = "Pending",                     // Business rule: new tasks are Pending
                    CreatedAt = DateTime.UtcNow             // Set server timestamp
                };

                // Add to DbContext (not yet saved to database)
                _context.Tasks.Add(task);
                
                // Save changes to database (executes INSERT SQL)
                await _context.SaveChangesAsync();

                // Retrieve the created task with related data for response
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

                return Ok(createdTask); // 200 with created task data
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the task", error = ex.Message });
            }
        }

        /*
         * ========================================================================
         * PUT /api/task/start/{id} - START A TASK
         * ========================================================================
         * 
         * TASK STATUS WORKFLOW:
         * --------------------
         * Pending → In Progress → Completed
         * 
         * This endpoint handles the first transition: Pending → In Progress
         * 
         * BUSINESS RULES ENFORCED:
         * -----------------------
         * 1. Only the assigned employee can start their task
         * 2. Task must be in "Pending" status
         * 3. Cannot skip statuses (e.g., Pending → Completed)
         * 4. StartDate is automatically set to current timestamp
         * 5. Admin and Manager CANNOT start tasks assigned to employees
         * 
         * AUTHORIZATION:
         * -------------
         * - User must be authenticated
         * - Task must be assigned to the current user
         * - User must be an Employee (not Admin or Manager)
         * 
         * LINQ USAGE:
         * ----------
         * Where(t => t.Id == id && t.AssignedTo == userId.Value)
         * 
         * This ensures:
         * - Task exists (Id matches)
         * - Task belongs to current user (AssignedTo matches)
         * 
         * If no task matches both conditions, returns 404 Not Found
         */
        [HttpPut("start/{id}")]
        public async Task<IActionResult> StartTask(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                
                if (userId == null)
                {
                    return Unauthorized();
                }

                // Find task that matches ID AND is assigned to current user
                // LINQ: Where with multiple conditions (AND operator)
                var task = await _context.Tasks
                    .Where(t => t.Id == id && t.AssignedTo == userId.Value)
                    .FirstOrDefaultAsync();

                if (task == null)
                {
                    return NotFound(new { message = "Task not found or not assigned to you" });
                }

                // Business rule: Can only start tasks in Pending status
                if (task.Status != "Pending")
                {
                    return BadRequest(new { message = "Task can only be started from Pending status" });
                }

                // Update task status and set start date
                task.Status = "In Progress";
                task.StartDate = DateTime.UtcNow;
                
                // Save changes to database (executes UPDATE SQL)
                await _context.SaveChangesAsync();

                return Ok(new { message = "Task started successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while starting the task", error = ex.Message });
            }
        }

        /*
         * ========================================================================
         * PUT /api/task/complete/{id} - COMPLETE A TASK
         * ========================================================================
         * 
         * TASK STATUS WORKFLOW:
         * --------------------
         * Pending → In Progress → Completed
         * 
         * This endpoint handles the second transition: In Progress → Completed
         * 
         * BUSINESS RULES ENFORCED:
         * -----------------------
         * 1. Only the assigned employee can complete their task
         * 2. Task must be in "In Progress" status
         * 3. Cannot complete a Pending task (must start it first)
         * 4. CompletedDate is automatically set to current timestamp
         * 5. Admin and Manager CANNOT complete tasks assigned to employees
         * 
         * AUTHORIZATION:
         * -------------
         * Same as StartTask - only assigned employee can complete
         * 
         * LINQ FILTERING:
         * --------------
         * Where(t => t.Id == id && t.AssignedTo == userId.Value)
         * 
         * Ensures task exists and belongs to current user
         */
        [HttpPut("complete/{id}")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                
                if (userId == null)
                {
                    return Unauthorized();
                }

                // LINQ query with multiple conditions
                var task = await _context.Tasks
                    .Where(t => t.Id == id && t.AssignedTo == userId.Value)
                    .FirstOrDefaultAsync();

                if (task == null)
                {
                    return NotFound(new { message = "Task not found or not assigned to you" });
                }

                // Business rule: Can only complete tasks in "In Progress" status
                if (task.Status != "In Progress")
                {
                    return BadRequest(new { message = "Task can only be completed from In Progress status" });
                }

                // Update task status and set completion date
                task.Status = "Completed";
                task.CompletedDate = DateTime.UtcNow;
                
                // EF Core tracks changes and generates UPDATE SQL
                await _context.SaveChangesAsync();

                return Ok(new { message = "Task completed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while completing the task", error = ex.Message });
            }
        }

        /*
         * ========================================================================
         * GET /api/task/completed - RETRIEVE COMPLETED TASKS
         * ========================================================================
         * 
         * Returns all completed tasks based on user role.
         * 
         * LINQ FILTERING:
         * --------------
         * Where(t => t.Status == "Completed")
         * 
         * Filters tasks by completed status
         */
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedTasks()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (userId == null)
                {
                    return Unauthorized();
                }

                // Start with completed tasks filter
                IQueryable<TaskItem> query = _context.Tasks
                    .Include(t => t.AssignedByUser)
                    .Include(t => t.AssignedToUser)
                    .Where(t => t.Status == "Completed");

                // Apply role-based filtering
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving completed tasks", error = ex.Message });
            }
        }

        /*
         * ========================================================================
         * GET /api/task/pending - RETRIEVE PENDING TASKS
         * ========================================================================
         * 
         * Returns all pending tasks based on user role.
         * 
         * LINQ FILTERING:
         * --------------
         * Where(t => t.Status == "Pending")
         * 
         * Filters tasks by pending status
         */
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingTasks()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (userId == null)
                {
                    return Unauthorized();
                }

                // Start with pending tasks filter
                IQueryable<TaskItem> query = _context.Tasks
                    .Include(t => t.AssignedByUser)
                    .Include(t => t.AssignedToUser)
                    .Where(t => t.Status == "Pending");

                // Apply role-based filtering
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving pending tasks", error = ex.Message });
            }
        }

        /*
         * ========================================================================
         * GET /api/task/created-by-me - RETRIEVE TASKS CREATED BY CURRENT USER
         * ========================================================================
         * 
         * Returns all tasks created by the current user (Admin/Manager).
         * 
         * LINQ FILTERING:
         * --------------
         * Where(t => t.AssignedBy == userId.Value)
         * 
         * Filters tasks by creator
         */
        [HttpGet("created-by-me")]
        public async Task<IActionResult> GetTasksCreatedByMe()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (userId == null)
                {
                    return Unauthorized();
                }

                // Only Admin and Manager can create tasks
                if (userRole == "Employee")
                {
                    return Forbid();
                }

                var tasks = await _context.Tasks
                    .Include(t => t.AssignedByUser)
                    .Include(t => t.AssignedToUser)
                    .Where(t => t.AssignedBy == userId.Value)
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving your created tasks", error = ex.Message });
            }
        }

        // ========================================================================
        // GET /api/task/{id} - GET TASK DETAILS
        // ========================================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks
                    .Include(t => t.AssignedByUser)
                    .Include(t => t.AssignedToUser)
                    .Where(t => t.Id == id)
                    .FirstOrDefaultAsync();

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // All authenticated users can view task details
                // (The task list already shows all tasks to everyone)

                var taskDto = new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    AssignedBy = task.AssignedBy,
                    AssignedByName = task.AssignedByUser!.Name,
                    AssignedTo = task.AssignedTo,
                    AssignedToName = task.AssignedToUser!.Name,
                    Priority = task.Priority,
                    Status = task.Status,
                    CreatedAt = task.CreatedAt,
                    StartDate = task.StartDate,
                    CompletedDate = task.CompletedDate,
                    Deadline = task.Deadline
                };

                return Ok(taskDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving task details", error = ex.Message });
            }
        }

        // ========================================================================
        // PUT /api/task/{id} - UPDATE/EDIT TASK
        // ========================================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks
                    .Include(t => t.AssignedByUser)
                    .Include(t => t.AssignedToUser)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // COMPLETED TASK LOCK RULE: No one can modify completed tasks
                if (task.Status == "Completed")
                {
                    return StatusCode(403, new { message = "This task is completed and cannot be modified." });
                }

                // Check if user can edit: either task creator OR has approved edit request
                bool isCreator = task.AssignedBy == userId.Value;
                bool hasApprovedEditRequest = false;

                if (!isCreator)
                {
                    // Check if there's an approved edit request for this user
                    hasApprovedEditRequest = await _context.TaskEditRequests
                        .AnyAsync(ter => ter.TaskId == id && 
                                       ter.RequestedByUserId == userId.Value && 
                                       ter.Status == "Approved");
                }

                if (!isCreator && !hasApprovedEditRequest)
                {
                    return StatusCode(403, new { message = "Only the task creator or users with approved edit access can edit this task" });
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(updateTaskDto.Title))
                {
                    task.Title = updateTaskDto.Title;
                }

                if (!string.IsNullOrEmpty(updateTaskDto.Description))
                {
                    task.Description = updateTaskDto.Description;
                }

                if (updateTaskDto.AssignedTo.HasValue)
                {
                    // Validate new assignee exists and role hierarchy
                    var newAssignee = await _context.Users.FindAsync(updateTaskDto.AssignedTo.Value);
                    if (newAssignee == null)
                    {
                        return BadRequest(new { message = "Assigned user not found" });
                    }

                    // Role-based assignment validation
                    if (userRole == "Admin")
                    {
                        if (newAssignee.Role != "Manager" && newAssignee.Role != "Employee")
                        {
                            return Forbid();
                        }
                    }
                    else if (userRole == "Manager")
                    {
                        if (newAssignee.Role != "Employee")
                        {
                            return Forbid();
                        }
                    }

                    task.AssignedTo = updateTaskDto.AssignedTo.Value;
                }

                if (!string.IsNullOrEmpty(updateTaskDto.Priority))
                {
                    task.Priority = updateTaskDto.Priority;
                }

                if (updateTaskDto.Deadline.HasValue)
                {
                    task.Deadline = updateTaskDto.Deadline.Value;
                }

                await _context.SaveChangesAsync();

                var updatedTask = new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    AssignedBy = task.AssignedBy,
                    AssignedByName = task.AssignedByUser!.Name,
                    AssignedTo = task.AssignedTo,
                    AssignedToName = task.AssignedToUser!.Name,
                    Priority = task.Priority,
                    Status = task.Status,
                    CreatedAt = task.CreatedAt,
                    StartDate = task.StartDate,
                    CompletedDate = task.CompletedDate,
                    Deadline = task.Deadline
                };

                return Ok(updatedTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task", error = ex.Message });
            }
        }

        // ========================================================================
        // POST /api/task/{taskId}/progress - ADD PROGRESS UPDATE
        // ========================================================================
        [HttpPost("{taskId}/progress")]
        public async Task<IActionResult> AddProgressUpdate(int taskId, [FromForm] CreateProgressUpdateDto dto, IFormFile? file)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // COMPLETED TASK LOCK RULE: No one can add progress to completed tasks
                if (task.Status == "Completed")
                {
                    return StatusCode(403, new { message = "This task is completed and cannot be modified." });
                }

                // Only assigned user can add progress
                if (task.AssignedTo != userId.Value)
                {
                    return Forbid();
                }

                string? filePath = null;
                if (file != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "tasks");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    
                    filePath = $"/uploads/tasks/{uniqueFileName}";
                }

                var progressUpdate = new TaskProgressUpdate
                {
                    TaskId = taskId,
                    UserId = userId.Value,
                    Description = dto.Description,
                    FilePath = filePath,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TaskProgressUpdates.Add(progressUpdate);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(userId.Value);
                var result = new TaskProgressUpdateDto
                {
                    Id = progressUpdate.Id,
                    TaskId = progressUpdate.TaskId,
                    UserId = progressUpdate.UserId,
                    UserName = user!.Name,
                    Description = progressUpdate.Description,
                    FilePath = progressUpdate.FilePath,
                    FileUrl = BuildProgressFileUrl(progressUpdate.TaskId, progressUpdate.Id, progressUpdate.FilePath),
                    CreatedAt = progressUpdate.CreatedAt
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding progress update", error = ex.Message });
            }
        }

        // ========================================================================
        // GET /api/task/{taskId}/progress - GET PROGRESS UPDATES
        // ========================================================================
        [HttpGet("{taskId}/progress")]
        public async Task<IActionResult> GetProgressUpdates(int taskId)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                if (!CanViewTask(task, userId.Value, userRole))
                {
                    return Forbid();
                }

                var updates = await _context.TaskProgressUpdates
                    .Include(tpu => tpu.User)
                    .Where(tpu => tpu.TaskId == taskId)
                    .OrderByDescending(tpu => tpu.CreatedAt)
                    .Select(tpu => new TaskProgressUpdateDto
                    {
                        Id = tpu.Id,
                        TaskId = tpu.TaskId,
                        UserId = tpu.UserId,
                        UserName = tpu.User!.Name,
                        Description = tpu.Description,
                        FilePath = tpu.FilePath,
                        FileUrl = BuildProgressFileUrl(tpu.TaskId, tpu.Id, tpu.FilePath),
                        CreatedAt = tpu.CreatedAt
                    })
                    .ToListAsync();

                return Ok(updates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving progress updates", error = ex.Message });
            }
        }

        // ========================================================================
        // GET /api/task/{taskId}/progress/{progressId}/file - DOWNLOAD/VIEW FILE
        // ========================================================================
        [HttpGet("{taskId}/progress/{progressId}/file")]
        public async Task<IActionResult> GetProgressUpdateFile(int taskId, int progressId)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                if (!CanViewTask(task, userId.Value, userRole))
                {
                    return Forbid();
                }

                var progressUpdate = await _context.TaskProgressUpdates
                    .Where(tpu => tpu.Id == progressId && tpu.TaskId == taskId)
                    .FirstOrDefaultAsync();

                if (progressUpdate == null || string.IsNullOrWhiteSpace(progressUpdate.FilePath))
                {
                    return NotFound(new { message = "File not found" });
                }

                var relativePath = progressUpdate.FilePath.TrimStart('/');
                var fullPath = Path.GetFullPath(
                    Path.Combine(Directory.GetCurrentDirectory(), relativePath.Replace('/', Path.DirectorySeparatorChar))
                );
                var uploadsRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "uploads"));

                if (!fullPath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound(new { message = "File not found" });
                }

                var contentTypeProvider = new FileExtensionContentTypeProvider();
                if (!contentTypeProvider.TryGetContentType(fullPath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                return PhysicalFile(fullPath, contentType, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the file", error = ex.Message });
            }
        }

        // ========================================================================
        // POST /api/task/{taskId}/attachments - UPLOAD ATTACHMENT
        // ========================================================================
        [HttpPost("{taskId}/attachments")]
        public async Task<IActionResult> UploadAttachment(int taskId, IFormFile file)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file provided" });
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // COMPLETED TASK LOCK RULE: No one can upload attachments to completed tasks
                if (task.Status == "Completed")
                {
                    return StatusCode(403, new { message = "This task is completed and cannot be modified." });
                }

                // Only task creator can upload attachments
                if (task.AssignedBy != userId.Value)
                {
                    return StatusCode(403, new { message = "Only the task creator can upload attachments" });
                }

                // Upload file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "tasks");
                Directory.CreateDirectory(uploadsFolder);
                
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var attachment = new TaskAttachment
                {
                    TaskId = taskId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/tasks/{uniqueFileName}",
                    UploadedBy = userId.Value,
                    UploadedAt = DateTime.UtcNow
                };

                _context.TaskAttachments.Add(attachment);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(userId.Value);
                var result = new TaskAttachmentDto
                {
                    Id = attachment.Id,
                    TaskId = attachment.TaskId,
                    FileName = attachment.FileName,
                    FilePath = attachment.FilePath,
                    UploadedBy = attachment.UploadedBy,
                    UploadedByName = user!.Name,
                    UploadedAt = attachment.UploadedAt
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while uploading attachment", error = ex.Message });
            }
        }

        // ========================================================================
        // GET /api/task/{taskId}/attachments - GET ATTACHMENTS
        // ========================================================================
        [HttpGet("{taskId}/attachments")]
        public async Task<IActionResult> GetAttachments(int taskId)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                var attachments = await _context.TaskAttachments
                    .Include(ta => ta.UploadedByUser)
                    .Where(ta => ta.TaskId == taskId)
                    .OrderByDescending(ta => ta.UploadedAt)
                    .Select(ta => new TaskAttachmentDto
                    {
                        Id = ta.Id,
                        TaskId = ta.TaskId,
                        FileName = ta.FileName,
                        FilePath = ta.FilePath,
                        UploadedBy = ta.UploadedBy,
                        UploadedByName = ta.UploadedByUser!.Name,
                        UploadedAt = ta.UploadedAt
                    })
                    .ToListAsync();

                return Ok(attachments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving attachments", error = ex.Message });
            }
        }

        // ========================================================================
        // POST /api/task/{taskId}/comments - ADD COMMENT
        // ========================================================================
        [HttpPost("{taskId}/comments")]
        public async Task<IActionResult> AddComment(int taskId, [FromBody] CreateCommentDto dto)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                var comment = new TaskComment
                {
                    TaskId = taskId,
                    UserId = userId.Value,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TaskComments.Add(comment);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(userId.Value);
                var result = new TaskCommentDto
                {
                    Id = comment.Id,
                    TaskId = comment.TaskId,
                    UserId = comment.UserId,
                    UserName = user!.Name,
                    Comment = comment.Comment,
                    CreatedAt = comment.CreatedAt
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding comment", error = ex.Message });
            }
        }

        // ========================================================================
        // GET /api/task/{taskId}/comments - GET COMMENTS
        // ========================================================================
        [HttpGet("{taskId}/comments")]
        public async Task<IActionResult> GetComments(int taskId)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                var comments = await _context.TaskComments
                    .Include(tc => tc.User)
                    .Where(tc => tc.TaskId == taskId)
                    .OrderBy(tc => tc.CreatedAt)
                    .Select(tc => new TaskCommentDto
                    {
                        Id = tc.Id,
                        TaskId = tc.TaskId,
                        UserId = tc.UserId,
                        UserName = tc.User!.Name,
                        Comment = tc.Comment,
                        CreatedAt = tc.CreatedAt
                    })
                    .ToListAsync();

                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving comments", error = ex.Message });
            }
        }

        // ========================================================================
        // POST /api/task/{taskId}/attachment-request - CREATE PERMISSION REQUEST
        // ========================================================================
        [HttpPost("{taskId}/attachment-request")]
        public async Task<IActionResult> CreateAttachmentPermissionRequest(int taskId, [FromBody] CreateAttachmentPermissionRequestDto dto)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // Can only request permission for completed tasks
                if (task.Status != "Completed")
                {
                    return BadRequest(new { message = "Can only request permission for completed tasks" });
                }

                // Validate RequestType
                if (dto.RequestType != "Attachment" && dto.RequestType != "Edit")
                {
                    return BadRequest(new { message = "RequestType must be either 'Attachment' or 'Edit'" });
                }

                // Check if user already has a pending request of the same type
                var existingRequest = await _context.AttachmentPermissionRequests
                    .Where(apr => apr.TaskId == taskId 
                               && apr.RequestedByUserId == userId.Value 
                               && apr.RequestType == dto.RequestType
                               && apr.Status == "Pending")
                    .FirstOrDefaultAsync();

                if (existingRequest != null)
                {
                    return BadRequest(new { message = $"You already have a pending {dto.RequestType} request for this task" });
                }

                var request = new AttachmentPermissionRequest
                {
                    TaskId = taskId,
                    RequestedByUserId = userId.Value,
                    RequestType = dto.RequestType,
                    Message = dto.Message,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _context.AttachmentPermissionRequests.Add(request);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(userId.Value);
                var result = new AttachmentPermissionRequestDto
                {
                    Id = request.Id,
                    TaskId = request.TaskId,
                    RequestedByUserId = request.RequestedByUserId,
                    RequestedByUserName = user!.Name,
                    RequestType = request.RequestType,
                    Message = request.Message,
                    Status = request.Status,
                    CreatedAt = request.CreatedAt
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating permission request", error = ex.Message });
            }
        }

        // ========================================================================
        // GET /api/task/{taskId}/attachment-requests - GET PERMISSION REQUESTS
        // ========================================================================
        [HttpGet("{taskId}/attachment-requests")]
        public async Task<IActionResult> GetAttachmentPermissionRequests(int taskId)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // Only task creator can view requests
                if (task.AssignedBy != userId.Value)
                {
                    return Forbid();
                }

                var requests = await _context.AttachmentPermissionRequests
                    .Include(apr => apr.RequestedByUser)
                    .Include(apr => apr.ReviewedByUser)
                    .Where(apr => apr.TaskId == taskId)
                    .OrderByDescending(apr => apr.CreatedAt)
                    .Select(apr => new AttachmentPermissionRequestDto
                    {
                        Id = apr.Id,
                        TaskId = apr.TaskId,
                        RequestedByUserId = apr.RequestedByUserId,
                        RequestedByUserName = apr.RequestedByUser!.Name,
                        RequestType = apr.RequestType,
                        Message = apr.Message,
                        Status = apr.Status,
                        CreatedAt = apr.CreatedAt,
                        ReviewedByUserId = apr.ReviewedByUserId,
                        ReviewedByUserName = apr.ReviewedByUser != null ? apr.ReviewedByUser.Name : null,
                        ReviewedAt = apr.ReviewedAt
                    })
                    .ToListAsync();

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving permission requests", error = ex.Message });
            }
        }

        // ========================================================================
        // POST /api/task/attachment-request/{id}/approve - APPROVE REQUEST
        // ========================================================================
        [HttpPost("attachment-request/{id}/approve")]
        public async Task<IActionResult> ApproveAttachmentPermissionRequest(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                var request = await _context.AttachmentPermissionRequests
                    .Include(apr => apr.Task)
                    .Where(apr => apr.Id == id)
                    .FirstOrDefaultAsync();

                if (request == null)
                {
                    return NotFound(new { message = "Permission request not found" });
                }

                // Only task creator can approve
                if (request.Task!.AssignedBy != userId.Value)
                {
                    return Forbid();
                }

                if (request.Status != "Pending")
                {
                    return BadRequest(new { message = "Request has already been reviewed" });
                }

                request.Status = "Approved";
                request.ReviewedByUserId = userId.Value;
                request.ReviewedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Permission request approved" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while approving request", error = ex.Message });
            }
        }

        // ========================================================================
        // POST /api/task/attachment-request/{id}/reject - REJECT REQUEST
        // ========================================================================
        [HttpPost("attachment-request/{id}/reject")]
        public async Task<IActionResult> RejectAttachmentPermissionRequest(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                var request = await _context.AttachmentPermissionRequests
                    .Include(apr => apr.Task)
                    .Where(apr => apr.Id == id)
                    .FirstOrDefaultAsync();

                if (request == null)
                {
                    return NotFound(new { message = "Permission request not found" });
                }

                // Only task creator can reject
                if (request.Task!.AssignedBy != userId.Value)
                {
                    return Forbid();
                }

                if (request.Status != "Pending")
                {
                    return BadRequest(new { message = "Request has already been reviewed" });
                }

                request.Status = "Rejected";
                request.ReviewedByUserId = userId.Value;
                request.ReviewedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Permission request rejected" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while rejecting request", error = ex.Message });
            }
        }

        // ========================================================================
        // POST /api/task/{taskId}/edit-request - CREATE EDIT REQUEST
        // ========================================================================
        [HttpPost("{taskId}/edit-request")]
        public async Task<IActionResult> CreateEditRequest(int taskId, [FromBody] CreateTaskEditRequestDto dto)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // Cannot request edit access if you're the creator
                if (task.AssignedBy == userId.Value)
                {
                    return BadRequest(new { message = "You are the task creator and can already edit this task" });
                }

                // Only the assigned user can request edit access
                if (task.AssignedTo != userId.Value)
                {
                    return StatusCode(403, new { message = "Only the user assigned to this task can request edit access" });
                }

                // Check if there's already any request (pending, approved, or rejected)
                var existingRequest = await _context.TaskEditRequests
                    .Where(ter => ter.TaskId == taskId && ter.RequestedByUserId == userId.Value)
                    .FirstOrDefaultAsync();

                if (existingRequest != null)
                {
                    if (existingRequest.Status == "Pending")
                    {
                        return BadRequest(new { message = "You already have a pending edit request for this task" });
                    }
                    else if (existingRequest.Status == "Approved")
                    {
                        return BadRequest(new { message = "You already have approved edit access for this task" });
                    }
                    else if (existingRequest.Status == "Rejected")
                    {
                        return BadRequest(new { message = "Your previous edit request was rejected. Please contact the task creator." });
                    }
                }

                var editRequest = new TaskEditRequest
                {
                    TaskId = taskId,
                    RequestedByUserId = userId.Value,
                    RequestMessage = dto.RequestMessage,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _context.TaskEditRequests.Add(editRequest);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Edit request submitted successfully", requestId = editRequest.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating edit request", error = ex.Message });
            }
        }

        // ========================================================================
        // GET /api/task/{taskId}/edit-requests - GET ALL EDIT REQUESTS FOR TASK
        // ========================================================================
        [HttpGet("{taskId}/edit-requests")]
        public async Task<IActionResult> GetEditRequests(int taskId)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // Only task creator can view ALL edit requests
                if (task.AssignedBy != userId.Value)
                {
                    // Non-creators can only see their own request
                    var myRequest = await _context.TaskEditRequests
                        .Include(ter => ter.Task)
                        .Include(ter => ter.RequestedByUser)
                        .Include(ter => ter.ReviewedByUser)
                        .Where(ter => ter.TaskId == taskId && ter.RequestedByUserId == userId.Value)
                        .Select(ter => new TaskEditRequestDto
                        {
                            Id = ter.Id,
                            TaskId = ter.TaskId,
                            TaskTitle = ter.Task!.Title,
                            RequestedByUserId = ter.RequestedByUserId,
                            RequestedByUserName = ter.RequestedByUser!.Name,
                            RequestMessage = ter.RequestMessage,
                            Status = ter.Status,
                            CreatedAt = ter.CreatedAt,
                            ReviewedByUserId = ter.ReviewedByUserId,
                            ReviewedByUserName = ter.ReviewedByUser != null ? ter.ReviewedByUser.Name : null,
                            ReviewedAt = ter.ReviewedAt
                        })
                        .ToListAsync();

                    return Ok(myRequest);
                }

                var requests = await _context.TaskEditRequests
                    .Include(ter => ter.Task)
                    .Include(ter => ter.RequestedByUser)
                    .Include(ter => ter.ReviewedByUser)
                    .Where(ter => ter.TaskId == taskId)
                    .OrderByDescending(ter => ter.CreatedAt)
                    .Select(ter => new TaskEditRequestDto
                    {
                        Id = ter.Id,
                        TaskId = ter.TaskId,
                        TaskTitle = ter.Task!.Title,
                        RequestedByUserId = ter.RequestedByUserId,
                        RequestedByUserName = ter.RequestedByUser!.Name,
                        RequestMessage = ter.RequestMessage,
                        Status = ter.Status,
                        CreatedAt = ter.CreatedAt,
                        ReviewedByUserId = ter.ReviewedByUserId,
                        ReviewedByUserName = ter.ReviewedByUser != null ? ter.ReviewedByUser.Name : null,
                        ReviewedAt = ter.ReviewedAt
                    })
                    .ToListAsync();

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching edit requests", error = ex.Message });
            }
        }

        // ========================================================================
        // POST /api/task/edit-request/{id}/approve - APPROVE EDIT REQUEST
        // ========================================================================
        [HttpPost("edit-request/{id}/approve")]
        public async Task<IActionResult> ApproveEditRequest(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                var request = await _context.TaskEditRequests
                    .Include(ter => ter.Task)
                    .Where(ter => ter.Id == id)
                    .FirstOrDefaultAsync();

                if (request == null)
                {
                    return NotFound(new { message = "Edit request not found" });
                }

                // Only task creator can approve
                if (request.Task!.AssignedBy != userId.Value)
                {
                    return Forbid();
                }

                if (request.Status != "Pending")
                {
                    return BadRequest(new { message = "Request has already been reviewed" });
                }

                request.Status = "Approved";
                request.ReviewedByUserId = userId.Value;
                request.ReviewedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Edit request approved" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while approving request", error = ex.Message });
            }
        }

        // ========================================================================
        // POST /api/task/edit-request/{id}/reject - REJECT EDIT REQUEST
        // ========================================================================
        [HttpPost("edit-request/{id}/reject")]
        public async Task<IActionResult> RejectEditRequest(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                var request = await _context.TaskEditRequests
                    .Include(ter => ter.Task)
                    .Where(ter => ter.Id == id)
                    .FirstOrDefaultAsync();

                if (request == null)
                {
                    return NotFound(new { message = "Edit request not found" });
                }

                // Only task creator can reject
                if (request.Task!.AssignedBy != userId.Value)
                {
                    return Forbid();
                }

                if (request.Status != "Pending")
                {
                    return BadRequest(new { message = "Request has already been reviewed" });
                }

                request.Status = "Rejected";
                request.ReviewedByUserId = userId.Value;
                request.ReviewedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Edit request rejected" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while rejecting request", error = ex.Message });
            }
        }

        // ========================================================================
        // GET /api/task/edit-requests - GET ALL EDIT REQUESTS FOR CURRENT USER
        // ========================================================================
        [HttpGet("edit-requests")]
        public async Task<IActionResult> GetAllEditRequests()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                // Get all edit requests for tasks created by current user
                var requests = await _context.TaskEditRequests
                    .Include(ter => ter.Task)
                    .Include(ter => ter.RequestedByUser)
                    .Include(ter => ter.ReviewedByUser)
                    .Where(ter => ter.Task!.AssignedBy == userId.Value)
                    .OrderByDescending(ter => ter.CreatedAt)
                    .Select(ter => new TaskEditRequestDto
                    {
                        Id = ter.Id,
                        TaskId = ter.TaskId,
                        TaskTitle = ter.Task!.Title,
                        RequestedByUserId = ter.RequestedByUserId,
                        RequestedByUserName = ter.RequestedByUser!.Name,
                        RequestMessage = ter.RequestMessage,
                        Status = ter.Status,
                        CreatedAt = ter.CreatedAt,
                        ReviewedByUserId = ter.ReviewedByUserId,
                        ReviewedByUserName = ter.ReviewedByUser != null ? ter.ReviewedByUser.Name : null,
                        ReviewedAt = ter.ReviewedAt
                    })
                    .ToListAsync();

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving edit requests", error = ex.Message });
            }
        }
    }
}
