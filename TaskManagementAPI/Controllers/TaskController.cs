using Microsoft.AspNetCore.Mvc;
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

                // Role-based access control
                // Admin can view any task
                // Manager can view any task (for organizational visibility)
                // Employee can only view tasks assigned to them
                if (userRole == "Employee" && task.AssignedTo != userId.Value)
                {
                    return Forbid();
                }

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

                // Role-based access control
                // Admin and Manager can view any task's progress
                // Employee can only view progress for tasks assigned to them
                if (userRole == "Employee" && task.AssignedTo != userId.Value)
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

                // Only assigned user can upload attachments
                if (task.AssignedTo != userId.Value)
                {
                    return Forbid();
                }

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

                // Role-based access control
                // Admin and Manager can view any task's attachments
                // Employee can only view attachments for tasks assigned to them
                if (userRole == "Employee" && task.AssignedTo != userId.Value)
                {
                    return Forbid();
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

                // Role-based access control
                // Admin and Manager can view any task's comments
                // Employee can only view comments for tasks assigned to them
                if (userRole == "Employee" && task.AssignedTo != userId.Value)
                {
                    return Forbid();
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
    }
}
