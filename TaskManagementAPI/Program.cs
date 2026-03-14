using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;

/*
 * ============================================================================
 * ASP.NET CORE WEB API - PROGRAM.CS (APPLICATION ENTRY POINT)
 * ============================================================================
 * 
 * This file serves as the entry point for the ASP.NET Core Web API application.
 * It configures services, middleware, and the HTTP request pipeline.
 * 
 * LAYERED ARCHITECTURE EXPLANATION:
 * ---------------------------------
 * This application follows a layered architecture pattern:
 * 
 * 1. PRESENTATION LAYER (Angular Frontend)
 *    - User Interface components
 *    - Handles user interactions
 *    - Sends HTTP requests to API
 * 
 * 2. API LAYER (Controllers - This Project)
 *    - Receives HTTP requests from frontend
 *    - Validates input data using DTOs
 *    - Calls business logic
 *    - Returns HTTP responses
 * 
 * 3. BUSINESS LOGIC LAYER (Embedded in Controllers)
 *    - Implements business rules
 *    - Task status workflow validation
 *    - Role-based authorization logic
 *    - Data transformation
 * 
 * 4. DATA ACCESS LAYER (Entity Framework Core)
 *    - ApplicationDbContext manages database operations
 *    - LINQ queries for data retrieval
 *    - Entity models represent database tables
 *    - Migrations manage schema changes
 * 
 * 5. DATABASE LAYER (SQL Server/LocalDB)
 *    - Stores persistent data
 *    - Users, Tasks, TaskComments tables
 * 
 * MVC ARCHITECTURE IN WEB API:
 * ---------------------------
 * While this is a Web API (not MVC with Views), it follows MVC principles:
 * 
 * - MODEL: Entity classes (User, TaskItem, TaskComment) and DTOs
 *          Represent data structure and business entities
 * 
 * - VIEW: Angular frontend (separate project)
 *         Displays data to users
 * 
 * - CONTROLLER: API Controllers (AuthController, TaskController, UserController)
 *               Handle HTTP requests, process data, return responses
 * 
 * The separation allows frontend and backend to evolve independently.
 */

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// SERVICE CONFIGURATION (DEPENDENCY INJECTION CONTAINER)
// ============================================================================
// Services are registered here and injected into controllers via constructor

// Add MVC Controllers support for handling HTTP requests
builder.Services.AddControllers();

// Add API documentation support (Swagger/OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================================================================
// ENTITY FRAMEWORK CORE CONFIGURATION (DATA ACCESS LAYER)
// ============================================================================
// Configure DbContext with SQL Server connection
// DbContext is the bridge between C# objects and database tables
// Connection string is stored in appsettings.json for security and flexibility
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/*
 * ENTITY FRAMEWORK CORE MIGRATIONS WORKFLOW:
 * -----------------------------------------
 * 1. Create/Modify entity models (User.cs, TaskItem.cs, etc.)
 * 2. Run: dotnet ef migrations add MigrationName
 *    - Creates migration file with Up() and Down() methods
 *    - Up() applies changes, Down() reverts changes
 * 3. Run: dotnet ef database update
 *    - Applies pending migrations to database
 *    - Creates or modifies tables based on entity models
 * 4. Database schema now matches entity models
 * 
 * Benefits:
 * - Version control for database schema
 * - Easy rollback of database changes
 * - Team collaboration on database structure
 * - Automatic schema generation from code
 */

// ============================================================================
// SESSION CONFIGURATION (AUTHENTICATION STATE MANAGEMENT)
// ============================================================================
// Configure in-memory session storage for user authentication
builder.Services.AddDistributedMemoryCache(); // Stores session data in memory
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ============================================================================
// CORS CONFIGURATION (CROSS-ORIGIN RESOURCE SHARING)
// ============================================================================
// Allow Angular frontend (different origin) to make requests to this API
// Without CORS, browsers block requests from http://localhost:4200 to http://localhost:5150
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Allow requests from Angular dev server
                  .AllowAnyHeader()                      // Allow any HTTP headers
                  .AllowAnyMethod()                      // Allow GET, POST, PUT, DELETE, etc.
                  .AllowCredentials();                   // Allow cookies (needed for session)
        });
});

var app = builder.Build();

// ============================================================================
// DATABASE SEEDING (INITIAL DATA)
// ============================================================================
// Seed database with initial users for testing
// Creates Admin, Manager, and Employee users if they don't exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbSeeder.SeedData(context);
}

// ============================================================================
// MIDDLEWARE PIPELINE CONFIGURATION (HTTP REQUEST PROCESSING)
// ============================================================================
/*
 * MIDDLEWARE EXPLANATION:
 * ----------------------
 * Middleware components form a pipeline that processes HTTP requests and responses.
 * Each middleware can:
 * 1. Process the incoming request
 * 2. Pass control to the next middleware
 * 3. Process the outgoing response
 * 
 * REQUEST FLOW (Top to Bottom):
 * Client → CORS → Session → Authorization → Controllers → Business Logic → Database
 * 
 * RESPONSE FLOW (Bottom to Top):
 * Database → Business Logic → Controllers → Authorization → Session → CORS → Client
 * 
 * ORDER MATTERS! Middleware executes in the order it's added.
 * 
 * HTTP REQUEST-RESPONSE LIFECYCLE:
 * -------------------------------
 * 1. Client (Angular) sends HTTP request (e.g., POST /api/auth/login)
 * 2. Request enters ASP.NET Core pipeline
 * 3. CORS middleware checks if request origin is allowed
 * 4. Session middleware loads session data from cookie
 * 5. Authorization middleware checks user permissions (if configured)
 * 6. Routing middleware matches URL to controller action
 * 7. Controller action executes:
 *    - Validates input (DTOs)
 *    - Executes business logic
 *    - Queries database using LINQ and EF Core
 *    - Prepares response data
 * 8. Response travels back through middleware pipeline
 * 9. Session middleware saves session changes to cookie
 * 10. CORS middleware adds appropriate headers
 * 11. Response sent back to client
 * 
 * EXCEPTION HANDLING:
 * ------------------
 * Controllers use try-catch blocks to handle exceptions:
 * - Database errors (connection failures, constraint violations)
 * - Validation errors (invalid data)
 * - Business logic errors (invalid state transitions)
 * 
 * Exceptions are caught and converted to appropriate HTTP status codes:
 * - 400 Bad Request: Invalid input data
 * - 401 Unauthorized: Not authenticated
 * - 403 Forbidden: Not authorized for this action
 * - 404 Not Found: Resource doesn't exist
 * - 500 Internal Server Error: Unexpected errors
 */

// Enable Swagger UI in development for API testing
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS policy (must be before other middleware that handles requests)
app.UseCors("AllowAngular");

// Enable static file serving for uploaded files
app.UseStaticFiles();

// Enable session middleware (loads session data from cookie)
app.UseSession();

// Enable authorization middleware (checks user permissions)
app.UseAuthorization();

// Map controller routes (enables routing to controller actions)
app.MapControllers();

// Start the application and listen for HTTP requests
app.Run();
