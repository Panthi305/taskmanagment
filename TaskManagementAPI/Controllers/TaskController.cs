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

                // Start building LINQ query
                // Include() performs eager loading to avoid N+1 query problem
                IQueryable<TaskItem> query = _context.Tasks
                    .Include(t => t.AssignedByUser)  // Load creator user data
                    .Include(t => t.AssignedToUser); // Load assignee user data

                // Apply role-based filtering using LINQ Where() method
                if (userRole == "Manager")
                {
                    // Manager sees only tasks they created
                    // LINQ: Where(t => t.AssignedBy == userId.Value)
                    // SQL: WHERE AssignedBy = @userId
                    query = query.Where(t => t.AssignedBy == userId.Value);
                }
                else if (userRole == "Employee")
                {
                    // Employee sees only tasks assigned to them
                    // LINQ: Where(t => t.AssignedTo == userId.Value)
                    // SQL: WHERE AssignedTo = @userId
                    query = query.Where(t => t.AssignedTo == userId.Value);
                }
                // Admin: No filter applied, sees all tasks

                // Execute query and project results into TaskDto
                // Select() transforms TaskItem entities into TaskDto objects
                var tasks = await query
                    .Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        AssignedBy = t.AssignedBy,
                        AssignedByName = t.AssignedByUser!.Name,  // From included navigation property
                        AssignedTo = t.AssignedTo,
                        AssignedToName = t.AssignedToUser!.Name,  // From included navigation property
                        Priority = t.Priority,
                        Status = t.Status,
                        CreatedAt = t.CreatedAt,
                        StartDate = t.StartDate,
                        CompletedDate = t.CompletedDate
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

                // Create new TaskItem entity from DTO
                var task = new TaskItem
                {
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    AssignedBy = userId.Value,              // Set from session
                    AssignedTo = createTaskDto.AssignedTo,
                    Priority = createTaskDto.Priority,
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
                var userRole = HttpContext.Session.GetString("UserRole");
                
                if (userId == null)
                {
                    return Unauthorized();
                }

                // STRICT RULE: Only employees can start tasks
                if (userRole == "Admin" || userRole == "Manager")
                {
                    return Forbid(); // 403 - Admin/Manager cannot start tasks
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
                var userRole = HttpContext.Session.GetString("UserRole");
                
                if (userId == null)
                {
                    return Unauthorized();
                }

                // STRICT RULE: Only employees can complete tasks
                if (userRole == "Admin" || userRole == "Manager")
                {
                    return Forbid(); // 403 - Admin/Manager cannot complete tasks
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
    }
}
