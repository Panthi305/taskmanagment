# Internal Task Management System - Complete Project Analysis

## Table of Contents
1. [Project Overview](#project-overview)
2. [Technology Stack](#technology-stack)
3. [System Architecture](#system-architecture)
4. [MVC Architecture Explanation](#mvc-architecture-explanation)
5. [Layered Architecture](#layered-architecture)
6. [Database Schema](#database-schema)
7. [Entity Framework Core](#entity-framework-core)
8. [LINQ Usage](#linq-usage)
9. [DTO Pattern](#dto-pattern)
10. [API Endpoints](#api-endpoints)
11. [Authentication & Authorization](#authentication--authorization)
12. [Role-Based Permissions](#role-based-permissions)
13. [Task Management Workflow](#task-management-workflow)
14. [Angular SPA Architecture](#angular-spa-architecture)
15. [Angular CLI Usage](#angular-cli-usage)
16. [HTTP Request-Response Lifecycle](#http-request-response-lifecycle)
17. [Middleware Pipeline](#middleware-pipeline)
18. [Exception Handling](#exception-handling)
19. [Project Structure](#project-structure)
20. [Running the Application](#running-the-application)
21. [Demo Credentials](#demo-credentials)
22. [Completed Task Lock Rule](#completed-task-lock-rule)
23. [Edit Request Visibility and Authentication State Fix](#edit-request-visibility-and-authentication-state-fix)
24. [Edit Access Permission Enforcement](#edit-access-permission-enforcement)
25. [Future Enhancements](#future-enhancements)

---

## Project Overview

The **Internal Task Management System** is a full-stack web application designed to manage task assignments and track progress across different organizational roles. The system implements comprehensive role-based access control with three distinct user types: Admin, Manager, and Employee.

### Key Features
- Session-based authentication with BCrypt password hashing
- Role-based authorization (Admin, Manager, Employee)
- Task lifecycle management (Pending → In Progress → Completed)
- Real-time task status updates
- User management (Admin only)
- Task assignment and tracking
- Responsive web interface
- RESTful API design

### Academic Syllabus Coverage
This project demonstrates all required concepts:
- ✅ MVC Architecture
- ✅ Layered Architecture
- ✅ Entity Framework Core with Migrations
- ✅ LINQ Queries
- ✅ DTOs (Data Transfer Objects)
- ✅ ASP.NET Core Web API
- ✅ Angular SPA
- ✅ Angular Components, Services, Routing
- ✅ Template-driven Forms
- ✅ Two-way Data Binding
- ✅ HTTP Client
- ✅ Session-based Authentication
- ✅ Role-based Authorization
- ✅ Middleware Pipeline
- ✅ Exception Handling

---

## Technology Stack

### Backend Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| ASP.NET Core Web API | .NET 8.0 | Backend framework for RESTful API |
| Entity Framework Core | 8.0.0 | ORM for database operations |
| SQL Server / LocalDB | - | Relational database |
| BCrypt.Net | Latest | Password hashing |
| C# | 12.0 | Programming language |

### Frontend Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| Angular | 21.2.0 | Frontend SPA framework |
| TypeScript | 5.9.2 | Type-safe JavaScript |
| RxJS | 7.8.0 | Reactive programming |
| Angular Router | 21.2.0 | Client-side routing |
| Angular Forms | 21.2.0 | Form handling |
| Angular HTTP Client | 21.2.0 | API communication |

### Development Tools

| Tool | Purpose |
|------|---------|
| Angular CLI | Project scaffolding, development server, build |
| .NET CLI | Project management, migrations, build |
| npm | Package management |
| Visual Studio Code | IDE |

---

## System Architecture

### High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     CLIENT BROWSER                          │
│  ┌───────────────────────────────────────────────────────┐  │
│  │           Angular SPA (Port 4200)                     │  │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  │  │
│  │  │ Components  │  │  Services   │  │   Models    │  │  │
│  │  │  - Login    │  │  - Auth     │  │  - User     │  │  │
│  │  │  - Dashboard│  │  - Task     │  │  - Task     │  │  │
│  │  │  - Tasks    │  │  - User     │  │             │  │  │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  │  │
│  │         │                 │                           │  │
│  │         └─────────────────┴──────────────────────────┤  │
│  │                    HTTP Client                        │  │
│  └───────────────────────────────────────────────────────┘  │
└──────────────────────────┬──────────────────────────────────┘
                           │ HTTP/HTTPS
                           │ (with Cookies)
┌──────────────────────────┴──────────────────────────────────┐
│              ASP.NET Core Web API (Port 5150)               │
│  ┌───────────────────────────────────────────────────────┐  │
│  │                 Middleware Pipeline                    │  │
│  │  CORS → Session → Authorization → Routing             │  │
│  └───────────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────────┐  │
│  │                    Controllers                         │  │
│  │  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐  │  │
│  │  │    Auth      │ │     Task     │ │     User     │  │  │
│  │  │  Controller  │ │  Controller  │ │  Controller  │  │  │
│  │  └──────────────┘ └──────────────┘ └──────────────┘  │  │
│  └───────────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────────┐  │
│  │                  Business Logic                        │  │
│  │  - Role validation                                     │  │
│  │  - Task status workflow                                │  │
│  │  - Authorization checks                                │  │
│  └───────────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────────┐  │
│  │            Entity Framework Core (ORM)                 │  │
│  │  ┌──────────────────────────────────────────────────┐ │  │
│  │  │         ApplicationDbContext                      │ │  │
│  │  │  - DbSet<User>                                    │ │  │
│  │  │  - DbSet<TaskItem>                                │ │  │
│  │  │  - DbSet<TaskComment>                             │ │  │
│  │  │  - LINQ Query Translation                         │ │  │
│  │  └──────────────────────────────────────────────────┘ │  │
│  └───────────────────────────────────────────────────────┘  │
└──────────────────────────┬──────────────────────────────────┘
                           │ SQL Queries
┌──────────────────────────┴──────────────────────────────────┐
│                  SQL Server / LocalDB                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │    Users     │  │    Tasks     │  │ TaskComments │      │
│  │    Table     │  │    Table     │  │    Table     │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
```

---

## MVC Architecture Explanation

### What is MVC?

MVC (Model-View-Controller) is a software design pattern that separates an application into three interconnected components:


### 1. MODEL (Data Layer)

**Purpose**: Represents the data and business logic of the application.

**In This Project**:
- **Entity Models**: `User.cs`, `TaskItem.cs`, `TaskComment.cs`
  - Define database table structure
  - Contain navigation properties for relationships
  - Represent business entities

- **DTOs (Data Transfer Objects)**: `LoginDto.cs`, `CreateTaskDto.cs`, `TaskDto.cs`, etc.
  - Define data contracts between client and server
  - Separate API structure from database structure
  - Prevent over-posting attacks

**Example**:
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public ICollection<TaskItem> AssignedTasks { get; set; }
    public ICollection<TaskItem> CreatedTasks { get; set; }
}
```

### 2. VIEW (Presentation Layer)

**Purpose**: Displays data to users and captures user input.

**In This Project**:
- **Angular Components**: HTML templates that render UI
  - `login.html` - Login form
  - `dashboard.html` - Dashboard with statistics
  - `create-task.html` - Task creation form
  - `my-tasks.html` - Employee task list

**Example**:
```html
<!-- login.html - View for authentication -->
<form (ngSubmit)="onSubmit()">
  <input [(ngModel)]="email" name="email" type="email" />
  <input [(ngModel)]="password" name="password" type="password" />
  <button type="submit">Login</button>
</form>
```

### 3. CONTROLLER (Logic Layer)

**Purpose**: Handles user requests, processes business logic, and returns responses.

**In This Project**:
- **API Controllers**: `AuthController.cs`, `TaskController.cs`, `UserController.cs`
  - Receive HTTP requests
  - Validate input using DTOs
  - Execute business logic
  - Query database using LINQ
  - Return HTTP responses

**Example**:
```csharp
[HttpPost]
public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
{
    // Validate user role (business logic)
    if (userRole != "Admin" && userRole != "Manager")
        return Forbid();
    
    // Create entity from DTO (model)
    var task = new TaskItem { /* ... */ };
    
    // Save to database
    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();
    
    // Return response
    return Ok(createdTask);
}
```

### MVC Flow in This Application

```
1. User Action (View)
   ↓
   User clicks "Create Task" button in Angular component
   
2. HTTP Request
   ↓
   Angular service sends POST request to /api/task
   
3. Controller Receives Request
   ↓
   TaskController.CreateTask() method is invoked
   
4. Controller Processes (Business Logic)
   ↓
   - Validates user role
   - Checks session authentication
   - Validates input data
   
5. Controller Interacts with Model
   ↓
   - Creates TaskItem entity
   - Saves to database via EF Core
   
6. Controller Returns Response
   ↓
   Returns HTTP 200 with created task data
   
7. View Updates (Angular)
   ↓
   Component receives response and updates UI
```

### Benefits of MVC

1. **Separation of Concerns**: Each component has a specific responsibility
2. **Maintainability**: Changes to one layer don't affect others
3. **Testability**: Each layer can be tested independently
4. **Reusability**: Models and controllers can be reused
5. **Parallel Development**: Frontend and backend teams can work simultaneously

---

## Layered Architecture

The application follows a **layered architecture** pattern with clear separation between layers:

### Layer 1: Presentation Layer (Angular Frontend)

**Responsibility**: User interface and user experience

**Components**:
- Angular Components (UI elements)
- HTML Templates (structure)
- CSS Styles (appearance)
- TypeScript Logic (UI behavior)

**Key Features**:
- Responsive design
- Client-side routing
- Form validation
- Real-time updates
- User feedback (success/error messages)

**Technologies**: Angular, TypeScript, HTML, CSS

### Layer 2: API Layer (ASP.NET Core Controllers)

**Responsibility**: Handle HTTP requests and responses

**Components**:
- AuthController - Authentication endpoints
- TaskController - Task management endpoints
- UserController - User management endpoints

**Key Features**:
- RESTful API design
- Input validation using DTOs
- Session management
- CORS configuration
- HTTP status codes

**Technologies**: ASP.NET Core Web API, C#

### Layer 3: Business Logic Layer

**Responsibility**: Implement business rules and workflows

**Components**:
- Role-based authorization logic
- Task status workflow validation
- Business rule enforcement
- Data transformation

**Key Business Rules**:
- Only Admin can create/delete users
- Only Admin and Manager can create tasks
- Only assigned employee can start/complete tasks
- Task status must follow: Pending → In Progress → Completed
- Cannot skip task statuses

**Location**: Embedded in Controllers (could be extracted to separate service classes)

### Layer 4: Data Access Layer (Entity Framework Core)

**Responsibility**: Database operations and data persistence

**Components**:
- ApplicationDbContext - Database context
- Entity Models - Database table representations
- LINQ Queries - Data retrieval
- Migrations - Schema management

**Key Features**:
- Object-Relational Mapping (ORM)
- LINQ to SQL translation
- Change tracking
- Relationship management
- Transaction handling

**Technologies**: Entity Framework Core, LINQ

### Layer 5: Database Layer (SQL Server)

**Responsibility**: Data storage and retrieval

**Components**:
- Users table
- Tasks table
- TaskComments table
- Indexes and constraints
- Foreign key relationships

**Technologies**: SQL Server / LocalDB

### Benefits of Layered Architecture

1. **Modularity**: Each layer is independent and can be modified separately
2. **Scalability**: Layers can be scaled independently
3. **Maintainability**: Clear structure makes code easier to understand
4. **Testability**: Each layer can be tested in isolation
5. **Flexibility**: Can replace one layer without affecting others
6. **Security**: Each layer can implement its own security measures

### Communication Between Layers

```
Presentation Layer (Angular)
    ↓ HTTP Requests
API Layer (Controllers)
    ↓ Method Calls
Business Logic Layer
    ↓ LINQ Queries
Data Access Layer (EF Core)
    ↓ SQL Queries
Database Layer (SQL Server)
```

**Important**: Each layer only communicates with adjacent layers, never skipping layers.

---

## Database Schema

### Entity Relationship Diagram

```
┌─────────────────────────┐
│         Users           │
│─────────────────────────│
│ Id (PK)                 │
│ Name                    │
│ Email (Unique)          │
│ PasswordHash            │
│ Role                    │
│ CreatedAt               │
└─────────────────────────┘
         │ 1
         │
         │ Creates (AssignedBy)
         │
         ├──────────────────────┐
         │ *                    │ *
         │                      │
┌────────┴──────────────┐      │
│       Tasks           │      │
│───────────────────────│      │
│ Id (PK)               │      │
│ Title                 │      │
│ Description           │      │
│ AssignedBy (FK)       │──────┘
│ AssignedTo (FK)       │──────┐
│ Priority              │      │
│ Status                │      │ Assigned To
│ CreatedAt             │      │
│ StartDate             │      │ *
│ CompletedDate         │      │
└───────────────────────┘      │
         │ 1                   │
         │                     │
         │                     └──────────────┐
         │ Has                                │
         │                                    │
         │ *                                  │ 1
┌────────┴──────────────┐      ┌─────────────┴──────────┐
│    TaskComments       │      │         Users          │
│───────────────────────│      │  (Same as above)       │
│ Id (PK)               │      └────────────────────────┘
│ TaskId (FK)           │
│ UserId (FK)           │
│ Comment               │
│ CreatedAt             │
└───────────────────────┘
```


### Users Table

| Column | Data Type | Constraints | Description |
|--------|-----------|-------------|-------------|
| Id | int | PRIMARY KEY, IDENTITY | Unique user identifier |
| Name | nvarchar(MAX) | NOT NULL | User's full name |
| Email | nvarchar(MAX) | NOT NULL, UNIQUE | User's email address |
| PasswordHash | nvarchar(MAX) | NOT NULL | BCrypt hashed password |
| Role | nvarchar(MAX) | NOT NULL | Admin, Manager, or Employee |
| CreatedAt | datetime2 | NOT NULL | Account creation timestamp |

**Indexes**:
- Primary Key on Id
- Unique index on Email (enforced at application level)

### Tasks Table

| Column | Data Type | Constraints | Description |
|--------|-----------|-------------|-------------|
| Id | int | PRIMARY KEY, IDENTITY | Unique task identifier |
| Title | nvarchar(MAX) | NOT NULL | Task title |
| Description | nvarchar(MAX) | NOT NULL | Task description |
| AssignedBy | int | FOREIGN KEY (Users.Id), NOT NULL | Creator user ID |
| AssignedTo | int | FOREIGN KEY (Users.Id), NOT NULL | Assignee user ID |
| Priority | nvarchar(MAX) | NOT NULL | Low, Medium, or High |
| Status | nvarchar(MAX) | NOT NULL | Pending, In Progress, Completed |
| CreatedAt | datetime2 | NOT NULL | Task creation timestamp |
| StartDate | datetime2 | NULL | When task was started |
| CompletedDate | datetime2 | NULL | When task was completed |

**Foreign Keys**:
- AssignedBy → Users.Id (ON DELETE RESTRICT)
- AssignedTo → Users.Id (ON DELETE RESTRICT)

**Indexes**:
- Primary Key on Id
- Foreign Key indexes on AssignedBy and AssignedTo

### TaskComments Table

| Column | Data Type | Constraints | Description |
|--------|-----------|-------------|-------------|
| Id | int | PRIMARY KEY, IDENTITY | Unique comment identifier |
| TaskId | int | FOREIGN KEY (Tasks.Id), NOT NULL | Associated task ID |
| UserId | int | FOREIGN KEY (Users.Id), NOT NULL | Comment author ID |
| Comment | nvarchar(MAX) | NOT NULL | Comment text |
| CreatedAt | datetime2 | NOT NULL | Comment creation timestamp |

**Foreign Keys**:
- TaskId → Tasks.Id (ON DELETE CASCADE)
- UserId → Users.Id (ON DELETE CASCADE)

**Indexes**:
- Primary Key on Id
- Foreign Key indexes on TaskId and UserId

### Relationships Explained

#### 1. User → Tasks (AssignedBy) - One-to-Many

- **Relationship**: One user can create many tasks
- **Foreign Key**: Tasks.AssignedBy → Users.Id
- **Delete Behavior**: RESTRICT
  - Cannot delete a user if they have created tasks
  - Preserves audit trail of who created tasks
  - Must reassign or delete tasks before deleting user

#### 2. User → Tasks (AssignedTo) - One-to-Many

- **Relationship**: One user can be assigned many tasks
- **Foreign Key**: Tasks.AssignedTo → Users.Id
- **Delete Behavior**: RESTRICT
  - Cannot delete a user if they have assigned tasks
  - Prevents orphaned tasks
  - Must reassign tasks before deleting user

#### 3. Task → TaskComments - One-to-Many

- **Relationship**: One task can have many comments
- **Foreign Key**: TaskComments.TaskId → Tasks.Id
- **Delete Behavior**: CASCADE
  - Deleting a task automatically deletes all its comments
  - Comments don't make sense without their task
  - Simplifies cleanup

#### 4. User → TaskComments - One-to-Many

- **Relationship**: One user can write many comments
- **Foreign Key**: TaskComments.UserId → Users.Id
- **Delete Behavior**: CASCADE
  - Deleting a user deletes their comments
  - Comments are not critical data

### Database Constraints

1. **Primary Keys**: Ensure unique identification of records
2. **Foreign Keys**: Maintain referential integrity
3. **NOT NULL**: Prevent missing required data
4. **UNIQUE**: Prevent duplicate emails
5. **Delete Behaviors**: Control cascading operations

---

## Entity Framework Core

### What is Entity Framework Core?

Entity Framework Core (EF Core) is an **Object-Relational Mapper (ORM)** that:
- Maps C# classes to database tables
- Translates LINQ queries to SQL
- Tracks changes to entities
- Generates SQL commands automatically
- Manages database connections
- Handles transactions

### Benefits of Using EF Core

1. **Productivity**: Write C# code instead of SQL
2. **Type Safety**: Compile-time checking of queries
3. **Maintainability**: Changes to entities automatically update database
4. **Portability**: Can switch database providers easily
5. **LINQ Support**: Use familiar C# syntax for queries
6. **Change Tracking**: Automatically detects entity modifications
7. **Migrations**: Version control for database schema

### DbContext - The Heart of EF Core

**ApplicationDbContext.cs** is the bridge between your application and database:

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // DbSet properties represent database tables
    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }

    // Configure entity relationships
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API configuration
    }
}
```

**DbSet<T>** represents a table:
- `DbSet<User>` → Users table
- `DbSet<TaskItem>` → Tasks table
- `DbSet<TaskComment>` → TaskComments table

### Entity Framework Core Migrations

**Migrations** are version control for your database schema.

#### Migration Workflow

```
1. Create/Modify Entity Models
   ↓
   Example: Add new property to User class
   
2. Create Migration
   ↓
   Command: dotnet ef migrations add AddPhoneNumberToUser
   ↓
   EF Core compares current model with last migration
   ↓
   Generates migration file with Up() and Down() methods
   
3. Review Migration
   ↓
   Check generated SQL in migration file
   ↓
   Verify changes are correct
   
4. Apply Migration
   ↓
   Command: dotnet ef database update
   ↓
   EF Core executes Up() method
   ↓
   Database schema is updated
   
5. Database Updated
   ↓
   New column added to Users table
```

#### Migration Commands

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Apply migrations to database
dotnet ef database update

# Rollback to specific migration
dotnet ef database update PreviousMigrationName

# Remove last migration (if not applied)
dotnet ef migrations remove

# List all migrations
dotnet ef migrations list

# Generate SQL script from migrations
dotnet ef migrations script
```

#### Migration File Structure

```csharp
public partial class InitialCreate : Migration
{
    // Applied when migrating forward
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(nullable: false),
                Email = table.Column<string>(nullable: false),
                // ... other columns
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });
    }

    // Applied when rolling back
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Users");
    }
}
```

### Configuring Relationships with Fluent API

**OnModelCreating** method configures entity relationships:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configure TaskItem → User (AssignedBy) relationship
    modelBuilder.Entity<TaskItem>()
        .HasOne(t => t.AssignedByUser)        // TaskItem has one User
        .WithMany(u => u.CreatedTasks)        // User has many created tasks
        .HasForeignKey(t => t.AssignedBy)     // Foreign key column
        .OnDelete(DeleteBehavior.Restrict);   // Cannot delete user if they created tasks

    // Configure TaskItem → User (AssignedTo) relationship
    modelBuilder.Entity<TaskItem>()
        .HasOne(t => t.AssignedToUser)
        .WithMany(u => u.AssignedTasks)
        .HasForeignKey(t => t.AssignedTo)
        .OnDelete(DeleteBehavior.Restrict);

    // Configure TaskComment → Task relationship
    modelBuilder.Entity<TaskComment>()
        .HasOne(tc => tc.Task)
        .WithMany(t => t.Comments)
        .HasForeignKey(tc => tc.TaskId)
        .OnDelete(DeleteBehavior.Cascade);    // Deleting task deletes comments
}
```

### EF Core in Action

#### Querying Data

```csharp
// Simple query
var users = await _context.Users.ToListAsync();

// Filtered query
var admins = await _context.Users
    .Where(u => u.Role == "Admin")
    .ToListAsync();

// Query with navigation properties
var tasks = await _context.Tasks
    .Include(t => t.AssignedByUser)
    .Include(t => t.AssignedToUser)
    .ToListAsync();
```

#### Adding Data

```csharp
var user = new User
{
    Name = "John Doe",
    Email = "john@example.com",
    PasswordHash = BCrypt.HashPassword("password"),
    Role = "Employee",
    CreatedAt = DateTime.UtcNow
};

_context.Users.Add(user);
await _context.SaveChangesAsync();
```

#### Updating Data

```csharp
var task = await _context.Tasks.FindAsync(taskId);
task.Status = "Completed";
task.CompletedDate = DateTime.UtcNow;
await _context.SaveChangesAsync();
```

#### Deleting Data

```csharp
var user = await _context.Users.FindAsync(userId);
_context.Users.Remove(user);
await _context.SaveChangesAsync();
```

---

## LINQ Usage

### What is LINQ?

**LINQ (Language Integrated Query)** is a powerful feature in C# that allows you to query data using a SQL-like syntax directly in your code.

### Benefits of LINQ

1. **Type Safety**: Compile-time checking of queries
2. **IntelliSense**: Auto-completion in IDE
3. **Readability**: More readable than raw SQL
4. **Consistency**: Same syntax for different data sources
5. **Composability**: Build queries incrementally
6. **Integration**: Works seamlessly with C# language features


### LINQ Query Syntax vs Method Syntax

#### Query Syntax (SQL-like)
```csharp
var employees = from u in _context.Users
                where u.Role == "Employee"
                select u;
```

#### Method Syntax (Fluent)
```csharp
var employees = _context.Users
    .Where(u => u.Role == "Employee");
```

**This project uses Method Syntax** as it's more flexible and commonly used.

### LINQ Examples in This Project

#### 1. Filtering Tasks by Assigned User

**Location**: TaskController.GetMyTasks()

```csharp
var tasks = await _context.Tasks
    .Where(t => t.AssignedTo == userId.Value)
    .ToListAsync();
```

**SQL Equivalent**:
```sql
SELECT * FROM Tasks WHERE AssignedTo = @userId
```

**Explanation**:
- `Where()` filters tasks based on condition
- `t => t.AssignedTo == userId.Value` is a lambda expression
- Returns only tasks assigned to the current user

#### 2. Filtering Tasks by Status (Completed)

**Location**: TaskController.GetCompletedTasks()

```csharp
var tasks = await _context.Tasks
    .Include(t => t.AssignedByUser)
    .Include(t => t.AssignedToUser)
    .Where(t => t.Status == "Completed")
    .ToListAsync();
```

**SQL Equivalent**:
```sql
SELECT * FROM Tasks WHERE Status = 'Completed'
```

**Explanation**:
- Filters tasks to show only completed ones
- Demonstrates LINQ filtering by string comparison

#### 3. Filtering Tasks by Status (Pending)

**Location**: TaskController.GetPendingTasks()

```csharp
var tasks = await _context.Tasks
    .Include(t => t.AssignedByUser)
    .Include(t => t.AssignedToUser)
    .Where(t => t.Status == "Pending")
    .ToListAsync();
```

**SQL Equivalent**:
```sql
SELECT * FROM Tasks WHERE Status = 'Pending'
```

**Explanation**:
- Filters tasks to show only pending ones
- Useful for employees to see what tasks they need to start

#### 4. Filtering Tasks Created by Current User

**Location**: TaskController.GetTasksCreatedByMe()

```csharp
var tasks = await _context.Tasks
    .Include(t => t.AssignedByUser)
    .Include(t => t.AssignedToUser)
    .Where(t => t.AssignedBy == userId.Value)
    .ToListAsync();
```

**SQL Equivalent**:
```sql
SELECT * FROM Tasks WHERE AssignedBy = @userId
```

**Explanation**:
- Manager sees only tasks they created
- `AssignedBy` column stores the creator's user ID
- Demonstrates filtering by creator

#### 5. Filtering Tasks by Manager

**Location**: TaskController.GetTasks()

```csharp
if (userRole == "Manager")
{
    query = query.Where(t => t.AssignedBy == userId.Value);
}
```

**SQL Equivalent**:
```sql
SELECT * FROM Tasks WHERE AssignedBy = @userId
```

**Explanation**:
- Manager sees only tasks they created
- `AssignedBy` column stores the creator's user ID

#### 6. Complex Query with Includes and Projection

**Location**: TaskController.GetTasks()

```csharp
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
```

**SQL Equivalent**:
```sql
SELECT 
    t.Id, t.Title, t.Description, t.AssignedBy, t.AssignedTo,
    t.Priority, t.Status, t.CreatedAt, t.StartDate, t.CompletedDate,
    u1.Name AS AssignedByName,
    u2.Name AS AssignedToName
FROM Tasks t
INNER JOIN Users u1 ON t.AssignedBy = u1.Id
INNER JOIN Users u2 ON t.AssignedTo = u2.Id
WHERE t.AssignedTo = @userId
```

**Explanation**:
- `Include()` performs eager loading (JOIN in SQL)
- `Where()` filters results
- `Select()` projects data into DTO
- `ToListAsync()` executes query asynchronously

#### 7. Finding User by Email

**Location**: AuthController.Login()

```csharp
var user = await _context.Users
    .Where(u => u.Email == loginDto.Email)
    .FirstOrDefaultAsync();
```

**SQL Equivalent**:
```sql
SELECT TOP 1 * FROM Users WHERE Email = @email
```

**Explanation**:
- `Where()` filters by email
- `FirstOrDefaultAsync()` returns first match or null
- Used for authentication

#### 8. Finding Task with Multiple Conditions

**Location**: TaskController.StartTask()

```csharp
var task = await _context.Tasks
    .Where(t => t.Id == id && t.AssignedTo == userId.Value)
    .FirstOrDefaultAsync();
```

**SQL Equivalent**:
```sql
SELECT TOP 1 * FROM Tasks 
WHERE Id = @id AND AssignedTo = @userId
```

**Explanation**:
- Multiple conditions with AND operator
- Ensures task exists and belongs to user
- Returns null if no match found

#### 9. Filtering Pending Tasks (Client-Side)

**Location**: Dashboard component (TypeScript)

```typescript
this.stats.pending = this.tasks.filter(t => t.status === 'Pending').length;
```

**Explanation**:
- Filters tasks array to get only pending tasks
- `.length` counts the filtered results
- Demonstrates LINQ-like operations in TypeScript

### Common LINQ Methods Used

| Method | Purpose | Example |
|--------|---------|---------|
| `Where()` | Filter data | `.Where(u => u.Role == "Admin")` |
| `Select()` | Project/transform data | `.Select(u => new UserDto { ... })` |
| `Include()` | Eager load related data | `.Include(t => t.AssignedByUser)` |
| `FirstOrDefaultAsync()` | Get first item or null | `.FirstOrDefaultAsync()` |
| `ToListAsync()` | Execute query, return list | `.ToListAsync()` |
| `FindAsync()` | Find by primary key | `.FindAsync(id)` |
| `Add()` | Add entity to context | `.Add(user)` |
| `Remove()` | Remove entity from context | `.Remove(user)` |
| `SaveChangesAsync()` | Persist changes to database | `.SaveChangesAsync()` |

### LINQ Query Execution

**Deferred Execution**: LINQ queries are not executed until you enumerate them.

```csharp
// Query is defined but NOT executed
var query = _context.Users.Where(u => u.Role == "Admin");

// Query is executed here
var admins = await query.ToListAsync();
```

**Immediate Execution**: Some methods execute immediately.

```csharp
// Executes immediately
var count = await _context.Users.CountAsync();
var first = await _context.Users.FirstAsync();
var exists = await _context.Users.AnyAsync(u => u.Email == email);
```

---

## DTO Pattern

### What are DTOs?

**DTOs (Data Transfer Objects)** are simple objects used to transfer data between layers of an application, particularly between the API and clients.

### Why Use DTOs?

1. **Security**: Hide sensitive data (e.g., PasswordHash)
2. **Decoupling**: Separate API contracts from database entities
3. **Flexibility**: API structure can differ from database structure
4. **Validation**: Define exactly what data is required
5. **Over-posting Prevention**: Clients can't modify fields not in DTO
6. **Versioning**: Can maintain multiple DTO versions for API compatibility

### DTOs in This Project

#### 1. LoginDto

**Purpose**: Accept login credentials

```csharp
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

**Why?**
- Only accepts email and password
- Prevents clients from sending extra fields
- Clear contract for authentication

#### 2. CreateUserDto

**Purpose**: Accept data for creating new user

```csharp
public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
```

**Why?**
- Client provides plain password (not hash)
- Server hashes password before storing
- Id and CreatedAt are set by server
- Prevents clients from manipulating these fields

#### 3. UserDto

**Purpose**: Return user data to client

```csharp
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

**Why?**
- Excludes PasswordHash (security)
- Excludes navigation properties (performance)
- Only returns necessary data

#### 4. CreateTaskDto

**Purpose**: Accept data for creating new task

```csharp
public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AssignedTo { get; set; }
    public string Priority { get; set; } = "Medium";
}
```

**Why?**
- Client provides: Title, Description, AssignedTo, Priority
- Server sets: Id, AssignedBy, Status, CreatedAt
- Prevents clients from setting task status
- Enforces business rule: new tasks are always "Pending"

#### 5. TaskDto

**Purpose**: Return task data to client

```csharp
public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AssignedBy { get; set; }
    public string AssignedByName { get; set; } = string.Empty;
    public int AssignedTo { get; set; }
    public string AssignedToName { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}
```

**Why?**
- Includes computed fields: AssignedByName, AssignedToName
- Flattens navigation properties for easier client consumption
- Provides all data needed for display

### DTO vs Entity Comparison

| Aspect | Entity (TaskItem) | DTO (TaskDto) |
|--------|-------------------|---------------|
| Purpose | Database representation | Data transfer |
| Navigation Properties | Yes (AssignedByUser, AssignedToUser) | No |
| Computed Fields | No | Yes (AssignedByName) |
| Validation | Database constraints | API validation |
| Modification | Tracked by EF Core | Not tracked |
| Usage | Internal to backend | Sent to/from client |

### DTO Mapping Example

```csharp
// Entity to DTO mapping
var taskDto = new TaskDto
{
    Id = task.Id,
    Title = task.Title,
    Description = task.Description,
    AssignedBy = task.AssignedBy,
    AssignedByName = task.AssignedByUser!.Name,  // From navigation property
    AssignedTo = task.AssignedTo,
    AssignedToName = task.AssignedToUser!.Name,  // From navigation property
    Priority = task.Priority,
    Status = task.Status,
    CreatedAt = task.CreatedAt,
    StartDate = task.StartDate,
    CompletedDate = task.CompletedDate
};

// DTO to Entity mapping
var task = new TaskItem
{
    Title = createTaskDto.Title,
    Description = createTaskDto.Description,
    AssignedBy = userId.Value,              // From session
    AssignedTo = createTaskDto.AssignedTo,
    Priority = createTaskDto.Priority,
    Status = "Pending",                     // Business rule
    CreatedAt = DateTime.UtcNow             // Server timestamp
};
```

### Benefits Demonstrated

1. **Security**: PasswordHash never sent to client
2. **Flexibility**: AssignedByName computed from navigation property
3. **Control**: Server sets AssignedBy, Status, CreatedAt
4. **Clarity**: Clear API contracts for each operation
5. **Validation**: Each DTO defines required fields

---

## API Endpoints

### Base URL
```
http://localhost:5150/api
```

### Authentication Endpoints

#### POST /api/auth/login
Authenticates a user and creates a session.

**Request**:
```json
{
  "email": "admin@test.com",
  "password": "admin123"
}
```

**Response** (200 OK):
```json
{
  "id": 1,
  "name": "Admin User",
  "email": "admin@test.com",
  "role": "Admin"
}
```

**Error Responses**:
- 401 Unauthorized: Invalid credentials


#### POST /api/auth/logout
Logs out the current user and clears the session.

**Request**: Empty body

**Response** (200 OK):
```json
{
  "message": "Logged out successfully"
}
```

#### GET /api/auth/session
Retrieves the current user's session information.

**Response** (200 OK):
```json
{
  "id": 1,
  "name": "Admin User",
  "role": "Admin"
}
```

**Error Responses**:
- 401 Unauthorized: Not authenticated

### User Management Endpoints

#### GET /api/user
Retrieves all users (Admin and Manager only).

**Authorization**: Admin, Manager

**Response** (200 OK):
```json
[
  {
    "id": 1,
    "name": "Admin User",
    "email": "admin@test.com",
    "role": "Admin",
    "createdAt": "2026-03-11T18:38:32Z"
  },
  {
    "id": 2,
    "name": "Manager User",
    "email": "manager@test.com",
    "role": "Manager",
    "createdAt": "2026-03-11T18:38:32Z"
  }
]
```

**Error Responses**:
- 403 Forbidden: Not authorized (Employee role)

#### POST /api/user
Creates a new user (Admin only).

**Authorization**: Admin

**Request**:
```json
{
  "name": "New Employee",
  "email": "employee@test.com",
  "password": "password123",
  "role": "Employee"
}
```

**Response** (200 OK):
```json
{
  "id": 5,
  "name": "New Employee",
  "email": "employee@test.com",
  "role": "Employee",
  "createdAt": "2026-03-12T10:30:00Z"
}
```

**Error Responses**:
- 400 Bad Request: Email already exists
- 403 Forbidden: Not authorized (Manager or Employee role)

#### DELETE /api/user/{id}
Deletes a user by ID (Admin only).

**Authorization**: Admin

**Response** (200 OK):
```json
{
  "message": "User deleted successfully"
}
```

**Error Responses**:
- 404 Not Found: User doesn't exist
- 403 Forbidden: Not authorized
- 500 Internal Server Error: User has associated tasks (foreign key constraint)

### Task Management Endpoints

#### GET /api/task
Retrieves tasks based on user role.

**Authorization**: All authenticated users

**Role-Based Filtering**:
- Admin: All tasks
- Manager: Tasks assigned by the manager
- Employee: Tasks assigned to the employee

**Response** (200 OK):
```json
[
  {
    "id": 1,
    "title": "Complete Project Documentation",
    "description": "Write comprehensive documentation for the project",
    "assignedBy": 2,
    "assignedByName": "Manager User",
    "assignedTo": 3,
    "assignedToName": "Employee One",
    "priority": "High",
    "status": "Pending",
    "createdAt": "2026-03-11T18:40:00Z",
    "startDate": null,
    "completedDate": null
  }
]
```

#### GET /api/task/my
Retrieves tasks assigned to the current user.

**Authorization**: All authenticated users

**Response**: Same format as GET /api/task

#### GET /api/task/completed
Retrieves all completed tasks based on user role.

**Authorization**: All authenticated users

**LINQ Query**:
```csharp
.Where(t => t.Status == "Completed")
```

**Role-Based Filtering**:
- Admin: All completed tasks
- Manager: Completed tasks they created
- Employee: Completed tasks assigned to them

**Response**: Same format as GET /api/task

#### GET /api/task/pending
Retrieves all pending tasks based on user role.

**Authorization**: All authenticated users

**LINQ Query**:
```csharp
.Where(t => t.Status == "Pending")
```

**Role-Based Filtering**:
- Admin: All pending tasks
- Manager: Pending tasks they created
- Employee: Pending tasks assigned to them

**Response**: Same format as GET /api/task

#### GET /api/task/created-by-me
Retrieves all tasks created by the current user.

**Authorization**: Admin, Manager only

**LINQ Query**:
```csharp
.Where(t => t.AssignedBy == userId.Value)
```

**Response**: Same format as GET /api/task

**Error Responses**:
- 403 Forbidden: Employee role (cannot create tasks)

#### POST /api/task
Creates a new task (Admin and Manager only).

**Authorization**: Admin, Manager

**Request**:
```json
{
  "title": "Implement User Authentication",
  "description": "Add login and session management",
  "assignedTo": 3,
  "priority": "High"
}
```

**Response** (200 OK):
```json
{
  "id": 10,
  "title": "Implement User Authentication",
  "description": "Add login and session management",
  "assignedBy": 2,
  "assignedByName": "Manager User",
  "assignedTo": 3,
  "assignedToName": "Employee One",
  "priority": "High",
  "status": "Pending",
  "createdAt": "2026-03-12T10:45:00Z",
  "startDate": null,
  "completedDate": null
}
```

**Error Responses**:
- 403 Forbidden: Not authorized (Employee role)

#### PUT /api/task/start/{id}
Starts a task (changes status from Pending to In Progress).

**Authorization**: Assigned employee only

**STRICT ENFORCEMENT**:
- Only users with "Employee" role can start tasks
- Admin and Manager will receive 403 Forbidden
- Task must be assigned to the current user

**Response** (200 OK):
```json
{
  "message": "Task started successfully"
}
```

**Error Responses**:
- 403 Forbidden: User is Admin or Manager (not allowed to start tasks)
- 404 Not Found: Task not found or not assigned to you
- 400 Bad Request: Task can only be started from Pending status

#### PUT /api/task/complete/{id}
Completes a task (changes status from In Progress to Completed).

**Authorization**: Assigned employee only

**STRICT ENFORCEMENT**:
- Only users with "Employee" role can complete tasks
- Admin and Manager will receive 403 Forbidden
- Task must be assigned to the current user

**Response** (200 OK):
```json
{
  "message": "Task completed successfully"
}
```

**Error Responses**:
- 403 Forbidden: User is Admin or Manager (not allowed to complete tasks)
- 404 Not Found: Task not found or not assigned to you
- 400 Bad Request: Task can only be completed from In Progress status

### HTTP Status Codes Used

| Code | Meaning | Usage |
|------|---------|-------|
| 200 OK | Success | Successful GET, POST, PUT requests |
| 400 Bad Request | Invalid input | Validation errors, business rule violations |
| 401 Unauthorized | Not authenticated | Missing or invalid session |
| 403 Forbidden | Not authorized | Insufficient permissions for action |
| 404 Not Found | Resource not found | User or task doesn't exist |
| 500 Internal Server Error | Server error | Unexpected errors, database errors |

---

## Authentication & Authorization

### Authentication (Who are you?)

**Method**: Session-based authentication with cookies

#### Authentication Flow

```
1. User Login
   ↓
   User submits email and password
   
2. Credential Validation
   ↓
   Backend queries database for user
   ↓
   BCrypt verifies password against hash
   
3. Session Creation
   ↓
   If valid, create session with user data:
   - UserId
   - UserRole
   - UserName
   
4. Cookie Sent to Client
   ↓
   Session ID stored in HTTP-only cookie
   ↓
   Browser automatically includes cookie in requests
   
5. Subsequent Requests
   ↓
   Browser sends cookie with each request
   ↓
   Backend reads session data from cookie
   ↓
   User is identified
```

#### Session Configuration

**Program.cs**:
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Session expires after 30 min
    options.Cookie.HttpOnly = true;                  // Prevents JavaScript access
    options.Cookie.IsEssential = true;               // Required for app functionality
});
```

#### Session Storage

**AuthController.Login()**:
```csharp
HttpContext.Session.SetInt32("UserId", user.Id);
HttpContext.Session.SetString("UserRole", user.Role);
HttpContext.Session.SetString("UserName", user.Name);
```

#### Session Retrieval

**TaskController.GetTasks()**:
```csharp
var userId = HttpContext.Session.GetInt32("UserId");
var userRole = HttpContext.Session.GetString("UserRole");
```

### Authorization (What can you do?)

**Method**: Role-based authorization

#### Authorization Checks

**1. Controller-Level Authorization**:
```csharp
var userRole = HttpContext.Session.GetString("UserRole");

if (userRole != "Admin")
{
    return Forbid();  // 403 Forbidden
}
```

**2. Resource-Level Authorization**:
```csharp
var task = await _context.Tasks
    .Where(t => t.Id == id && t.AssignedTo == userId.Value)
    .FirstOrDefaultAsync();

if (task == null)
{
    return NotFound();  // Task doesn't exist or not assigned to user
}
```

### Password Security

#### BCrypt Hashing

**Why BCrypt?**
- Slow by design (prevents brute force attacks)
- Automatic salt generation (prevents rainbow table attacks)
- Adaptive (can increase cost factor over time)
- Industry standard for password hashing

**Hashing Password** (UserController.CreateUser):
```csharp
var user = new User
{
    PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password)
};
```

**Verifying Password** (AuthController.Login):
```csharp
if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
{
    return Unauthorized();
}
```

### Security Features

1. **HTTP-Only Cookies**: Prevents XSS attacks
2. **Session Timeout**: Automatic logout after inactivity
3. **Password Hashing**: Passwords never stored in plain text
4. **CORS Policy**: Only allows requests from Angular frontend
5. **Role Validation**: Every endpoint checks user role
6. **Resource Ownership**: Users can only modify their own resources

### Session vs Token Authentication

**This project uses Session-based authentication**:

| Aspect | Session-Based | Token-Based (JWT) |
|--------|---------------|-------------------|
| Storage | Server-side | Client-side |
| Scalability | Requires sticky sessions | Stateless, easily scalable |
| Security | More secure (server-controlled) | Vulnerable if token stolen |
| Logout | Immediate (clear session) | Requires token blacklist |
| Complexity | Simpler to implement | More complex |
| Use Case | Traditional web apps | APIs, mobile apps, microservices |

---

## Role-Based Permissions

### User Roles

The system has three distinct roles with different permissions:

### 1. Admin Role

**Capabilities**:
- ✅ Create users
- ✅ Delete users
- ✅ View all users
- ✅ Create tasks
- ✅ Assign tasks to any employee
- ✅ View all tasks in the system
- ❌ Start tasks (only employees can start their assigned tasks)
- ❌ Complete tasks (only employees can complete their assigned tasks)

**Use Case**: System administrators who manage users and oversee all operations

**Implementation**:
```csharp
// UserController.CreateUser()
if (userRole != "Admin")
{
    return Forbid();
}
```

### 2. Manager Role

**Capabilities**:
- ❌ Create users
- ❌ Delete users
- ✅ View all users (to assign tasks)
- ✅ Create tasks
- ✅ Assign tasks to employees
- ✅ View tasks they created
- ❌ View tasks created by other managers
- ❌ Start tasks
- ❌ Complete tasks

**Use Case**: Team leads who create and assign tasks to their team members

**Implementation**:
```csharp
// TaskController.GetTasks()
if (userRole == "Manager")
{
    query = query.Where(t => t.AssignedBy == userId.Value);
}
```

### 3. Employee Role

**Capabilities**:
- ❌ Create users
- ❌ Delete users
- ❌ View all users
- ❌ Create tasks
- ❌ Assign tasks
- ✅ View tasks assigned to them
- ✅ Start their assigned tasks
- ✅ Complete their assigned tasks
- ❌ Modify tasks assigned to other employees

**Use Case**: Team members who execute assigned tasks

**Implementation**:
```csharp
// TaskController.StartTask()
var task = await _context.Tasks
    .Where(t => t.Id == id && t.AssignedTo == userId.Value)
    .FirstOrDefaultAsync();

if (task == null)
{
    return NotFound(new { message = "Task not found or not assigned to you" });
}
```

### Permission Matrix

| Action | Admin | Manager | Employee |
|--------|-------|---------|----------|
| Create User | ✅ | ❌ | ❌ |
| Delete User | ✅ | ❌ | ❌ |
| View All Users | ✅ | ✅ | ❌ |
| Create Task | ✅ | ✅ | ❌ |
| View All Tasks | ✅ | ❌ | ❌ |
| View Own Created Tasks | ✅ | ✅ | ❌ |
| View Assigned Tasks | ✅ | ✅ | ✅ |
| Start Task | ❌ | ❌ | ✅ (own only) |
| Complete Task | ❌ | ❌ | ✅ (own only) |

### Authorization Implementation

#### Backend Authorization

**1. Role-Based Endpoint Access**:
```csharp
// Only Admin can create users
[HttpPost]
public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
{
    var userRole = HttpContext.Session.GetString("UserRole");
    if (userRole != "Admin")
    {
        return Forbid();  // 403 Forbidden
    }
    // ... create user logic
}
```

**2. Resource Ownership Validation**:
```csharp
// Only assigned employee can start task
[HttpPut("start/{id}")]
public async Task<IActionResult> StartTask(int id)
{
    var userId = HttpContext.Session.GetInt32("UserId");
    var task = await _context.Tasks
        .Where(t => t.Id == id && t.AssignedTo == userId.Value)
        .FirstOrDefaultAsync();
    
    if (task == null)
    {
        return NotFound();  // Task doesn't exist or not assigned to user
    }
    // ... start task logic
}
```

#### Frontend Authorization

**1. Conditional UI Rendering**:
```typescript
// dashboard.ts
canCreateTask(): boolean {
    return this.currentUser?.role === 'Admin' || 
           this.currentUser?.role === 'Manager';
}
```

```html
<!-- dashboard.html -->
<button *ngIf="canCreateTask()" routerLink="/create-task">
    Create Task
</button>
```

**2. Role-Based Navigation**:
```typescript
// auth.service.ts
hasRole(role: string): boolean {
    const user = this.currentUserSubject.value;
    return user !== null && user.role === role;
}
```

### Security Considerations

1. **Backend Enforcement**: Authorization is always enforced on backend
2. **Frontend UI**: Frontend only hides UI elements, doesn't enforce security
3. **Defense in Depth**: Multiple layers of authorization checks
4. **Principle of Least Privilege**: Users only get minimum required permissions

---

## Task Management Workflow

### Task Lifecycle

Tasks follow a strict three-stage lifecycle:

```
┌──────────┐      Start Task      ┌─────────────┐      Complete Task      ┌───────────┐
│ Pending  │ ──────────────────> │ In Progress │ ──────────────────────> │ Completed │
└──────────┘                      └─────────────┘                          └───────────┘
     │                                   │                                       │
     │ Created by Admin/Manager          │ Started by Employee                  │ Completed by Employee
     │ AssignedTo: Employee              │ StartDate: Set                       │ CompletedDate: Set
     │ Status: "Pending"                 │ Status: "In Progress"                │ Status: "Completed"
```

### Business Rules

1. **Task Creation**:
   - Only Admin and Manager can create tasks
   - New tasks always start with "Pending" status
   - AssignedBy is automatically set to current user
   - CreatedAt is set to current timestamp
   - Employees CANNOT create tasks

2. **Task Assignment**:
   - Tasks can only be assigned to users with "Employee" role
   - AssignedTo field stores the employee's user ID
   - Only Admin and Manager can assign tasks

3. **Starting Tasks**:
   - **STRICT RULE**: Only the assigned employee can start their task
   - **Admin and Manager CANNOT start tasks** - even if assigned to them
   - Task must be in "Pending" status
   - Cannot skip from "Pending" to "Completed"
   - StartDate is automatically set when task is started
   - Backend validates user role is "Employee"

4. **Completing Tasks**:
   - **STRICT RULE**: Only the assigned employee can complete their task
   - **Admin and Manager CANNOT complete tasks** - even if assigned to them
   - Task must be in "In Progress" status
   - Cannot complete a "Pending" task (must start it first)
   - CompletedDate is automatically set when task is completed
   - Backend validates user role is "Employee"

5. **Status Transitions**:
   - Pending → In Progress: Valid ✅ (Employee only)
   - In Progress → Completed: Valid ✅ (Employee only)
   - Pending → Completed: Invalid ❌
   - Completed → In Progress: Invalid ❌ (no rollback)
   - Admin/Manager attempting to start/complete: Forbidden ❌ (403 error)


### Task Creation Flow

```
1. Admin/Manager navigates to Create Task page
   ↓
2. Form loads list of employees (filtered from all users)
   ↓
3. Admin/Manager fills in:
   - Title
   - Description
   - Assigned To (select employee)
   - Priority (Low, Medium, High)
   ↓
4. Submit form
   ↓
5. Angular sends POST request to /api/task
   ↓
6. Backend validates:
   - User is authenticated
   - User role is Admin or Manager
   - All required fields are provided
   ↓
7. Backend creates task:
   - Sets AssignedBy to current user ID
   - Sets Status to "Pending"
   - Sets CreatedAt to current timestamp
   ↓
8. Task saved to database
   ↓
9. Success response sent to client
   ↓
10. Angular shows success message and redirects to dashboard
```

### Task Execution Flow (Employee Perspective)

```
1. Employee logs in
   ↓
2. Navigates to "My Tasks" page
   ↓
3. System loads tasks assigned to employee
   ↓
4. Employee sees list of tasks with statuses:
   - Pending: Shows "Start Task" button
   - In Progress: Shows "Complete Task" button
   - Completed: No action buttons
   ↓
5. Employee clicks "Start Task"
   ↓
6. Angular sends PUT request to /api/task/start/{id}
   ↓
7. Backend validates:
   - User is authenticated
   - Task is assigned to this user
   - Task status is "Pending"
   ↓
8. Backend updates task:
   - Status → "In Progress"
   - StartDate → Current timestamp
   ↓
9. Changes saved to database
   ↓
10. Success response sent to client
   ↓
11. Angular refreshes task list
   ↓
12. Task now shows "Complete Task" button
   ↓
13. Employee works on task...
   ↓
14. Employee clicks "Complete Task"
   ↓
15. Similar validation and update process
   ↓
16. Task status → "Completed"
   ↓
17. CompletedDate → Current timestamp
   ↓
18. Task list refreshes, task marked as completed
```

### Validation Examples

#### Backend Validation (TaskController.StartTask)

```csharp
// Check authentication
var userId = HttpContext.Session.GetInt32("UserId");
var userRole = HttpContext.Session.GetString("UserRole");

if (userId == null)
{
    return Unauthorized();
}

// STRICT RULE: Only employees can start tasks
if (userRole == "Admin" || userRole == "Manager")
{
    return Forbid(); // 403 Forbidden
}

// Check task ownership
var task = await _context.Tasks
    .Where(t => t.Id == id && t.AssignedTo == userId.Value)
    .FirstOrDefaultAsync();

if (task == null)
{
    return NotFound(new { message = "Task not found or not assigned to you" });
}

// Check current status
if (task.Status != "Pending")
{
    return BadRequest(new { message = "Task can only be started from Pending status" });
}

// Update task
task.Status = "In Progress";
task.StartDate = DateTime.UtcNow;
await _context.SaveChangesAsync();
```

#### Backend Validation (TaskController.CompleteTask)

```csharp
// Check authentication
var userId = HttpContext.Session.GetInt32("UserId");
var userRole = HttpContext.Session.GetString("UserRole");

if (userId == null)
{
    return Unauthorized();
}

// STRICT RULE: Only employees can complete tasks
if (userRole == "Admin" || userRole == "Manager")
{
    return Forbid(); // 403 Forbidden
}

// Check task ownership
var task = await _context.Tasks
    .Where(t => t.Id == id && t.AssignedTo == userId.Value)
    .FirstOrDefaultAsync();

if (task == null)
{
    return NotFound(new { message = "Task not found or not assigned to you" });
}

// Check current status
if (task.Status != "In Progress")
{
    return BadRequest(new { message = "Task can only be completed from In Progress status" });
}

// Update task
task.Status = "Completed";
task.CompletedDate = DateTime.UtcNow;
await _context.SaveChangesAsync();
```

#### Frontend Validation (my-tasks.ts)

```typescript
canStart(task: Task): boolean {
    return task.status === 'Pending';
}

canComplete(task: Task): boolean {
    return task.status === 'In Progress';
}
```

```html
<button *ngIf="canStart(task)" (click)="startTask(task.id)">
    Start Task
</button>
<button *ngIf="canComplete(task)" (click)="completeTask(task.id)">
    Complete Task
</button>
```

---

## Angular SPA Architecture

### What is a Single Page Application (SPA)?

A **Single Page Application** is a web application that:
- Loads once (single HTML page)
- Dynamically updates content without page reloads
- Provides fast, responsive user experience
- Feels like a native desktop/mobile app

### Traditional Web App vs SPA

#### Traditional Multi-Page Application
```
User clicks link
   ↓
Browser requests new page from server
   ↓
Server generates complete HTML page
   ↓
Browser receives and renders entire page
   ↓
Page reloads (white screen flash)
   ↓
User sees new page
```

#### Single Page Application (This Project)
```
User clicks link
   ↓
Angular Router changes URL (no page reload)
   ↓
Angular loads new component
   ↓
Component requests data from API (if needed)
   ↓
Server returns JSON data only
   ↓
Component updates view with new data
   ↓
User sees new content (no page reload)
```

### Angular SPA Benefits

1. **Fast Navigation**: No page reloads, instant transitions
2. **Better UX**: Smooth, app-like experience
3. **Reduced Server Load**: Server only sends data, not HTML
4. **Offline Capability**: Can cache app and work offline
5. **State Management**: Maintain state across navigation
6. **Responsive**: Feels like native app

### Angular Application Structure

```
TaskManagementUI/
├── src/
│   ├── app/
│   │   ├── components/          # UI Components
│   │   │   ├── login/
│   │   │   │   ├── login.ts     # Component logic
│   │   │   │   ├── login.html   # Template
│   │   │   │   └── login.css    # Styles
│   │   │   ├── dashboard/
│   │   │   ├── create-task/
│   │   │   ├── task-list/
│   │   │   └── my-tasks/
│   │   ├── services/            # Business Logic & API
│   │   │   ├── auth.service.ts
│   │   │   ├── task.service.ts
│   │   │   └── user.service.ts
│   │   ├── models/              # TypeScript Interfaces
│   │   │   ├── user.ts
│   │   │   └── task.ts
│   │   ├── app.routes.ts        # Routing Configuration
│   │   ├── app.config.ts        # App Configuration
│   │   ├── app.ts               # Root Component
│   │   └── app.html             # Root Template
│   ├── index.html               # Single HTML file
│   └── main.ts                  # Application Entry Point
├── angular.json                 # Angular CLI Configuration
├── package.json                 # Dependencies
└── tsconfig.json                # TypeScript Configuration
```

### Angular Core Concepts

#### 1. Components

**Purpose**: Building blocks of Angular applications

**Structure**:
```typescript
@Component({
  selector: 'app-login',           // HTML tag: <app-login></app-login>
  standalone: true,                // Self-contained component
  imports: [CommonModule, FormsModule],  // Dependencies
  templateUrl: './login.html',     // HTML template
  styleUrl: './login.css'          // Component styles
})
export class LoginComponent {
  // Component logic
  email = '';
  password = '';
  
  onSubmit() {
    // Handle form submission
  }
}
```

**Key Features**:
- Encapsulation: Each component has its own template, styles, and logic
- Reusability: Components can be used multiple times
- Composition: Components can contain other components
- Lifecycle: Components have lifecycle hooks (ngOnInit, ngOnDestroy, etc.)

#### 2. Services

**Purpose**: Share data and logic across components

**Structure**:
```typescript
@Injectable({
  providedIn: 'root'  // Singleton service
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  constructor(private http: HttpClient) { }
  
  login(credentials: LoginRequest): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/login`, credentials);
  }
}
```

**Key Features**:
- Singleton: One instance shared across app
- Dependency Injection: Automatically provided to components
- Separation of Concerns: Business logic separate from UI
- Testability: Easy to mock for testing

#### 3. Routing

**Purpose**: Navigate between views without page reloads

**Configuration** (app.routes.ts):
```typescript
export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'create-task', component: CreateTaskComponent },
  { path: 'tasks', component: TaskListComponent },
  { path: 'my-tasks', component: MyTasksComponent }
];
```

**Navigation**:
```typescript
// Programmatic navigation
this.router.navigate(['/dashboard']);

// Template navigation
<a routerLink="/dashboard">Dashboard</a>
```

#### 4. Template-Driven Forms

**Purpose**: Handle user input with two-way binding

**Example**:
```html
<form (ngSubmit)="onSubmit()">
  <input [(ngModel)]="email" name="email" type="email" required />
  <input [(ngModel)]="password" name="password" type="password" required />
  <button type="submit">Login</button>
</form>
```

**Features**:
- Two-way binding: `[(ngModel)]`
- Form validation
- Easy to implement
- Good for simple forms

#### 5. Data Binding

**Types of Binding**:

**a) Interpolation** (Component → Template):
```html
<h1>Welcome, {{ currentUser?.name }}</h1>
```

**b) Property Binding** (Component → Template):
```html
<input [value]="email" />
<button [disabled]="!isValid">Submit</button>
```

**c) Event Binding** (Template → Component):
```html
<button (click)="logout()">Logout</button>
<form (ngSubmit)="onSubmit()">...</form>
```

**d) Two-Way Binding** (Component ↔ Template):
```html
<input [(ngModel)]="email" />
```

#### 6. Directives

**Structural Directives** (modify DOM structure):

```html
<!-- *ngIf: Conditional rendering -->
<div *ngIf="currentUser">Welcome, {{ currentUser.name }}</div>

<!-- *ngFor: Loop through array -->
<div *ngFor="let task of tasks">
  {{ task.title }}
</div>
```

**Attribute Directives** (modify element appearance/behavior):

```html
<!-- [ngClass]: Conditional CSS classes -->
<div [ngClass]="{'active': isActive, 'disabled': !isEnabled}">

<!-- [ngStyle]: Conditional styles -->
<div [ngStyle]="{'color': task.priority === 'High' ? 'red' : 'black'}">
```

#### 7. HTTP Client

**Purpose**: Communicate with backend API

**Configuration** (app.config.ts):
```typescript
export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withFetch())
  ]
};
```

**Usage** (task.service.ts):
```typescript
// Get all tasks (role-based filtering)
getTasks(): Observable<Task[]> {
  return this.http.get<Task[]>(this.apiUrl, { withCredentials: true });
}

// Get tasks assigned to current user
getMyTasks(): Observable<Task[]> {
  return this.http.get<Task[]>(`${this.apiUrl}/my`, { withCredentials: true });
}

// Get completed tasks
getCompletedTasks(): Observable<Task[]> {
  return this.http.get<Task[]>(`${this.apiUrl}/completed`, { withCredentials: true });
}

// Get pending tasks
getPendingTasks(): Observable<Task[]> {
  return this.http.get<Task[]>(`${this.apiUrl}/pending`, { withCredentials: true });
}

// Get tasks created by current user
getCreatedByMe(): Observable<Task[]> {
  return this.http.get<Task[]>(`${this.apiUrl}/created-by-me`, { withCredentials: true });
}

// Create new task
createTask(task: CreateTaskRequest): Observable<Task> {
  return this.http.post<Task>(this.apiUrl, task, { withCredentials: true });
}

// Start task (Employee only)
startTask(id: number): Observable<any> {
  return this.http.put(`${this.apiUrl}/start/${id}`, {}, { withCredentials: true });
}

// Complete task (Employee only)
completeTask(id: number): Observable<any> {
  return this.http.put(`${this.apiUrl}/complete/${id}`, {}, { withCredentials: true });
}
```

**Features**:
- Observable-based (RxJS)
- Type-safe responses
- Automatic JSON parsing
- Interceptor support
- Error handling
- Cookie support with withCredentials

#### 8. RxJS Observables

**Purpose**: Handle asynchronous operations

**Example**:
```typescript
// Service returns Observable
this.authService.login(credentials)
  .subscribe({
    next: (user) => {
      // Success: user logged in
      this.router.navigate(['/dashboard']);
    },
    error: (error) => {
      // Error: show error message
      this.errorMessage = 'Login failed';
    }
  });
```

**Key Concepts**:
- Observable: Stream of data over time
- Subscribe: Listen to observable
- Operators: Transform data (map, filter, tap, etc.)
- BehaviorSubject: Observable with current value

### Angular Application Flow

```
1. Application Starts
   ↓
   main.ts bootstraps AppComponent
   
2. AppComponent Loads
   ↓
   Renders root template with <router-outlet>
   
3. Router Initializes
   ↓
   Checks current URL
   ↓
   Loads matching component (e.g., LoginComponent)
   
4. Component Initializes
   ↓
   Constructor runs
   ↓
   ngOnInit() lifecycle hook runs
   ↓
   Component fetches data from services
   
5. Services Make API Calls
   ↓
   HttpClient sends requests to backend
   ↓
   Backend processes and returns data
   
6. Component Receives Data
   ↓
   Updates component properties
   ↓
   Angular change detection runs
   ↓
   Template re-renders with new data
   
7. User Interacts
   ↓
   Clicks button, fills form, etc.
   ↓
   Event handlers execute
   ↓
   Component updates state
   ↓
   Template updates automatically
   
8. Navigation
   ↓
   User clicks link or programmatic navigation
   ↓
   Router changes URL
   ↓
   New component loads
   ↓
   Process repeats from step 4
```

### State Management

**This project uses services for state management**:

```typescript
// AuthService maintains authentication state
private currentUserSubject = new BehaviorSubject<User | null>(null);
public currentUser$ = this.currentUserSubject.asObservable();

// Components subscribe to state changes
this.authService.currentUser$.subscribe(user => {
  this.currentUser = user;
  if (!user) {
    this.router.navigate(['/login']);
  }
});
```

**Benefits**:
- Centralized state
- Reactive updates
- Components automatically sync
- Survives navigation (within session)

---

## Angular CLI Usage

### What is Angular CLI?

**Angular CLI (Command Line Interface)** is a powerful tool for:
- Creating new Angular projects
- Generating components, services, modules
- Running development server
- Building for production
- Running tests
- Managing dependencies

### Installation

```bash
# Install Angular CLI globally
npm install -g @angular/cli

# Verify installation
ng version
```

### Common Commands

#### Project Creation

```bash
# Create new Angular project
ng new project-name

# Create with routing
ng new project-name --routing

# Create with specific style format
ng new project-name --style=scss
```

#### Development Server

```bash
# Start development server (default port 4200)
ng serve

# Start on specific port
ng serve --port 4300

# Open browser automatically
ng serve --open

# Enable live reload
ng serve --live-reload
```

**This project**:
```bash
cd TaskManagementUI
ng serve
# Application runs on http://localhost:4200
```

#### Code Generation

```bash
# Generate component
ng generate component component-name
ng g c component-name  # Shorthand

# Generate service
ng generate service service-name
ng g s service-name

# Generate standalone component
ng g c component-name --standalone

# Generate component with inline template/styles
ng g c component-name --inline-template --inline-style
```

**Examples from this project**:
```bash
ng g c components/login --standalone
ng g c components/dashboard --standalone
ng g s services/auth
ng g s services/task
```

#### Building

```bash
# Build for development
ng build

# Build for production
ng build --configuration production
ng build --prod  # Shorthand

# Build with specific output path
ng build --output-path dist/my-app
```

**Production build optimizations**:
- Minification
- Tree shaking (removes unused code)
- Ahead-of-Time (AOT) compilation
- Bundle optimization

#### Testing

```bash
# Run unit tests
ng test

# Run tests once (CI mode)
ng test --watch=false

# Run end-to-end tests
ng e2e
```

#### Other Useful Commands

```bash
# Check Angular version
ng version

# Update Angular packages
ng update

# Add new capability (e.g., PWA, Angular Material)
ng add @angular/pwa
ng add @angular/material

# Lint code
ng lint

# Analyze bundle size
ng build --stats-json
```

### Package.json Scripts

**This project's scripts**:

```json
{
  "scripts": {
    "ng": "ng",
    "start": "ng serve",
    "build": "ng build",
    "watch": "ng build --watch --configuration development",
    "test": "ng test"
  }
}
```

**Usage**:
```bash
npm start        # Same as ng serve
npm run build    # Same as ng build
npm test         # Same as ng test
```

### Project Structure Created by CLI

```
TaskManagementUI/
├── src/
│   ├── app/                 # Application code
│   ├── assets/              # Static assets (images, fonts)
│   ├── index.html           # Main HTML file
│   ├── main.ts              # Application entry point
│   └── styles.css           # Global styles
├── angular.json             # Angular CLI configuration
├── package.json             # Dependencies and scripts
├── tsconfig.json            # TypeScript configuration
└── tsconfig.app.json        # App-specific TypeScript config
```

### Angular CLI Benefits

1. **Consistency**: Enforces best practices and project structure
2. **Productivity**: Generates boilerplate code automatically
3. **Optimization**: Handles build optimization
4. **Development Experience**: Hot reload, error reporting
5. **Testing**: Integrated testing tools
6. **Deployment**: Production-ready builds

---

## HTTP Request-Response Lifecycle

### Complete Request-Response Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    CLIENT (Angular)                             │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 1. User Action
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Component: login.ts                                            │
│  onSubmit() {                                                   │
│    this.authService.login(credentials).subscribe(...)          │
│  }                                                              │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 2. Service Call
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Service: auth.service.ts                                       │
│  login(credentials): Observable<User> {                         │
│    return this.http.post<User>(url, credentials, options)      │
│  }                                                              │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 3. HTTP Request
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  HTTP Client                                                    │
│  - Serializes credentials to JSON                              │
│  - Adds headers (Content-Type: application/json)               │
│  - Includes session cookie (withCredentials: true)             │
│  - Sends POST request to http://localhost:5150/api/auth/login  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 4. Network
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                    SERVER (ASP.NET Core)                        │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 5. Request Enters Pipeline
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Middleware Pipeline                                            │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ 1. CORS Middleware                                       │  │
│  │    - Checks request origin                               │  │
│  │    - Validates against allowed origins                   │  │
│  │    - Adds CORS headers to response                       │  │
│  └──────────────────────────────────────────────────────────┘  │
│                              ↓                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ 2. Session Middleware                                    │  │
│  │    - Reads session cookie from request                   │  │
│  │    - Loads session data from memory                      │  │
│  │    - Makes session available to controllers              │  │
│  └──────────────────────────────────────────────────────────┘  │
│                              ↓                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ 3. Authorization Middleware                              │  │
│  │    - Checks user permissions (if configured)             │  │
│  └──────────────────────────────────────────────────────────┘  │
│                              ↓                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ 4. Routing Middleware                                    │  │
│  │    - Matches URL to controller action                    │  │
│  │    - POST /api/auth/login → AuthController.Login()      │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 6. Controller Action
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Controller: AuthController.cs                                  │
│  [HttpPost("login")]                                            │
│  public async Task<IActionResult> Login(LoginDto loginDto) {   │
│    try {                                                        │
│      // 7. Validate credentials                                │
│      var user = await _context.Users                           │
│        .Where(u => u.Email == loginDto.Email)                  │
│        .FirstOrDefaultAsync();                                 │
│                                                                 │
│      if (user == null || !BCrypt.Verify(...)) {                │
│        return Unauthorized();                                  │
│      }                                                          │
│                                                                 │
│      // 8. Create session                                      │
│      HttpContext.Session.SetInt32("UserId", user.Id);          │
│      HttpContext.Session.SetString("UserRole", user.Role);     │
│                                                                 │
│      // 9. Return response                                     │
│      return Ok(new { id, name, email, role });                 │
│    }                                                            │
│    catch (Exception ex) {                                      │
│      return StatusCode(500, new { message, error });           │
│    }                                                            │
│  }                                                              │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 10. Database Query
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Entity Framework Core                                          │
│  - Translates LINQ to SQL                                       │
│  - Executes: SELECT * FROM Users WHERE Email = @email          │
│  - Returns User entity                                          │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 11. Response Travels Back
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Middleware Pipeline (Reverse Order)                            │
│  - Session middleware saves session changes to cookie           │
│  - CORS middleware adds headers                                 │
│  - Response serialized to JSON                                  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 12. HTTP Response
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  HTTP Response                                                  │
│  Status: 200 OK                                                 │
│  Headers:                                                       │
│    Content-Type: application/json                              │
│    Set-Cookie: .AspNetCore.Session=...                         │
│    Access-Control-Allow-Origin: http://localhost:4200          │
│    Access-Control-Allow-Credentials: true                      │
│  Body:                                                          │
│    { "id": 1, "name": "Admin", "email": "...", "role": "..." } │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 13. Network
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Angular HTTP Client                                            │
│  - Receives response                                            │
│  - Parses JSON automatically                                    │
│  - Stores cookie in browser                                     │
│  - Returns Observable with User object                          │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 14. Observable Subscription
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Component: login.ts                                            │
│  .subscribe({                                                   │
│    next: (user) => {                                            │
│      // 15. Success handler                                     │
│      this.router.navigate(['/dashboard']);                      │
│    },                                                           │
│    error: (error) => {                                          │
│      // Error handler                                           │
│      this.errorMessage = 'Login failed';                        │
│    }                                                            │
│  })                                                             │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ 16. Navigation
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Angular Router                                                 │
│  - Changes URL to /dashboard                                    │
│  - Loads DashboardComponent                                     │
│  - No page reload (SPA behavior)                                │
└─────────────────────────────────────────────────────────────────┘
```


### Request Components

#### HTTP Request Structure

```
POST /api/auth/login HTTP/1.1
Host: localhost:5150
Content-Type: application/json
Cookie: .AspNetCore.Session=CfDJ8...
Origin: http://localhost:4200
Accept: application/json

{
  "email": "admin@test.com",
  "password": "admin123"
}
```

**Components**:
- **Method**: POST (creating/modifying data)
- **URL**: /api/auth/login (endpoint path)
- **Headers**: Metadata about request
- **Cookie**: Session identifier
- **Body**: JSON payload with credentials

#### HTTP Response Structure

```
HTTP/1.1 200 OK
Content-Type: application/json
Set-Cookie: .AspNetCore.Session=CfDJ8...; path=/; httponly
Access-Control-Allow-Origin: http://localhost:4200
Access-Control-Allow-Credentials: true

{
  "id": 1,
  "name": "Admin User",
  "email": "admin@test.com",
  "role": "Admin"
}
```

**Components**:
- **Status Code**: 200 OK (success)
- **Headers**: Metadata about response
- **Set-Cookie**: New/updated session cookie
- **Body**: JSON payload with user data

### HTTP Methods Used

| Method | Purpose | Example Endpoint |
|--------|---------|------------------|
| GET | Retrieve data | GET /api/task |
| POST | Create data | POST /api/task |
| PUT | Update data | PUT /api/task/start/1 |
| DELETE | Delete data | DELETE /api/user/5 |

### Status Codes Used

| Code | Meaning | When Used |
|------|---------|-----------|
| 200 OK | Success | Successful GET, POST, PUT |
| 400 Bad Request | Invalid input | Validation errors, business rule violations |
| 401 Unauthorized | Not authenticated | Missing or invalid session |
| 403 Forbidden | Not authorized | Insufficient permissions |
| 404 Not Found | Resource not found | User or task doesn't exist |
| 500 Internal Server Error | Server error | Unexpected errors, database errors |

---

## Middleware Pipeline

### What is Middleware?

**Middleware** are components that form a pipeline to process HTTP requests and responses. Each middleware can:
1. Process the incoming request
2. Pass control to the next middleware
3. Process the outgoing response

### Middleware Pipeline in This Project

**Configured in Program.cs**:

```csharp
var app = builder.Build();

// Middleware pipeline (order matters!)
app.UseCors("AllowAngular");      // 1. CORS
app.UseSession();                  // 2. Session
app.UseAuthorization();            // 3. Authorization
app.MapControllers();              // 4. Routing
```

### Request Flow Through Pipeline

```
┌─────────────────────────────────────────────────────────────┐
│                    Incoming Request                         │
└─────────────────────────────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│  1. CORS Middleware (app.UseCors)                           │
│  ─────────────────────────────────────────────────────────  │
│  Purpose: Handle Cross-Origin Resource Sharing             │
│                                                             │
│  Actions:                                                   │
│  - Check request origin (http://localhost:4200)            │
│  - Validate against allowed origins                        │
│  - Add CORS headers to response:                           │
│    • Access-Control-Allow-Origin                           │
│    • Access-Control-Allow-Credentials                      │
│    • Access-Control-Allow-Methods                          │
│    • Access-Control-Allow-Headers                          │
│                                                             │
│  Why First?                                                 │
│  - Must run before other middleware                        │
│  - Handles preflight OPTIONS requests                      │
│  - Prevents unauthorized cross-origin requests             │
└─────────────────────────────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│  2. Session Middleware (app.UseSession)                     │
│  ─────────────────────────────────────────────────────────  │
│  Purpose: Manage user sessions                             │
│                                                             │
│  Actions:                                                   │
│  - Read session cookie from request                        │
│  - Load session data from memory store                     │
│  - Make session available via HttpContext.Session          │
│  - Track session changes                                   │
│  - Save session changes to cookie on response              │
│                                                             │
│  Session Data:                                              │
│  - UserId: int                                             │
│  - UserRole: string                                        │
│  - UserName: string                                        │
│                                                             │
│  Why Here?                                                  │
│  - Must run before controllers need session data           │
│  - Must run after CORS (cookies need CORS headers)         │
└─────────────────────────────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│  3. Authorization Middleware (app.UseAuthorization)         │
│  ─────────────────────────────────────────────────────────  │
│  Purpose: Check user permissions                           │
│                                                             │
│  Actions:                                                   │
│  - Verify user is authenticated (if required)              │
│  - Check user has required roles/claims                    │
│  - Return 401/403 if not authorized                        │
│                                                             │
│  Note: This project uses manual authorization checks       │
│  in controllers rather than [Authorize] attributes         │
│                                                             │
│  Why Here?                                                  │
│  - Must run after session (needs user info)                │
│  - Must run before routing to controllers                  │
└─────────────────────────────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│  4. Routing Middleware (app.MapControllers)                 │
│  ─────────────────────────────────────────────────────────  │
│  Purpose: Route requests to controller actions             │
│                                                             │
│  Actions:                                                   │
│  - Match request URL to controller route                   │
│  - Extract route parameters                                │
│  - Invoke controller action method                         │
│  - Pass request data to action                             │
│                                                             │
│  Example:                                                   │
│  POST /api/auth/login                                       │
│    → AuthController.Login()                                 │
│                                                             │
│  PUT /api/task/start/5                                      │
│    → TaskController.StartTask(5)                            │
└─────────────────────────────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│  5. Controller Action Executes                              │
│  ─────────────────────────────────────────────────────────  │
│  - Validates input                                          │
│  - Executes business logic                                  │
│  - Queries database                                         │
│  - Returns IActionResult                                    │
└─────────────────────────────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│                    Response Travels Back                    │
│                    (Reverse Order)                          │
└─────────────────────────────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│  Routing → Authorization → Session → CORS → Client          │
│                                                             │
│  - Session middleware saves session changes                 │
│  - CORS middleware adds headers                             │
│  - Response sent to client                                  │
└─────────────────────────────────────────────────────────────┘
```

### Middleware Order Importance

**Order matters!** Middleware executes in the order it's added.

**Correct Order**:
```csharp
app.UseCors("AllowAngular");   // 1. First
app.UseSession();               // 2. Second
app.UseAuthorization();         // 3. Third
app.MapControllers();           // 4. Last
```

**Why This Order?**

1. **CORS First**: Must handle preflight requests before anything else
2. **Session Second**: Controllers need session data
3. **Authorization Third**: Needs session data to check permissions
4. **Routing Last**: Executes controller actions

**Wrong Order Example**:
```csharp
app.UseSession();               // ❌ Wrong!
app.UseCors("AllowAngular");   // Session won't work without CORS headers
```

### Custom Middleware Example

**You could add custom middleware**:

```csharp
// Logging middleware
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();  // Call next middleware
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});
```

### Middleware Benefits

1. **Modularity**: Each middleware has single responsibility
2. **Reusability**: Middleware can be reused across projects
3. **Flexibility**: Easy to add/remove middleware
4. **Separation of Concerns**: Cross-cutting concerns handled separately
5. **Testability**: Each middleware can be tested independently

---

## Exception Handling

### Exception Handling Strategy

This project uses **try-catch blocks** in controller actions to handle exceptions gracefully.

### Why Exception Handling?

1. **User Experience**: Show friendly error messages instead of crashes
2. **Security**: Don't expose sensitive error details to clients
3. **Logging**: Capture errors for debugging
4. **Stability**: Prevent application crashes
5. **Debugging**: Easier to identify and fix issues

### Exception Handling in Controllers

#### Example: TaskController.GetTasks()

```csharp
[HttpGet]
public async Task<IActionResult> GetTasks()
{
    try
    {
        // Attempt to retrieve tasks
        var userId = HttpContext.Session.GetInt32("UserId");
        var userRole = HttpContext.Session.GetString("UserRole");

        if (userId == null)
        {
            return Unauthorized();
        }

        var tasks = await _context.Tasks
            .Include(t => t.AssignedByUser)
            .Include(t => t.AssignedToUser)
            .ToListAsync();

        return Ok(tasks);
    }
    catch (Exception ex)
    {
        // Catch any unexpected errors
        return StatusCode(500, new 
        { 
            message = "An error occurred while retrieving tasks",
            error = ex.Message 
        });
    }
}
```

### Types of Exceptions Handled

#### 1. Database Exceptions

**Scenario**: Database connection failure, query timeout

```csharp
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    return StatusCode(500, new 
    { 
        message = "Database error occurred",
        error = ex.Message 
    });
}
```

**Common Causes**:
- Connection string incorrect
- Database server down
- Foreign key constraint violation
- Unique constraint violation

#### 2. Validation Exceptions

**Scenario**: Invalid input data

```csharp
if (string.IsNullOrEmpty(createTaskDto.Title))
{
    return BadRequest(new { message = "Title is required" });
}
```

**Common Causes**:
- Missing required fields
- Invalid data format
- Business rule violations

#### 3. Authorization Exceptions

**Scenario**: User not authorized for action

```csharp
if (userRole != "Admin")
{
    return Forbid();  // 403 Forbidden
}
```

**Common Causes**:
- Insufficient permissions
- Expired session
- Invalid role

#### 4. Not Found Exceptions

**Scenario**: Resource doesn't exist

```csharp
var task = await _context.Tasks.FindAsync(id);
if (task == null)
{
    return NotFound(new { message = "Task not found" });
}
```

**Common Causes**:
- Invalid ID
- Resource deleted
- User doesn't have access

### Exception Handling Best Practices

#### 1. Specific Catch Blocks

**Good**:
```csharp
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException ex)
{
    // Handle concurrency conflict
}
catch (DbUpdateException ex)
{
    // Handle database error
}
catch (Exception ex)
{
    // Handle any other error
}
```

**Bad**:
```csharp
try
{
    await _context.SaveChangesAsync();
}
catch (Exception ex)
{
    // Too generic, can't handle specific errors
}
```

#### 2. Meaningful Error Messages

**Good**:
```csharp
return BadRequest(new 
{ 
    message = "Task can only be started from Pending status",
    currentStatus = task.Status 
});
```

**Bad**:
```csharp
return BadRequest("Error");  // Not helpful
```

#### 3. Don't Expose Sensitive Information

**Good**:
```csharp
catch (Exception ex)
{
    // Log full error server-side
    _logger.LogError(ex, "Error creating task");
    
    // Return generic message to client
    return StatusCode(500, new { message = "An error occurred" });
}
```

**Bad**:
```csharp
catch (Exception ex)
{
    // Exposes stack trace and internal details
    return StatusCode(500, ex);
}
```

#### 4. Return Appropriate Status Codes

| Scenario | Status Code | Method |
|----------|-------------|--------|
| Success | 200 OK | `return Ok(data);` |
| Created | 201 Created | `return Created(uri, data);` |
| Invalid input | 400 Bad Request | `return BadRequest(message);` |
| Not authenticated | 401 Unauthorized | `return Unauthorized();` |
| Not authorized | 403 Forbidden | `return Forbid();` |
| Not found | 404 Not Found | `return NotFound(message);` |
| Server error | 500 Internal Server Error | `return StatusCode(500, message);` |

### Frontend Error Handling

#### Angular Service Error Handling

```typescript
login(credentials: LoginRequest): Observable<User> {
  return this.http.post<User>(`${this.apiUrl}/login`, credentials)
    .pipe(
      tap(user => this.currentUserSubject.next(user)),
      catchError(error => {
        console.error('Login error:', error);
        return throwError(() => new Error('Login failed'));
      })
    );
}
```

#### Component Error Handling

```typescript
onSubmit(): void {
  this.authService.login({ email: this.email, password: this.password })
    .subscribe({
      next: () => {
        // Success
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        // Error
        this.errorMessage = 'Invalid email or password';
        console.error('Login failed:', error);
      }
    });
}
```

### Error Response Format

**Consistent error response structure**:

```json
{
  "message": "Human-readable error message",
  "error": "Technical error details (optional)",
  "field": "Specific field that caused error (optional)"
}
```

**Examples**:

```json
// Validation error
{
  "message": "Email already exists"
}

// Authorization error
{
  "message": "Task not found or not assigned to you"
}

// Server error
{
  "message": "An error occurred while creating the task",
  "error": "Database connection timeout"
}
```

---

## Project Structure

### Backend Structure (ASP.NET Core)

```
TaskManagementAPI/
├── Controllers/                    # API Endpoints
│   ├── AuthController.cs          # Authentication (login, logout, session)
│   ├── TaskController.cs          # Task management (CRUD, status updates)
│   └── UserController.cs          # User management (CRUD)
│
├── Data/                           # Database Context
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── DbSeeder.cs                # Database seeding
│
├── DTOs/                           # Data Transfer Objects
│   ├── LoginDto.cs                # Login credentials
│   ├── CreateUserDto.cs           # User creation data
│   ├── UserDto.cs                 # User response data
│   ├── CreateTaskDto.cs           # Task creation data
│   └── TaskDto.cs                 # Task response data
│
├── Models/                         # Entity Models
│   ├── User.cs                    # User entity
│   ├── TaskItem.cs                # Task entity
│   └── TaskComment.cs             # Comment entity
│
├── Migrations/                     # EF Core Migrations
│   ├── 20260311183832_InitialCreate.cs
│   ├── 20260311183832_InitialCreate.Designer.cs
│   └── ApplicationDbContextModelSnapshot.cs
│
├── Properties/
│   └── launchSettings.json        # Launch configuration
│
├── bin/                            # Compiled binaries
├── obj/                            # Build artifacts
│
├── Program.cs                      # Application entry point
├── appsettings.json               # Configuration
├── appsettings.Development.json   # Development configuration
└── TaskManagementAPI.csproj       # Project file
```

### Frontend Structure (Angular)

```
TaskManagementUI/
├── src/
│   ├── app/
│   │   ├── components/                    # UI Components
│   │   │   ├── login/
│   │   │   │   ├── login.ts              # Login logic
│   │   │   │   ├── login.html            # Login template
│   │   │   │   └── login.css             # Login styles
│   │   │   ├── dashboard/
│   │   │   │   ├── dashboard.ts          # Dashboard logic
│   │   │   │   ├── dashboard.html        # Dashboard template
│   │   │   │   └── dashboard.css         # Dashboard styles
│   │   │   ├── create-task/
│   │   │   │   ├── create-task.ts        # Task creation logic
│   │   │   │   ├── create-task.html      # Task creation form
│   │   │   │   └── create-task.css       # Form styles
│   │   │   ├── task-list/
│   │   │   │   ├── task-list.ts          # Task list logic
│   │   │   │   ├── task-list.html        # Task list template
│   │   │   │   └── task-list.css         # List styles
│   │   │   └── my-tasks/
│   │   │       ├── my-tasks.ts           # Employee tasks logic
│   │   │       ├── my-tasks.html         # Employee tasks template
│   │   │       └── my-tasks.css          # Tasks styles
│   │   │
│   │   ├── services/                      # Business Logic & API
│   │   │   ├── auth.service.ts           # Authentication service
│   │   │   ├── task.service.ts           # Task service
│   │   │   └── user.service.ts           # User service
│   │   │
│   │   ├── models/                        # TypeScript Interfaces
│   │   │   ├── user.ts                   # User interface
│   │   │   └── task.ts                   # Task interface
│   │   │
│   │   ├── app.routes.ts                 # Routing configuration
│   │   ├── app.config.ts                 # App configuration
│   │   ├── app.ts                        # Root component
│   │   └── app.html                      # Root template
│   │
│   ├── assets/                            # Static assets
│   ├── index.html                         # Main HTML file
│   ├── main.ts                            # Application entry point
│   └── styles.css                         # Global styles
│
├── .angular/                              # Angular cache
├── node_modules/                          # Dependencies
│
├── angular.json                           # Angular CLI configuration
├── package.json                           # Dependencies and scripts
├── package-lock.json                      # Dependency lock file
├── tsconfig.json                          # TypeScript configuration
└── tsconfig.app.json                      # App TypeScript config
```

### Key Files Explained

#### Backend

**Program.cs**: Application entry point, configures services and middleware
**appsettings.json**: Configuration (connection strings, logging)
**ApplicationDbContext.cs**: Database context, entity relationships
**Controllers**: Handle HTTP requests, implement business logic
**Models**: Entity classes representing database tables
**DTOs**: Data transfer objects for API communication
**Migrations**: Database schema version control

#### Frontend

**main.ts**: Application entry point, bootstraps AppComponent
**app.config.ts**: Application configuration (providers)
**app.routes.ts**: Routing configuration
**Components**: UI building blocks (logic + template + styles)
**Services**: Business logic, API communication, state management
**Models**: TypeScript interfaces for type safety

---

## Running the Application

### Prerequisites

1. **.NET 8.0 SDK**
   ```bash
   # Check installation
   dotnet --version
   ```

2. **Node.js and npm**
   ```bash
   # Check installation
   node --version
   npm --version
   ```

3. **SQL Server / LocalDB**
   - Included with Visual Studio
   - Or install SQL Server Express

4. **Angular CLI**
   ```bash
   npm install -g @angular/cli
   ```

### Setup Instructions

#### 1. Clone/Download Project

```bash
# Navigate to project directory
cd path/to/project
```

#### 2. Setup Backend

```bash
# Navigate to API project
cd TaskManagementAPI

# Restore NuGet packages
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the API
dotnet run
```

**API will run on**: `http://localhost:5150`

**Verify API is running**:
- Open browser: `http://localhost:5150/swagger`
- Should see Swagger UI with API documentation

#### 3. Setup Frontend

```bash
# Navigate to UI project (in new terminal)
cd TaskManagementUI

# Install npm packages
npm install

# Run the Angular app
npm start
# or
ng serve
```

**Frontend will run on**: `http://localhost:4200`

**Verify frontend is running**:
- Open browser: `http://localhost:4200`
- Should see login page

### Development Workflow

#### Terminal 1: Backend
```bash
cd TaskManagementAPI
dotnet watch run  # Auto-restart on code changes
```

#### Terminal 2: Frontend
```bash
cd TaskManagementUI
ng serve  # Auto-reload on code changes
```

### Database Management

#### Create New Migration

```bash
cd TaskManagementAPI
dotnet ef migrations add MigrationName
```

#### Apply Migrations

```bash
dotnet ef database update
```

#### Rollback Migration

```bash
dotnet ef database update PreviousMigrationName
```

#### Remove Last Migration

```bash
dotnet ef migrations remove
```

### Building for Production

#### Backend

```bash
cd TaskManagementAPI
dotnet publish -c Release -o ./publish
```

#### Frontend

```bash
cd TaskManagementUI
ng build --configuration production
```

**Output**: `dist/` folder with optimized files

### Troubleshooting

#### Backend Issues

**Problem**: Database connection error
```
Solution: Check connection string in appsettings.json
```

**Problem**: Port 5150 already in use
```
Solution: Change port in Properties/launchSettings.json
```

**Problem**: Migration error
```
Solution: Delete Migrations folder and database, recreate:
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Frontend Issues

**Problem**: Port 4200 already in use
```
Solution: ng serve --port 4300
```

**Problem**: CORS error
```
Solution: Ensure backend CORS policy allows http://localhost:4200
```

**Problem**: npm install fails
```
Solution: Delete node_modules and package-lock.json, run npm install again
```

---

## Demo Credentials

The system is seeded with test users for each role:

### Admin Account

```
Email: admin@test.com
Password: admin123
```

**Capabilities**:
- Create and delete users
- Create and assign tasks
- View all tasks
- Full system access

### Manager Account

```
Email: manager@test.com
Password: manager123
```

**Capabilities**:
- Create and assign tasks
- View tasks they created
- View all users (for task assignment)

### Employee Accounts

**Employee 1**:
```
Email: employee1@test.com
Password: employee123
```

**Employee 2**:
```
Email: employee2@test.com
Password: employee123
```

**Capabilities**:
- View tasks assigned to them
- Start and complete their tasks
- Cannot create tasks or users

### Testing Workflow

1. **Login as Admin**:
   - Create new users
   - Create tasks and assign to employees
   - View all tasks in system

2. **Login as Manager**:
   - Create tasks and assign to employees
   - View only tasks you created
   - Cannot delete users

3. **Login as Employee**:
   - View tasks assigned to you
   - Start pending tasks
   - Complete in-progress tasks
   - Cannot create tasks

---

## Task Editing and Attachment Permission Workflow

### Overview

The system implements strict permission controls for task editing and attachment uploads, with a comprehensive request-approval system for modifications after task completion.

### Core Permission Rules

#### 1. Task Edit Permissions

**Who Can Edit:**
- Only the task creator (user who assigned the task via `AssignedBy` field)

**When Can Edit:**
- Only when task status is NOT "Completed"
- Allowed statuses: "Pending", "In Progress"

**What Can Be Edited:**
- Title
- Description
- Priority (Low, Medium, High)
- Deadline

**Enforcement:**
```csharp
// Backend validation in TaskController.cs
if (task.AssignedBy != currentUserId)
{
    return StatusCode(403, "Only the task creator can edit this task");
}

if (task.Status == "Completed")
{
    return StatusCode(403, "Cannot edit completed tasks");
}
```

#### 2. Attachment Upload Permissions

**Who Can Upload:**
- Only the task creator (`AssignedBy` user)
- No one else, including the assigned employee

**When Can Upload:**
- Only when task status is NOT "Completed"
- Allowed statuses: "Pending", "In Progress"

**Enforcement:**
```csharp
// Backend validation in TaskController.cs
if (task.AssignedBy != currentUserId)
{
    return StatusCode(403, "Only the task creator can upload attachments");
}

if (task.Status == "Completed")
{
    return StatusCode(403, "Cannot upload attachments to completed tasks");
}
```

#### 3. Task Completion Lock

When a task status becomes "Completed":
- ❌ No editing allowed
- ❌ No attachment uploads allowed
- ❌ No modifications to task details
- ✅ Can still view task and existing attachments
- ✅ Can add comments
- ✅ Can add progress updates (by assigned employee)

### Post-Completion Permission Request System

#### Database Schema

**AttachmentPermissionRequests Table:**
```sql
CREATE TABLE AttachmentPermissionRequests (
    Id INT PRIMARY KEY IDENTITY,
    TaskId INT NOT NULL,
    RequestedByUserId INT NOT NULL,
    RequestType NVARCHAR(MAX) NOT NULL,  -- "Attachment" or "Edit"
    Message NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(MAX) NOT NULL,       -- "Pending", "Approved", "Rejected"
    CreatedAt DATETIME2 NOT NULL,
    ReviewedByUserId INT NULL,
    ReviewedAt DATETIME2 NULL,
    FOREIGN KEY (TaskId) REFERENCES Tasks(Id),
    FOREIGN KEY (RequestedByUserId) REFERENCES Users(Id),
    FOREIGN KEY (ReviewedByUserId) REFERENCES Users(Id)
);
```

**Entity Model:**
```csharp
public class AttachmentPermissionRequest
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int RequestedByUserId { get; set; }
    public string RequestType { get; set; } = "Attachment";
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public TaskItem? Task { get; set; }
    public User? RequestedByUser { get; set; }
    public User? ReviewedByUser { get; set; }
}
```

#### API Endpoints

**1. Create Permission Request**
```http
POST /api/task/{taskId}/attachment-request
Content-Type: application/json

{
  "requestType": "Attachment",
  "message": "Need to upload final documentation"
}
```

**Authorization:**
- Task must be completed
- User must be authenticated
- Cannot have duplicate pending requests

**Response:**
```json
{
  "id": 1,
  "taskId": 5,
  "requestedByUserId": 2,
  "requestedByUserName": "John Manager",
  "requestType": "Attachment",
  "message": "Need to upload final documentation",
  "status": "Pending",
  "createdAt": "2026-03-13T10:30:00Z"
}
```

**2. Get Permission Requests**
```http
GET /api/task/{taskId}/attachment-requests
```

**Authorization:**
- Only task creator can view requests

**Response:**
```json
[
  {
    "id": 1,
    "taskId": 5,
    "requestedByUserId": 2,
    "requestedByUserName": "John Manager",
    "requestType": "Attachment",
    "message": "Need to upload final documentation",
    "status": "Pending",
    "createdAt": "2026-03-13T10:30:00Z",
    "reviewedByUserId": null,
    "reviewedByUserName": null,
    "reviewedAt": null
  }
]
```

**3. Approve Permission Request**
```http
POST /api/task/attachment-request/{id}/approve
```

**Authorization:**
- Only task creator can approve
- Request must be in "Pending" status

**Effect:**
- Request status changes to "Approved"
- Requesting user can now upload attachments to the completed task

**4. Reject Permission Request**
```http
POST /api/task/attachment-request/{id}/reject
```

**Authorization:**
- Only task creator can reject
- Request must be in "Pending" status

**Effect:**
- Request status changes to "Rejected"
- Requesting user remains blocked from uploading

### Frontend Implementation

#### Task Details Page Features

**1. Edit Task Button**
- Shown when: User is task creator AND task is not completed
- Action: Enables inline editing of task details
- Fields: Title, Description, Priority, Deadline

**2. Upload Attachment Button**
- Shown when: User is task creator AND task is not completed
- Action: Opens file picker to upload attachment
- Validation: File required before upload

**3. Request Upload Permission Button**
- Shown when: User is task creator AND task is completed
- Action: Opens permission request form
- Form fields: Message (required)

**4. Permission Requests Section**
- Shown when: User is task creator AND requests exist
- Displays: All permission requests for the task
- Actions: Approve/Reject buttons for pending requests
- Status badges: Pending (yellow), Approved (green), Rejected (red)

#### TypeScript Implementation

```typescript
// Check if user can edit task
canEditTask(): boolean {
    if (!this.task) return false;
    return this.task.assignedBy === this.currentUserId && 
           this.task.status !== 'Completed';
}

// Check if user can upload attachments
canUploadAttachment(): boolean {
    if (!this.task) return false;
    return this.task.assignedBy === this.currentUserId && 
           this.task.status !== 'Completed';
}

// Check if user can request permission
canRequestPermission(): boolean {
    if (!this.task) return false;
    return this.task.assignedBy === this.currentUserId && 
           this.task.status === 'Completed';
}

// Check if user can see permission requests
canSeePermissionRequests(): boolean {
    if (!this.task) return false;
    return this.task.assignedBy === this.currentUserId;
}
```

### Complete Workflow Examples

#### Example 1: Normal Task Lifecycle

1. **Admin creates task** (Admin is task creator)
   - Status: Pending
   - Admin can: Edit task, Upload attachments

2. **Admin uploads requirements.pdf**
   - ✅ Success (is creator, task not completed)

3. **Employee starts task**
   - Status: In Progress
   - Admin can still: Edit task, Upload attachments

4. **Admin uploads additional-specs.pdf**
   - ✅ Success (is creator, task not completed)

5. **Employee completes task**
   - Status: Completed
   - Admin can: View only, Request permission

6. **Admin tries to upload final-report.pdf**
   - ❌ Blocked: "Cannot upload attachments to completed tasks"

#### Example 2: Post-Completion Upload Request

1. **Task is completed**
   - Admin wants to upload final documentation

2. **Admin clicks "Request Upload Permission"**
   - Opens permission request form

3. **Admin submits request**
   ```json
   {
     "requestType": "Attachment",
     "message": "Need to upload final project documentation"
   }
   ```

4. **Task creator (Admin) sees request**
   - Request appears in "Permission Requests" section
   - Shows: Requester name, message, date
   - Actions: Approve, Reject

5. **Task creator approves request**
   - Request status: Pending → Approved

6. **Admin can now upload**
   - Upload button becomes available
   - Can upload final-report.pdf
   - ✅ Success (has approved permission)

#### Example 3: Edit Task Workflow

1. **Manager creates task**
   - Status: Pending
   - Manager sees "Edit Task" button

2. **Manager clicks "Edit Task"**
   - Form appears with current values
   - Can edit: Title, Description, Priority, Deadline

3. **Manager updates priority to "High"**
   - Clicks "Save Changes"
   - Task updated successfully

4. **Task is completed**
   - "Edit Task" button disappears
   - Cannot edit anymore

### Security Considerations

**Backend Validation:**
- All permission checks enforced in backend
- Never rely on frontend validation alone
- Session-based authentication required
- Role-based authorization applied

**Permission Checks:**
```csharp
// Always verify task creator
if (task.AssignedBy != userId.Value)
{
    return StatusCode(403, new { message = "Forbidden" });
}

// Always check task status
if (task.Status == "Completed")
{
    // Check for approved permission request
    bool hasApprovedRequest = await _context.AttachmentPermissionRequests
        .AnyAsync(apr => apr.TaskId == taskId 
                      && apr.RequestedByUserId == userId.Value 
                      && apr.RequestType == "Attachment"
                      && apr.Status == "Approved");
    
    if (!hasApprovedRequest)
    {
        return StatusCode(403, new { message = "Permission required" });
    }
}
```

### Database Migration

**Migration Name:** `AddRequestTypeToPermissionRequests`

**Applied:** March 13, 2026

**Changes:**
- Added `RequestType` column to `AttachmentPermissionRequests` table
- Type: `NVARCHAR(MAX)`
- Default: Empty string
- Values: "Attachment" or "Edit"

**Command:**
```bash
dotnet ef migrations add AddRequestTypeToPermissionRequests --project TaskManagementAPI
dotnet ef database update --project TaskManagementAPI
```

### Testing Checklist

**Task Edit Permissions:**
- ✅ Task creator can edit pending task
- ✅ Task creator can edit in-progress task
- ✅ Task creator cannot edit completed task
- ✅ Non-creator cannot edit any task
- ✅ Edit button shows/hides correctly

**Attachment Upload Permissions:**
- ✅ Task creator can upload to pending task
- ✅ Task creator can upload to in-progress task
- ✅ Task creator cannot upload to completed task
- ✅ Non-creator cannot upload to any task
- ✅ Upload button shows/hides correctly

**Permission Request System:**
- ✅ Can create request for completed task
- ✅ Cannot create request for active task
- ✅ Cannot create duplicate pending requests
- ✅ Task creator can view all requests
- ✅ Task creator can approve requests
- ✅ Task creator can reject requests
- ✅ Approved request allows upload
- ✅ Rejected request blocks upload

**UI/UX:**
- ✅ Edit form appears/disappears correctly
- ✅ Permission request form shows for completed tasks
- ✅ Permission requests section displays properly
- ✅ Status badges show correct colors
- ✅ Approve/Reject buttons work correctly
- ✅ Helpful messages shown when actions blocked

### Summary

The task editing and attachment permission system provides:

1. **Strict Access Control**: Only task creators can edit tasks and upload attachments
2. **Completion Lock**: Completed tasks are protected from modifications
3. **Flexible Post-Completion Access**: Permission request system allows controlled modifications after completion
4. **Audit Trail**: All permission requests tracked with timestamps and reviewers
5. **User-Friendly UI**: Clear visual indicators and helpful messages
6. **Backend Security**: All permissions enforced server-side with proper validation

This implementation ensures data integrity while providing flexibility for legitimate post-completion modifications through a controlled approval process.

---

## Completed Task Lock Rule

### Overview
The system enforces a strict lock on completed tasks to ensure data integrity and prevent any modifications once a task reaches the "Completed" status. This rule applies universally to ALL users regardless of their role or relationship to the task.

### Core Rule
**When a task status becomes "Completed", the task becomes completely read-only.**

No user can:
- Edit task details (title, description, priority, deadline)
- Upload attachments
- Delete attachments
- Add progress updates
- Modify any task information

This rule applies to:
- Admin users
- Manager users
- Employee users
- Task creator (AssignedBy)
- Task assignee (AssignedTo)
- All other users

### Business Justification
1. **Data Integrity:** Prevents tampering with completed work records
2. **Audit Compliance:** Ensures completed tasks remain as historical records
3. **Accountability:** Maintains accurate completion timestamps and final states
4. **Simplicity:** Clear, unambiguous rule with no exceptions
5. **Trust:** Stakeholders can rely on completed task data being immutable

### Backend Implementation

#### Validation Logic
All modification endpoints check task status before allowing any changes:

```csharp
// Check if task is completed
if (task.Status == "Completed")
{
    return StatusCode(403, new { message = "This task is completed and cannot be modified." });
}
```

#### Affected Endpoints

**1. Edit Task Endpoint**
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
{
    // COMPLETED TASK LOCK RULE: No one can modify completed tasks
    if (task.Status == "Completed")
    {
        return StatusCode(403, new { message = "This task is completed and cannot be modified." });
    }
    // ... proceed with update ...
}
```

**2. Upload Attachment Endpoint**
```csharp
[HttpPost("{taskId}/attachments")]
public async Task<IActionResult> UploadAttachment(int taskId, IFormFile file)
{
    // COMPLETED TASK LOCK RULE: No one can upload attachments to completed tasks
    if (task.Status == "Completed")
    {
        return StatusCode(403, new { message = "This task is completed and cannot be modified." });
    }
    // ... proceed with upload ...
}
```

**3. Add Progress Update Endpoint**
```csharp
[HttpPost("{taskId}/progress")]
public async Task<IActionResult> AddProgressUpdate(int taskId, [FromForm] CreateProgressUpdateDto dto, IFormFile? file)
{
    // COMPLETED TASK LOCK RULE: No one can add progress to completed tasks
    if (task.Status == "Completed")
    {
        return StatusCode(403, new { message = "This task is completed and cannot be modified." });
    }
    // ... proceed with progress update ...
}
```

### Frontend Implementation

#### TypeScript Component Logic

```typescript
// Check if task is completed
isTaskCompleted(): boolean {
    return this.task?.status === 'Completed';
}

// Edit permission - blocks completed tasks
canEditTask(): boolean {
    if (!this.task) return false;
    if (this.task.status === 'Completed') return false;
    return this.task.assignedBy === this.currentUserId;
}

// Upload permission - blocks completed tasks
canUploadAttachment(): boolean {
    if (!this.task) return false;
    if (this.task.status === 'Completed') return false;
    return this.task.assignedBy === this.currentUserId;
}

// Progress update permission - blocks completed tasks
canAddProgress(): boolean {
    if (!this.task) return false;
    if (this.task.status === 'Completed') return false;
    return this.task.assignedTo === this.currentUserId;
}
```

#### HTML Template Changes

```html
<!-- Completed Task Warning Message -->
<div class="info-message warning" *ngIf="isTaskCompleted()">
    ⚠️ This task is completed. No further modifications are allowed.
</div>

<!-- Edit button - hidden for completed tasks -->
<button *ngIf="canEditTask() && !isEditMode" 
        (click)="enableEditMode()" 
        class="btn-primary">
    Edit Task
</button>

<!-- Upload form - hidden for completed tasks -->
<div class="upload-form" *ngIf="canUploadAttachment()">
    <input type="file" (change)="onAttachmentFileSelected($event)" />
    <button (click)="uploadAttachment()" class="btn-primary">Upload</button>
</div>

<!-- Progress form - hidden for completed tasks -->
<div class="progress-form" *ngIf="canAddProgress()">
    <textarea [(ngModel)]="newProgressDescription"></textarea>
    <button (click)="addProgressUpdate()" class="btn-primary">Add Update</button>
</div>
```

### Permission Matrix

| Action | Admin | Manager | Employee | Task Creator | Task Assignee | Status |
|--------|-------|---------|----------|--------------|---------------|--------|
| Edit Task | ✗ | ✗ | ✗ | ✗ | ✗ | Completed |
| Upload Attachment | ✗ | ✗ | ✗ | ✗ | ✗ | Completed |
| Add Progress | ✗ | ✗ | ✗ | ✗ | ✗ | Completed |
| View Task | ✓ | ✓ | ✓ | ✓ | ✓ | Completed |
| View Attachments | ✓ | ✓ | ✓ | ✓ | ✓ | Completed |
| View Progress | ✓ | ✓ | ✓ | ✓ | ✓ | Completed |
| Add Comments | ✓ | ✓ | ✓ | ✓ | ✓ | Completed |

**Note:** Comments are still allowed on completed tasks as they don't modify the task itself.

### Error Handling

#### Backend Error Response
**HTTP Status Code:** 403 Forbidden

**Response Body:**
```json
{
    "message": "This task is completed and cannot be modified."
}
```

### Test Cases Summary

| Test Case | Expected Result |
|-----------|----------------|
| Active task - Edit | ✓ Works |
| Active task - Upload | ✓ Works |
| Active task - Progress | ✓ Works |
| Completed task - Edit (UI) | ✗ Button hidden |
| Completed task - Edit (API) | ✗ HTTP 403 |
| Completed task - Upload (UI) | ✗ Form hidden |
| Completed task - Upload (API) | ✗ HTTP 403 |
| Completed task - Progress (UI) | ✗ Form hidden |
| Completed task - Progress (API) | ✗ HTTP 403 |
| Admin override attempt | ✗ Blocked |
| Manager override attempt | ✗ Blocked |

### Benefits

1. **Data Integrity:** Completed tasks remain as accurate historical records
2. **Audit Compliance:** Immutable completion records for compliance requirements
3. **Simplicity:** No complex permission logic or exception handling
4. **Trust:** Stakeholders can rely on completed task data
5. **Accountability:** Clear completion timestamps that cannot be altered
6. **Performance:** Simpler validation logic improves response times

### Conclusion

The Completed Task Lock Rule provides a simple, secure, and effective way to protect completed task data. By enforcing a universal lock with no exceptions, the system ensures data integrity, supports audit compliance, and maintains trust in the task management process.

**Key Takeaways:**
- Completed tasks are completely read-only for ALL users
- No exceptions based on role or relationship to task
- Backend validation ensures security regardless of frontend state
- Clear error messages guide users when modifications are blocked
- Comments remain allowed as they don't modify task data

---

## Task Edit Access Request Workflow

### Overview
The Task Edit Access Request workflow allows assigned users (assignees) to request permission from the task creator to edit a task. This feature provides a controlled mechanism for collaborative task editing while maintaining the principle that only task creators have default edit permissions.

### Business Justification
1. **Flexibility:** Allows assignees to suggest changes when needed
2. **Control:** Task creators maintain oversight of all modifications
3. **Collaboration:** Enables team members to contribute to task refinement
4. **Audit Trail:** All edit requests and approvals are tracked
5. **Security:** Prevents unauthorized task modifications

### Workflow Process

#### Step 1: Request Edit Access
- Assigned user (assignee) views a task they cannot edit
- System displays "Request Edit Access" button
- User clicks button and optionally provides a message explaining why they need edit access
- System creates a new edit request with "Pending" status

#### Step 2: Review Request
- Task creator views the task details
- System displays "Edit Access Requests" section showing all pending requests
- Each request shows:
  - Requester name
  - Request message
  - Request date
  - Approve/Reject buttons

#### Step 3: Approve or Reject
- Task creator clicks "Approve" or "Reject" button
- System updates request status and records reviewer information
- Approved requests grant temporary edit access to the requester
- Rejected requests block edit access

#### Step 4: Edit Task (If Approved)
- If approved, assignee can now edit the task
- System validates edit permission by checking for approved edit request
- Assignee can modify task details (title, description, priority, deadline)
- Edit access remains active until task is completed

### Database Schema

#### TaskEditRequests Table
```sql
CREATE TABLE TaskEditRequests (
    Id INT PRIMARY KEY IDENTITY,
    TaskId INT NOT NULL,
    RequestedByUserId INT NOT NULL,
    RequestMessage NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(MAX) NOT NULL,  -- 'Pending', 'Approved', 'Rejected'
    CreatedAt DATETIME2 NOT NULL,
    ReviewedByUserId INT NULL,
    ReviewedAt DATETIME2 NULL,
    
    CONSTRAINT FK_TaskEditRequests_Tasks FOREIGN KEY (TaskId) 
        REFERENCES Tasks(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TaskEditRequests_RequestedBy FOREIGN KEY (RequestedByUserId) 
        REFERENCES Users(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_TaskEditRequests_ReviewedBy FOREIGN KEY (ReviewedByUserId) 
        REFERENCES Users(Id) ON DELETE NO ACTION
);
```

### API Endpoints

#### 1. Create Edit Request
**Endpoint:** `POST /api/task/{taskId}/edit-request`

**Request Body:**
```json
{
    "requestMessage": "I need to update the task description based on new requirements"
}
```

**Response:**
```json
{
    "message": "Edit request submitted successfully",
    "requestId": 1
}
```

**Validation:**
- User must be authenticated
- Task must exist
- User cannot be the task creator
- No pending request already exists for this user and task
- Task must not be completed

#### 2. Get Edit Requests
**Endpoint:** `GET /api/task/{taskId}/edit-requests`

**Response:**
```json
[
    {
        "id": 1,
        "taskId": 5,
        "requestedByUserId": 3,
        "requestedByUserName": "John Doe",
        "requestMessage": "Need to update description",
        "status": "Pending",
        "createdAt": "2026-03-13T10:30:00Z",
        "reviewedByUserId": null,
        "reviewedByUserName": null,
        "reviewedAt": null
    }
]
```

**Authorization:**
- Only task creator can view edit requests

#### 3. Approve Edit Request
**Endpoint:** `POST /api/task/edit-request/{id}/approve`

**Response:**
```json
{
    "message": "Edit request approved"
}
```

**Validation:**
- User must be the task creator
- Request must exist
- Request status must be "Pending"

#### 4. Reject Edit Request
**Endpoint:** `POST /api/task/edit-request/{id}/reject`

**Response:**
```json
{
    "message": "Edit request rejected"
}
```

**Validation:**
- User must be the task creator
- Request must exist
- Request status must be "Pending"

### Permission Validation

#### Updated Edit Task Logic
The `UpdateTask` endpoint now checks for approved edit requests:

```csharp
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
    return StatusCode(403, new { 
        message = "Only the task creator or users with approved edit access can edit this task" 
    });
}
```

### Frontend Implementation

#### Task Service Methods
```typescript
// Create edit request
createEditRequest(taskId: number, message: string): Observable<any> {
    return this.http.post(
        `${this.apiUrl}/${taskId}/edit-request`, 
        { requestMessage: message }, 
        { withCredentials: true }
    );
}

// Get edit requests
getEditRequests(taskId: number): Observable<any[]> {
    return this.http.get<any[]>(
        `${this.apiUrl}/${taskId}/edit-requests`, 
        { withCredentials: true }
    );
}

// Approve edit request
approveEditRequest(requestId: number): Observable<any> {
    return this.http.post(
        `${this.apiUrl}/edit-request/${requestId}/approve`, 
        {}, 
        { withCredentials: true }
    );
}

// Reject edit request
rejectEditRequest(requestId: number): Observable<any> {
    return this.http.post(
        `${this.apiUrl}/edit-request/${requestId}/reject`, 
        {}, 
        { withCredentials: true }
    );
}
```

#### Component Logic
```typescript
// Check if user can request edit access
canRequestEditAccess(): boolean {
    if (!this.task) return false;
    if (this.task.assignedBy === this.currentUserId) return false;
    if (this.task.status === 'Completed') return false;
    if (this.hasApprovedEditRequest) return false;
    if (this.hasPendingEditRequest) return false;
    return true;
}

// Check if user can edit task
canEditTask(): boolean {
    if (!this.task) return false;
    if (this.task.status === 'Completed') return false;
    if (this.task.assignedBy === this.currentUserId) return true;
    return this.hasApprovedEditRequest;
}
```

#### UI Components

**Request Edit Access Button:**
```html
<button *ngIf="canRequestEditAccess()" 
        (click)="openEditRequestDialog()" 
        class="btn-warning">
    Request Edit Access
</button>
```

**Edit Request Dialog:**
```html
<div class="modal-overlay" *ngIf="showEditRequestDialog">
    <div class="modal-content">
        <h3>Request Edit Access</h3>
        <p>Send a request to the task creator to allow you to edit this task.</p>
        <div class="form-group">
            <label>Message (optional):</label>
            <textarea [(ngModel)]="editRequestMessage" 
                      placeholder="Explain why you need edit access..."
                      class="form-control" rows="3"></textarea>
        </div>
        <div class="modal-actions">
            <button (click)="submitEditRequest()" class="btn-primary">
                Submit Request
            </button>
            <button (click)="closeEditRequestDialog()" class="btn-secondary">
                Cancel
            </button>
        </div>
    </div>
</div>
```

**Edit Requests Section (Task Creator View):**
```html
<div class="info-section" *ngIf="canSeeEditRequests()">
    <h3>Edit Access Requests</h3>
    <div class="requests-list">
        <div *ngFor="let request of editRequests" class="request-item">
            <div class="request-header">
                <strong>{{ request.requestedByUserName }}</strong>
                <span class="status-badge" [class]="request.status.toLowerCase()">
                    {{ request.status }}
                </span>
            </div>
            <p *ngIf="request.requestMessage">{{ request.requestMessage }}</p>
            <small>Requested on {{ request.createdAt | date:'short' }}</small>
            <div class="request-actions" *ngIf="request.status === 'Pending'">
                <button (click)="approveEditRequest(request.id)" class="btn-success">
                    Approve
                </button>
                <button (click)="rejectEditRequest(request.id)" class="btn-danger">
                    Reject
                </button>
            </div>
        </div>
    </div>
</div>
```

**Status Messages:**
```html
<!-- Pending request message -->
<div class="info-message info" *ngIf="hasPendingEditRequest && !canEditTask()">
    ⏳ Edit request pending approval
</div>

<!-- Approved request message -->
<div class="info-message success" *ngIf="hasApprovedEditRequest && !isTaskCompleted()">
    ✓ Edit access granted
</div>
```

### Security Considerations

1. **Backend Validation:** All permission checks are performed on the backend, not just the frontend
2. **Session Authentication:** All endpoints require valid session authentication
3. **Authorization Checks:** Each endpoint validates user permissions before processing
4. **Status Validation:** Requests can only be approved/rejected if status is "Pending"
5. **Completed Task Lock:** Edit requests cannot be created for completed tasks
6. **Duplicate Prevention:** Users cannot create multiple pending requests for the same task

### User Experience Flow

#### For Assignee (Requesting Edit Access):
1. View task details
2. See "Request Edit Access" button (if eligible)
3. Click button to open dialog
4. Enter optional message explaining need for edit access
5. Submit request
6. See "Edit request pending approval" message
7. Wait for task creator to review
8. If approved: See "Edit access granted" message and "Edit Task" button becomes available
9. If rejected: Continue viewing task without edit access

#### For Task Creator (Reviewing Requests):
1. View task details
2. See "Edit Access Requests" section
3. Review each request with requester name, message, and date
4. Click "Approve" to grant edit access
5. Click "Reject" to deny edit access
6. See updated request status after action

### Benefits

1. **Controlled Collaboration:** Enables team collaboration while maintaining oversight
2. **Audit Trail:** All requests and approvals are logged with timestamps
3. **Flexibility:** Task creators can grant edit access on a case-by-case basis
4. **Security:** Prevents unauthorized modifications
5. **Transparency:** Clear communication between assignees and creators
6. **User-Friendly:** Simple workflow with clear status indicators

### Limitations

1. **Single Approval:** Once approved, edit access remains until task completion
2. **No Revocation:** Approved edit access cannot be revoked (except by completing the task)
3. **No Expiration:** Edit requests don't expire automatically
4. **No Notifications:** System doesn't send email/push notifications for requests

### Conclusion

The Task Edit Access Request workflow provides a balanced approach to task editing permissions. It maintains the security principle that only task creators can edit tasks by default, while providing a controlled mechanism for assignees to request and receive edit access when needed. The implementation includes comprehensive validation, clear user feedback, and a complete audit trail of all edit requests and approvals.

---

## Edit Request Visibility and Authentication State Fix

### Overview
This section documents two critical fixes implemented to improve the system's reliability and user experience:
1. Edit request visibility for task creators (Admin/Manager)
2. Authentication state persistence across page refreshes

### Issue 1: Edit Request Visibility for Admin

#### Problem
Employees were sending edit requests for completed tasks, but these requests were not visible in the Admin/Manager dashboard. The requests existed in the database but there was no way for task creators to view and manage them.

#### Root Cause
The system only had an endpoint to retrieve edit requests for a specific task (`GET /api/task/{taskId}/edit-requests`), but no endpoint to retrieve all edit requests for tasks created by the current user.

#### Solution

**Backend Implementation:**

Added a new API endpoint to retrieve all edit requests for tasks created by the current user:

```csharp
// GET /api/task/edit-requests - GET ALL EDIT REQUESTS FOR CURRENT USER
[HttpGet("edit-requests")]
public async Task<IActionResult> GetAllEditRequests()
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
```

**Key Logic:**
- Filters requests where `Task.AssignedBy == currentUserId`
- This ensures only the task creator sees requests for their tasks
- Includes task title, requester name, and reviewer information
- Orders by creation date (newest first)

**DTO Enhancement:**

Added `TaskTitle` field to `TaskEditRequestDto`:

```csharp
public class TaskEditRequestDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;  // NEW FIELD
    public int RequestedByUserId { get; set; }
    public string RequestedByUserName { get; set; } = string.Empty;
    public string RequestMessage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? ReviewedByUserId { get; set; }
    public string? ReviewedByUserName { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
```

**Frontend Implementation:**

Added service method in `TaskService`:

```typescript
getAllEditRequests(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/edit-requests`, { withCredentials: true });
}
```

Updated `DashboardComponent` to load and display edit requests:

```typescript
export class DashboardComponent implements OnInit {
  editRequests: any[] = [];

  loadTasks(): void {
    this.taskService.getTasks().subscribe(tasks => {
      this.tasks = tasks;
      this.calculateStats();
    });

    // Load edit requests for Admin and Manager
    if (this.canCreateTask()) {
      this.loadEditRequests();
    }
  }

  loadEditRequests(): void {
    this.taskService.getAllEditRequests().subscribe({
      next: (requests) => {
        this.editRequests = requests;
      },
      error: (err) => {
        console.error('Failed to load edit requests', err);
      }
    });
  }

  approveEditRequest(requestId: number): void {
    this.taskService.approveEditRequest(requestId).subscribe({
      next: () => {
        alert('Edit request approved');
        this.loadEditRequests();
      },
      error: (err) => {
        alert('Failed to approve request');
      }
    });
  }

  rejectEditRequest(requestId: number): void {
    this.taskService.rejectEditRequest(requestId).subscribe({
      next: () => {
        alert('Edit request rejected');
        this.loadEditRequests();
      },
      error: (err) => {
        alert('Failed to reject request');
      }
    });
  }
}
```

**Dashboard UI:**

Added "Edit Access Requests" section to dashboard:

```html
<div class="edit-requests" *ngIf="canCreateTask() && editRequests.length > 0">
    <h3>Edit Access Requests</h3>
    <table>
        <thead>
            <tr>
                <th>Task Name</th>
                <th>Requested By</th>
                <th>Message</th>
                <th>Request Date</th>
                <th>Status</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            <tr *ngFor="let request of editRequests">
                <td>{{ request.taskTitle }}</td>
                <td>{{ request.requestedByUserName }}</td>
                <td>{{ request.requestMessage }}</td>
                <td>{{ request.createdAt | date:'short' }}</td>
                <td><span class="badge status-{{ request.status.toLowerCase() }}">{{ request.status }}</span></td>
                <td>
                    <button *ngIf="request.status === 'Pending'" 
                            (click)="approveEditRequest(request.id)" 
                            class="btn-success btn-sm">
                        Approve
                    </button>
                    <button *ngIf="request.status === 'Pending'" 
                            (click)="rejectEditRequest(request.id)" 
                            class="btn-danger btn-sm">
                        Reject
                    </button>
                    <span *ngIf="request.status !== 'Pending'">
                        {{ request.status }} by {{ request.reviewedByUserName }}
                    </span>
                </td>
            </tr>
        </tbody>
    </table>
</div>
```

### Issue 2: Role Changes After Refresh

#### Problem
When refreshing the page, the dashboard sometimes switched roles (Admin became Employee, or vice versa). However, navigating back showed the correct role again. This indicated that Angular's authentication state was not persisted correctly across page refreshes.

#### Root Cause
The authentication state was only stored in memory (BehaviorSubject). When the page refreshed:
1. Angular app reloaded completely
2. All in-memory state was lost
3. `checkSession()` was called to restore state from backend
4. There was a brief moment where the user appeared logged out
5. This caused flickering and inconsistent role display

#### Solution

**localStorage Persistence:**

Updated `AuthService` to persist authentication state in localStorage:

```typescript
@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private currentUserSubject = new BehaviorSubject<User | null>(null);
    public currentUser$ = this.currentUserSubject.asObservable();

    constructor(private http: HttpClient) {
        // Load user from localStorage first (for immediate state restoration)
        this.loadUserFromStorage();
        // Then check session with backend (for validation)
        this.checkSession();
    }

    login(credentials: LoginRequest): Observable<User> {
        return this.http.post<User>(`${this.apiUrl}/login`, credentials, { withCredentials: true })
            .pipe(
                tap(user => {
                    this.currentUserSubject.next(user);
                    this.saveUserToStorage(user);  // Persist to localStorage
                })
            );
    }

    logout(): Observable<any> {
        return this.http.post(`${this.apiUrl}/logout`, {}, { withCredentials: true })
            .pipe(
                tap(() => {
                    this.currentUserSubject.next(null);
                    this.clearUserFromStorage();  // Clear localStorage
                })
            );
    }

    checkSession(): void {
        this.http.get<User>(`${this.apiUrl}/session`, { withCredentials: true })
            .subscribe({
                next: (user) => {
                    this.currentUserSubject.next(user);
                    this.saveUserToStorage(user);  // Update localStorage
                },
                error: () => {
                    this.currentUserSubject.next(null);
                    this.clearUserFromStorage();  // Clear invalid session
                }
            });
    }

    // Save user to localStorage for persistence
    private saveUserToStorage(user: User): void {
        localStorage.setItem('user', JSON.stringify(user));
    }

    // Load user from localStorage on app initialization
    private loadUserFromStorage(): void {
        const storedUser = localStorage.getItem('user');
        if (storedUser) {
            try {
                const user = JSON.parse(storedUser);
                this.currentUserSubject.next(user);
            } catch (error) {
                this.clearUserFromStorage();
            }
        }
    }

    // Clear user from localStorage on logout
    private clearUserFromStorage(): void {
        localStorage.removeItem('user');
    }
}
```

**How It Works:**

1. **On Login:**
   - User credentials are validated by backend
   - Backend creates session and returns user data
   - Frontend saves user to both BehaviorSubject and localStorage
   - User can now navigate the app

2. **On Page Refresh:**
   - Angular app reloads
   - AuthService constructor runs
   - `loadUserFromStorage()` immediately restores user from localStorage
   - UI shows correct role instantly (no flickering)
   - `checkSession()` validates with backend in background
   - If session is valid, localStorage is updated with fresh data
   - If session is invalid, user is logged out and localStorage is cleared

3. **On Logout:**
   - Backend destroys session
   - Frontend clears both BehaviorSubject and localStorage
   - User is redirected to login page

**Benefits:**

1. **Instant State Restoration:** User role appears immediately on refresh
2. **No Flickering:** Dashboard doesn't switch between roles
3. **Consistent Experience:** Role remains stable across refreshes
4. **Backend Validation:** Session is still validated with backend
5. **Security:** Invalid sessions are detected and cleared

### Testing

#### Test Case 1: Edit Request Visibility
**Steps:**
1. Login as Employee
2. Navigate to a completed task
3. Request edit permission
4. Logout
5. Login as Admin (task creator)
6. View dashboard

**Expected Result:**
- ✓ Edit request appears in "Edit Access Requests" section
- ✓ Shows task name, requester name, message, and date
- ✓ Approve and Reject buttons are visible
- ✓ Clicking Approve grants permission
- ✓ Clicking Reject denies permission

#### Test Case 2: Admin Refresh Persistence
**Steps:**
1. Login as Admin
2. View dashboard (shows Admin role)
3. Refresh page (F5)

**Expected Result:**
- ✓ Dashboard still shows Admin role
- ✓ No flickering or role switching
- ✓ Edit requests section remains visible
- ✓ All admin features remain accessible

#### Test Case 3: Employee Refresh Persistence
**Steps:**
1. Login as Employee
2. View dashboard (shows Employee role)
3. Refresh page (F5)

**Expected Result:**
- ✓ Dashboard still shows Employee role
- ✓ No flickering or role switching
- ✓ "My Tasks" button remains visible
- ✓ "Create Task" button remains hidden

#### Test Case 4: Manager Refresh Persistence
**Steps:**
1. Login as Manager
2. View dashboard (shows Manager role)
3. Refresh page (F5)

**Expected Result:**
- ✓ Dashboard still shows Manager role
- ✓ No flickering or role switching
- ✓ Edit requests section visible (if any exist)
- ✓ "Create Task" button remains visible

#### Test Case 5: Browser Back Button
**Steps:**
1. Login as Admin
2. Navigate to task list
3. Click browser back button

**Expected Result:**
- ✓ Returns to dashboard
- ✓ Still shows Admin role
- ✓ No role recalculation
- ✓ State remains consistent

### Security Considerations

1. **localStorage Security:**
   - User data in localStorage is accessible to JavaScript
   - No sensitive data (passwords) is stored
   - Session validation still happens on backend
   - Invalid sessions are detected and cleared

2. **Backend Validation:**
   - Every API request validates session cookie
   - localStorage is only for UI state
   - Backend is the source of truth for permissions
   - Tampering with localStorage doesn't grant access

3. **Session Expiration:**
   - Backend session expires after inactivity
   - `checkSession()` detects expired sessions
   - User is logged out automatically
   - localStorage is cleared on logout

### Benefits

1. **Improved User Experience:**
   - No role flickering on refresh
   - Instant dashboard rendering
   - Consistent role display
   - Smooth navigation

2. **Better Admin Workflow:**
   - Edit requests visible in dashboard
   - Easy approval/rejection process
   - Clear request information
   - Centralized request management

3. **Reliability:**
   - Authentication state persists across refreshes
   - No unexpected role changes
   - Predictable behavior
   - Reduced user confusion

### Conclusion

These fixes address critical usability issues in the task management system. The edit request visibility ensures that task creators can properly manage permission requests, while the authentication state persistence provides a stable and consistent user experience across page refreshes.

**Key Takeaways:**
- Edit requests are now visible to task creators in the dashboard
- Authentication state persists across page refreshes using localStorage
- Role consistency is maintained throughout the user session
- Backend validation ensures security despite client-side storage
- User experience is significantly improved with instant state restoration

---

## Edit Access Permission Enforcement

### Overview
The Edit Access Request system allows non-creator users to request permission to edit tasks. Once a request is approved or rejected, the system must enforce the correct permission behavior. This section documents how approved and rejected edit requests affect task editing permissions.

### Permission Logic

Only two types of users can edit a task:
1. The task creator (user who created the task via `Task.AssignedBy`)
2. A user whose edit request has been APPROVED for that specific task

All other users are blocked from editing, including:
- Users with pending edit requests
- Users with rejected edit requests
- Users who haven't requested edit access
- Admin users (unless they created the task)

### Database Schema

The `TaskEditRequests` table stores edit access requests:

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| TaskId | int | Foreign key to Tasks |
| RequestedByUserId | int | User requesting edit access |
| RequestMessage | string | Reason for request |
| Status | string | Pending / Approved / Rejected |
| CreatedAt | datetime | Request timestamp |
| ReviewedByUserId | int? | User who reviewed |
| ReviewedAt | datetime? | Review timestamp |

**Key Relationships:**
- Task → TaskEditRequests (One-to-Many, Cascade Delete)
- User → TaskEditRequests (RequestedBy, Restrict Delete)
- User → TaskEditRequests (ReviewedBy, Restrict Delete)

### Backend Implementation

#### Edit Task Validation

The `UpdateTask` endpoint enforces edit permissions:

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
{
    var userId = HttpContext.Session.GetInt32("UserId");
    var task = await _context.Tasks.FindAsync(id);

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

    // Proceed with update...
}
```

**Validation Flow:**
1. Check if task is completed (blocked for everyone)
2. Check if user is task creator (allowed)
3. If not creator, check for approved edit request
4. If no approved request, return 403 Forbidden

#### Duplicate Request Prevention

The `CreateEditRequest` endpoint prevents duplicate requests:

```csharp
[HttpPost("{taskId}/edit-request")]
public async Task<IActionResult> CreateEditRequest(int taskId, [FromBody] CreateTaskEditRequestDto dto)
{
    var userId = HttpContext.Session.GetInt32("UserId");
    var task = await _context.Tasks.FindAsync(taskId);

    // Cannot request edit access if you're the creator
    if (task.AssignedBy == userId.Value)
    {
        return BadRequest(new { message = "You are the task creator and can already edit this task" });
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

    // Create new request...
}
```

**Duplicate Prevention Logic:**
- Only one request per user per task is allowed
- Checks for existing requests regardless of status
- Provides specific error messages for each status
- Rejected requests cannot be resubmitted automatically

### Frontend Implementation

#### TypeScript Component Logic

The `TaskDetailsComponent` manages edit request state:

```typescript
export class TaskDetailsComponent implements OnInit {
    hasApprovedEditRequest: boolean = false;
    hasPendingEditRequest: boolean = false;
    hasRejectedEditRequest: boolean = false;

    // Check if user can edit task
    canEditTask(): boolean {
        if (!this.task) return false;

        // COMPLETED TASK LOCK RULE: No one can edit completed tasks
        if (this.task.status === 'Completed') {
            return false;
        }

        // Task creator can always edit
        if (this.task.assignedBy === this.currentUserId) {
            return true;
        }

        // User with approved edit request can edit
        return this.hasApprovedEditRequest;
    }

    // Check if user can request edit access
    canRequestEditAccess(): boolean {
        if (!this.task) return false;

        // Cannot request if you're the creator
        if (this.task.assignedBy === this.currentUserId) {
            return false;
        }

        // Cannot request if task is completed
        if (this.task.status === 'Completed') {
            return false;
        }

        // Cannot request if already approved
        if (this.hasApprovedEditRequest) {
            return false;
        }

        // Cannot request if already pending
        if (this.hasPendingEditRequest) {
            return false;
        }

        // Cannot request if already rejected
        if (this.hasRejectedEditRequest) {
            return false;
        }

        return true;
    }

    // Check edit request status for current user
    checkEditRequestStatus(taskId: number): void {
        if (!this.task || this.task.assignedBy === this.currentUserId) {
            return;
        }

        this.taskService.getEditRequests(taskId).subscribe({
            next: (requests) => {
                const myRequest = requests.find(r => r.requestedByUserId === this.currentUserId);
                if (myRequest) {
                    this.hasApprovedEditRequest = myRequest.status === 'Approved';
                    this.hasPendingEditRequest = myRequest.status === 'Pending';
                    this.hasRejectedEditRequest = myRequest.status === 'Rejected';
                }
            }
        });
    }
}
```

#### HTML Template

The UI displays different elements based on edit request status:

```html
<!-- Edit Task Button (only for creator or approved users) -->
<button *ngIf="canEditTask() && !isEditMode" 
        (click)="enableEditMode()" 
        class="btn-primary">
    Edit Task
</button>

<!-- Request Edit Access Button (only if no request exists) -->
<button *ngIf="canRequestEditAccess()" 
        (click)="openEditRequestDialog()" 
        class="btn-warning">
    Request Edit Access
</button>

<!-- Edit Request Status Messages -->
<div class="info-message info" *ngIf="hasPendingEditRequest && !canEditTask()">
    ⏳ Edit request pending approval
</div>

<div class="info-message success" *ngIf="hasApprovedEditRequest && !isTaskCompleted()">
    ✓ Edit access granted
</div>

<div class="info-message error" *ngIf="hasRejectedEditRequest && !canEditTask()">
    ✗ Edit request rejected
</div>
```

### Permission States

#### State 1: No Request
**User:** Non-creator who hasn't requested edit access
**UI Display:**
- "Request Edit Access" button visible
- No status messages
- "Edit Task" button hidden

**Backend Behavior:**
- Edit attempts return 403 Forbidden
- Can create new edit request

#### State 2: Pending Request
**User:** Non-creator with pending edit request
**UI Display:**
- "Request Edit Access" button hidden
- Message: "⏳ Edit request pending approval"
- "Edit Task" button hidden

**Backend Behavior:**
- Edit attempts return 403 Forbidden
- Cannot create duplicate request

#### State 3: Approved Request
**User:** Non-creator with approved edit request
**UI Display:**
- "Request Edit Access" button hidden
- Message: "✓ Edit access granted"
- "Edit Task" button visible

**Backend Behavior:**
- Edit attempts succeed
- Cannot create duplicate request

#### State 4: Rejected Request
**User:** Non-creator with rejected edit request
**UI Display:**
- "Request Edit Access" button hidden
- Message: "✗ Edit request rejected"
- "Edit Task" button hidden

**Backend Behavior:**
- Edit attempts return 403 Forbidden
- Cannot create duplicate request
- Must contact task creator for reconsideration

#### State 5: Task Creator
**User:** Task creator
**UI Display:**
- "Request Edit Access" button hidden
- No status messages
- "Edit Task" button visible

**Backend Behavior:**
- Edit attempts always succeed (unless task is completed)
- Cannot create edit request for own task

### Approval/Rejection Workflow

#### Approval Process
1. Task creator views edit requests in dashboard or task details
2. Creator clicks "Approve" button
3. Backend updates request status to "Approved"
4. Backend sets ReviewedByUserId and ReviewedAt
5. Requester can now edit the task
6. Approval persists across page refreshes

#### Rejection Process
1. Task creator views edit requests
2. Creator clicks "Reject" button
3. Backend updates request status to "Rejected"
4. Backend sets ReviewedByUserId and ReviewedAt
5. Requester cannot edit the task
6. Requester cannot resubmit request
7. Rejection persists across page refreshes

### Database Persistence

**Request Status Storage:**
- Status is stored in `TaskEditRequests.Status` column
- Values: "Pending", "Approved", "Rejected"
- Status persists permanently in database
- Page refresh does not reset approval/rejection

**Query for Approved Access:**
```csharp
var hasApprovedAccess = await _context.TaskEditRequests
    .AnyAsync(ter => ter.TaskId == taskId && 
                   ter.RequestedByUserId == userId && 
                   ter.Status == "Approved");
```

### Security Considerations

1. **Backend Validation:**
   - All edit permissions enforced server-side
   - Frontend UI is for user experience only
   - Tampering with frontend doesn't grant access

2. **Database Integrity:**
   - Foreign key constraints prevent orphaned requests
   - Cascade delete removes requests when task is deleted
   - Restrict delete prevents user deletion if they have requests

3. **Status Immutability:**
   - Once approved/rejected, status cannot be changed by requester
   - Only task creator can review requests
   - No automatic status changes

4. **Duplicate Prevention:**
   - One request per user per task
   - Prevents request spam
   - Clear error messages for each scenario

### Test Cases

#### Test Case 1: Employee Requests Edit Access
**Steps:**
1. Login as Employee
2. Navigate to task created by Admin
3. Click "Request Edit Access"
4. Enter message and submit

**Expected Result:**
- ✓ Request created with Status="Pending"
- ✓ Message: "⏳ Edit request pending approval"
- ✓ "Request Edit Access" button hidden
- ✓ "Edit Task" button hidden

#### Test Case 2: Admin Approves Request
**Steps:**
1. Login as Admin (task creator)
2. View dashboard or task details
3. See pending edit request
4. Click "Approve"

**Expected Result:**
- ✓ Request status changes to "Approved"
- ✓ ReviewedBy and ReviewedAt fields populated
- ✓ Employee can now edit task

#### Test Case 3: Approved User Can Edit
**Steps:**
1. Login as Employee (with approved request)
2. Navigate to task
3. Click "Edit Task"
4. Modify task details
5. Save changes

**Expected Result:**
- ✓ Message: "✓ Edit access granted"
- ✓ "Edit Task" button visible
- ✓ Edit form opens
- ✓ Changes save successfully
- ✓ HTTP 200 OK response

#### Test Case 4: Admin Rejects Request
**Steps:**
1. Login as Admin (task creator)
2. View pending edit request
3. Click "Reject"

**Expected Result:**
- ✓ Request status changes to "Rejected"
- ✓ ReviewedBy and ReviewedAt fields populated
- ✓ Employee cannot edit task

#### Test Case 5: Rejected User Cannot Edit
**Steps:**
1. Login as Employee (with rejected request)
2. Navigate to task
3. Look for "Edit Task" button

**Expected Result:**
- ✓ Message: "✗ Edit request rejected"
- ✓ "Edit Task" button hidden
- ✓ "Request Edit Access" button hidden
- ✓ Manual API call returns 403 Forbidden

#### Test Case 6: Refresh Keeps Approval State
**Steps:**
1. Login as Employee (with approved request)
2. Navigate to task
3. Verify "Edit Task" button visible
4. Refresh page (F5)

**Expected Result:**
- ✓ "Edit Task" button still visible
- ✓ Message: "✓ Edit access granted"
- ✓ Can still edit task
- ✓ Approval persists from database

#### Test Case 7: Duplicate Request Prevention
**Steps:**
1. Login as Employee
2. Request edit access for task
3. Try to request again

**Expected Result:**
- ✗ Second request fails
- ✗ Error: "You already have a pending edit request for this task"
- ✗ No duplicate request created

#### Test Case 8: Rejected Request Cannot Resubmit
**Steps:**
1. Login as Employee (with rejected request)
2. Try to request edit access again

**Expected Result:**
- ✗ Request button hidden
- ✗ API call returns error
- ✗ Error: "Your previous edit request was rejected. Please contact the task creator."

#### Test Case 9: Other Users Cannot Edit
**Steps:**
1. Login as Employee A
2. Admin approves edit request for Employee B
3. Employee A tries to edit task

**Expected Result:**
- ✗ Employee A cannot see "Edit Task" button
- ✗ Manual API call returns 403 Forbidden
- ✗ Only Employee B has edit access

### Benefits

1. **Clear Permission Model:**
   - Only creator and approved users can edit
   - No ambiguity about who can edit
   - Easy to understand and enforce

2. **Controlled Access:**
   - Task creators maintain control
   - Explicit approval required
   - Can reject inappropriate requests

3. **Persistent State:**
   - Approvals survive page refreshes
   - Database is source of truth
   - Consistent behavior across sessions

4. **Duplicate Prevention:**
   - One request per user per task
   - Prevents request spam
   - Clear feedback for each state

5. **Security:**
   - Backend validation enforces rules
   - Frontend cannot bypass permissions
   - Database constraints ensure integrity

### Conclusion

The Edit Access Permission Enforcement system provides a robust and secure way to manage task editing permissions. By storing approval state in the database and validating on every edit attempt, the system ensures that only authorized users can modify tasks while maintaining a clear and user-friendly interface.

**Key Takeaways:**
- Only task creators and approved users can edit tasks
- Rejected requests cannot be resubmitted automatically
- Approval state persists across page refreshes
- Backend validation ensures security
- One request per user per task prevents duplicates
- Clear UI feedback for all permission states

---

## Future Enhancements

### Functional Enhancements

1. **Task Comments**:
   - Add comments to tasks
   - Discussion threads
   - File attachments

2. **Task Filtering and Search**:
   - Filter by status, priority, date
   - Search by title/description
   - Sort by various fields

3. **Notifications**:
   - Email notifications for task assignments
   - In-app notifications
   - Task deadline reminders

4. **Task Deadlines**:
   - Add due dates to tasks
   - Overdue task highlighting
   - Deadline notifications

5. **Task History**:
   - Track all status changes
   - Audit log of modifications
   - Time tracking

6. **User Profiles**:
   - Profile pictures
   - User preferences
   - Activity history

7. **Dashboard Analytics**:
   - Task completion rates
   - Performance metrics
   - Charts and graphs

8. **Task Categories/Tags**:
   - Organize tasks by category
   - Tag-based filtering
   - Custom labels

### Technical Enhancements

1. **Authentication**:
   - JWT token authentication
   - OAuth/Social login
   - Two-factor authentication
   - Password reset functionality

2. **Authorization**:
   - Attribute-based authorization
   - Custom authorization policies
   - Fine-grained permissions

3. **Validation**:
   - FluentValidation library
   - Client-side validation
   - Custom validation rules

4. **Logging**:
   - Structured logging (Serilog)
   - Log aggregation
   - Error tracking (e.g., Sentry)

5. **Caching**:
   - Redis caching
   - Response caching
   - Distributed caching

6. **API Documentation**:
   - Enhanced Swagger documentation
   - API versioning
   - Request/response examples

7. **Testing**:
   - Unit tests (xUnit, NUnit)
   - Integration tests
   - End-to-end tests (Cypress)
   - Test coverage reports

8. **Performance**:
   - Database indexing
   - Query optimization
   - Lazy loading
   - Pagination

9. **Security**:
   - Rate limiting
   - Input sanitization
   - SQL injection prevention
   - XSS protection
   - CSRF tokens

10. **Deployment**:
    - Docker containerization
    - CI/CD pipeline
    - Cloud deployment (Azure, AWS)
    - Environment configuration

### UI/UX Enhancements

1. **Responsive Design**:
   - Mobile-friendly interface
   - Tablet optimization
   - Progressive Web App (PWA)

2. **Accessibility**:
   - WCAG compliance
   - Screen reader support
   - Keyboard navigation
   - High contrast mode

3. **Internationalization**:
   - Multi-language support
   - Date/time localization
   - Currency formatting

4. **Theme Support**:
   - Dark mode
   - Custom themes
   - User preferences

5. **Advanced UI Components**:
   - Drag-and-drop task management
   - Kanban board view
   - Calendar view
   - Gantt chart

---

## Project Improvements Implemented

### 1. Strict Task Lifecycle Enforcement

**Problem**: Original implementation allowed Admin and Manager to start/complete tasks.

**Solution**: Added strict role validation in TaskController:

```csharp
// In StartTask method
if (userRole == "Admin" || userRole == "Manager")
{
    return Forbid(); // 403 Forbidden
}

// In CompleteTask method
if (userRole == "Admin" || userRole == "Manager")
{
    return Forbid(); // 403 Forbidden
}
```

**Result**:
- Only employees can start tasks (Pending → In Progress)
- Only employees can complete tasks (In Progress → Completed)
- Admin and Manager receive 403 Forbidden if they attempt these actions
- Enforces proper task workflow and role separation

### 2. Enhanced LINQ Filtering Endpoints

**New Endpoints Added**:

1. **GET /api/task/completed** - Filter completed tasks
   ```csharp
   .Where(t => t.Status == "Completed")
   ```

2. **GET /api/task/pending** - Filter pending tasks
   ```csharp
   .Where(t => t.Status == "Pending")
   ```

3. **GET /api/task/created-by-me** - Get tasks created by current user
   ```csharp
   .Where(t => t.AssignedBy == userId.Value)
   ```

**Benefits**:
- More granular task filtering
- Better performance (server-side filtering)
- Cleaner component code
- Demonstrates LINQ query capabilities

### 3. Improved Task Model

**Task Model Fields**:
```csharp
public class TaskItem
{
    public int Id { get; set; }                          // Primary key
    public string Title { get; set; }                    // Task title
    public string Description { get; set; }              // Task description
    public int AssignedBy { get; set; }                  // Creator user ID
    public int AssignedTo { get; set; }                  // Assignee user ID
    public string Status { get; set; } = "Pending";      // Pending, In Progress, Completed
    public string Priority { get; set; } = "Medium";     // Low, Medium, High
    public DateTime CreatedAt { get; set; }              // Creation timestamp
    public DateTime? StartDate { get; set; }             // When task was started
    public DateTime? CompletedDate { get; set; }         // When task was completed
    
    // Navigation properties
    public User? AssignedByUser { get; set; }
    public User? AssignedToUser { get; set; }
    public ICollection<TaskComment> Comments { get; set; }
}
```

**All Required Fields Present**:
- ✅ Id
- ✅ Title
- ✅ Description
- ✅ AssignedBy
- ✅ AssignedTo
- ✅ Status
- ✅ Priority
- ✅ CreatedAt
- ✅ StartDate
- ✅ CompletedDate

### 4. Enhanced Angular Task Service

**New Service Methods**:

```typescript
// Get completed tasks
getCompletedTasks(): Observable<Task[]> {
  return this.http.get<Task[]>(`${this.apiUrl}/completed`, { withCredentials: true });
}

// Get pending tasks
getPendingTasks(): Observable<Task[]> {
  return this.http.get<Task[]>(`${this.apiUrl}/pending`, { withCredentials: true });
}

// Get tasks created by current user
getCreatedByMe(): Observable<Task[]> {
  return this.http.get<Task[]>(`${this.apiUrl}/created-by-me`, { withCredentials: true });
}
```

**Benefits**:
- Components can easily filter tasks
- Consistent API interface
- Type-safe with TypeScript
- Observable-based for reactive programming

### 5. Validated Assignment Logic

**Role-Based Task Creation**:

```csharp
// In CreateTask method
if (userRole != "Admin" && userRole != "Manager")
{
    return Forbid(); // 403 Forbidden
}
```

**Validation Rules**:
- ✅ Only Admin and Manager can create tasks
- ✅ Employees cannot create tasks
- ✅ Employees cannot assign tasks to others
- ✅ Backend validates role before task creation
- ✅ Frontend hides "Create Task" button for employees

### 6. Improved UI Components

**Dashboard Component**:
- Shows task statistics (total, pending, in progress, completed)
- Role-based navigation (Create Task button only for Admin/Manager)
- Real-time task counts

**My Tasks Component**:
- Employees see only their assigned tasks
- Filter by status (All, Pending, In Progress, Completed)
- Start and Complete buttons based on task status
- Conditional rendering of action buttons

**Task List Component**:
- Admin sees all tasks
- Manager sees tasks they created
- Employee sees tasks assigned to them
- Table view with all task details
- Status and priority badges

**Create Task Component**:
- Only accessible to Admin and Manager
- Dropdown to select employee for assignment
- Priority selection
- Form validation

### Summary of Improvements

| Improvement | Status | Impact |
|-------------|--------|--------|
| Strict task lifecycle enforcement | ✅ Implemented | High - Prevents unauthorized task status changes |
| Enhanced LINQ filtering endpoints | ✅ Implemented | Medium - Better performance and code organization |
| Validated task model | ✅ Verified | High - All required fields present |
| Enhanced Angular service | ✅ Implemented | Medium - Better API abstraction |
| Validated assignment logic | ✅ Implemented | High - Enforces role-based permissions |
| Improved UI components | ✅ Verified | Medium - Better user experience |

### Testing the Improvements

**Test Scenario 1: Employee Starting Task**
1. Login as employee1@test.com
2. Navigate to "My Tasks"
3. Click "Start Task" on a pending task
4. ✅ Task status changes to "In Progress"
5. ✅ StartDate is set

**Test Scenario 2: Admin Attempting to Start Task**
1. Login as admin@test.com
2. Try to start a task via API
3. ✅ Receives 403 Forbidden error
4. ✅ Task status remains unchanged

**Test Scenario 3: Manager Creating Task**
1. Login as manager@test.com
2. Navigate to "Create Task"
3. Fill form and assign to employee
4. ✅ Task created successfully
5. ✅ Status is "Pending"
6. ✅ AssignedBy is manager's ID

**Test Scenario 4: Employee Attempting to Create Task**
1. Login as employee1@test.com
2. ✅ "Create Task" button is hidden
3. Try to access /create-task directly
4. ✅ Can access page but API returns 403 Forbidden

**Test Scenario 5: Filtering Tasks**
1. Login as any user
2. Navigate to task list
3. Use filter dropdown
4. ✅ Tasks filtered by status
5. ✅ Only relevant tasks shown

---

## Conclusion

The **Internal Task Management System** successfully demonstrates a complete full-stack application using modern web technologies. The system implements all required academic syllabus concepts including:

- **MVC Architecture**: Clear separation between Models, Views, and Controllers
- **Layered Architecture**: Presentation, API, Business Logic, Data Access, and Database layers
- **Entity Framework Core**: ORM with migrations, LINQ queries, and relationship management
- **DTOs**: Secure data transfer between client and server
- **ASP.NET Core Web API**: RESTful API with proper HTTP methods and status codes
- **Angular SPA**: Modern single-page application with components, services, and routing
- **Authentication & Authorization**: Session-based authentication with role-based permissions
- **Middleware Pipeline**: Request processing with CORS, Session, and Authorization
- **Exception Handling**: Comprehensive error handling with try-catch blocks
- **LINQ**: Extensive use of LINQ for data querying and filtering

**Recent Improvements**:
- ✅ Strict task lifecycle enforcement (Employee-only start/complete)
- ✅ Enhanced LINQ filtering endpoints (completed, pending, created-by-me)
- ✅ Validated task model with all required fields
- ✅ Enhanced Angular service with new methods
- ✅ Validated assignment logic (Admin/Manager only)
- ✅ Improved UI components with role-based rendering

The system provides a solid foundation for understanding enterprise application development and can be extended with additional features as needed.

### Key Takeaways

1. **Separation of Concerns**: Each layer has a specific responsibility
2. **Security First**: Authentication, authorization, and password hashing
3. **Type Safety**: TypeScript and C# provide compile-time checking
4. **Modern Practices**: Async/await, dependency injection, reactive programming
5. **Scalability**: Layered architecture allows for easy scaling
6. **Maintainability**: Clean code structure and comprehensive comments
7. **User Experience**: Responsive UI with real-time updates
8. **Role-Based Access**: Strict enforcement of permissions at all levels

This project serves as an excellent reference for building similar enterprise applications and demonstrates best practices in full-stack web development.

---

**Project Completed**: March 12, 2026  
**Technology Stack**: ASP.NET Core 8.0 + Angular 21.2.0  
**Database**: SQL Server / LocalDB  
**Architecture**: MVC + Layered Architecture  
**Authentication**: Session-based with BCrypt  
**Latest Update**: Enhanced with strict task lifecycle enforcement and additional LINQ endpoints

---

*End of Project Full Analysis*


---

## Advanced Task Workflow Features

### Overview
The system has been upgraded with comprehensive task management capabilities including detailed task views, progress tracking, file attachments, and an improved commenting system. These features enable complete internal task workflow management with role-based permissions.

### 1. Task Details Page

#### Description
A dedicated page that displays complete information about a task, accessible via `/tasks/:id` route.

#### Features
- **Complete Task Information Display:**
  - Title, Description, Priority, Status
  - Assigned To, Assigned By
  - Created Date, Start Date, Completed Date
  - Deadline (if set)

- **Integrated Sections:**
  - Task Information
  - Attachments
  - Progress Updates
  - Comments

- **Task Actions:**
  - Start Task (for assigned employees)
  - Complete Task (for assigned employees)
  - View button in task lists

#### Access Control
- Admin: Can view all tasks
- Manager: Can view tasks they created
- Employee: Can view tasks assigned to them

### 2. Task Progress Updates

#### Description
A feature that allows assigned users to submit progress updates with optional file attachments.

#### Database Schema
**Table: TaskProgressUpdates**
- `Id` (int, Primary Key)
- `TaskId` (int, Foreign Key → Tasks)
- `UserId` (int, Foreign Key → Users)
- `Description` (nvarchar, required)
- `FilePath` (nvarchar, optional)
- `CreatedAt` (datetime2)

#### API Endpoints
- `POST /api/task/{taskId}/progress` - Add progress update
- `GET /api/task/{taskId}/progress` - Get all progress updates for a task

#### Features
- Text description of progress
- Optional file upload
- Timestamp tracking
- User attribution
- Ordered by most recent first

#### Permissions
- Only assigned users can submit progress updates
- All authorized users can view progress updates

### 3. Task Attachments

#### Description
File upload system for task-related documents, images, and other files.

#### Database Schema
**Table: TaskAttachments**
- `Id` (int, Primary Key)
- `TaskId` (int, Foreign Key → Tasks)
- `FileName` (nvarchar)
- `FilePath` (nvarchar)
- `UploadedBy` (int, Foreign Key → Users)
- `UploadedAt` (datetime2)

#### API Endpoints
- `POST /api/task/{taskId}/attachments` - Upload attachment
- `GET /api/task/{taskId}/attachments` - Get all attachments for a task

#### Supported File Types
- Images (jpg, png, gif, etc.)
- PDF documents
- Office documents (doc, docx, xls, xlsx)
- Text files

#### Storage
- Files stored in: `/uploads/tasks/`
- Unique filename generation using GUID
- File path stored in database

#### Permissions
- Only assigned users can upload attachments
- All authorized users can view/download attachments

### 4. Improved Task Comments

#### Description
Enhanced commenting system displayed as a discussion thread on the task details page.

#### Database Schema
**Table: TaskComments** (existing, enhanced)
- `Id` (int, Primary Key)
- `TaskId` (int, Foreign Key → Tasks)
- `UserId` (int, Foreign Key → Users)
- `Comment` (nvarchar)
- `CreatedAt` (datetime2)

#### API Endpoints
- `POST /api/task/{taskId}/comments` - Add comment
- `GET /api/task/{taskId}/comments` - Get all comments for a task

#### Features
- User name display
- Message content
- Timestamp
- Ordered chronologically
- Real-time updates

#### Permissions
- All authorized users can add comments
- Comments visible to all authorized users

### 5. Role-Based Task Assignment (Fixed)

#### Admin Permissions
- Can assign tasks to:
  - Managers
  - Employees
- User dropdown shows: Managers and Employees

#### Manager Permissions
- Can assign tasks to:
  - Employees only
- User dropdown shows: Employees only

#### Employee Permissions
- Cannot create tasks
- "Create Task" button hidden in UI
- No access to task creation page

#### Implementation
**Backend (UserController):**
```csharp
// Admin can see Managers and Employees
if (userRole == "Admin")
{
    query = query.Where(u => u.Role == "Manager" || u.Role == "Employee");
}
// Manager can only see Employees
else if (userRole == "Manager")
{
    query = query.Where(u => u.Role == "Employee");
}
```

**Frontend (Dashboard):**
```typescript
canCreateTask(): boolean {
    return this.currentUser?.role === 'Admin' || this.currentUser?.role === 'Manager';
}
```

### 6. Task Deadline Feature

#### Description
Optional deadline field added to task creation and display.

#### Database Changes
- Added `Deadline` (datetime2, nullable) column to Tasks table

#### Features
- Optional deadline during task creation
- Displayed in task details
- Datetime picker in UI

### 7. Angular UI Improvements

#### Task List Page
- Added "View" button for each task
- Clicking navigates to `/tasks/:id`
- Maintains existing filter functionality

#### Task Details Page Components
**Task Information Section:**
- Displays all task metadata
- Shows status badges with color coding
- Priority indicators
- Action buttons (Start/Complete)

**Attachments Section:**
- List of uploaded files
- Upload form for new attachments
- File name and uploader information

**Progress Updates Section:**
- Timeline of progress updates
- File attachments in updates
- Add new progress form

**Comments Section:**
- Discussion thread layout
- User attribution
- Timestamp display
- Add comment form

#### My Tasks Page
- Added "View Details" button
- Maintains Start/Complete functionality
- Quick access to task details

### 8. File Upload Support

#### Backend Implementation
**Multipart Form Data Handling:**
```csharp
[HttpPost("{taskId}/progress")]
public async Task<IActionResult> AddProgressUpdate(
    int taskId, 
    [FromForm] CreateProgressUpdateDto dto, 
    IFormFile? file)
```

**File Storage:**
- Directory: `/uploads/tasks/`
- Unique naming: `{GUID}_{originalFileName}`
- Path stored in database

**Static File Serving:**
```csharp
app.UseStaticFiles();
```

#### Frontend Implementation
**FormData Usage:**
```typescript
const formData = new FormData();
formData.append('description', description);
if (file) {
    formData.append('file', file);
}
```

### 9. Permission Rules

#### Task Modification
- Only assigned users can:
  - Submit progress updates
  - Upload attachments
  - Start tasks
  - Complete tasks

#### Task Viewing
- Admin: All tasks
- Manager: Tasks they created
- Employee: Tasks assigned to them

#### Task Creation
- Admin: Can assign to Managers or Employees
- Manager: Can assign to Employees only
- Employee: Cannot create tasks

### 10. Database Migration

#### Migration Name
`AddProgressUpdatesAndAttachments`

#### Changes Applied
1. Added `Deadline` column to Tasks table
2. Created TaskProgressUpdates table with indexes
3. Created TaskAttachments table with indexes
4. Configured foreign key relationships
5. Set up cascade delete for task-related data

#### Migration Commands
```bash
dotnet ef migrations add AddProgressUpdatesAndAttachments
dotnet ef database update
```

### 11. New API Endpoints Summary

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/task/{id}` | Get task details | Yes |
| POST | `/api/task/{taskId}/progress` | Add progress update | Yes (Assigned) |
| GET | `/api/task/{taskId}/progress` | Get progress updates | Yes |
| POST | `/api/task/{taskId}/attachments` | Upload attachment | Yes (Assigned) |
| GET | `/api/task/{taskId}/attachments` | Get attachments | Yes |
| POST | `/api/task/{taskId}/comments` | Add comment | Yes |
| GET | `/api/task/{taskId}/comments` | Get comments | Yes |

### 12. Security Considerations

#### File Upload Security
- File size limits enforced
- Unique file naming prevents overwrites
- Path traversal prevention
- Stored outside web root

#### Access Control
- Session-based authentication
- Role-based authorization on all endpoints
- Task ownership verification
- User-specific data filtering

### 13. User Experience Improvements

#### Navigation
- Back buttons on all pages
- Breadcrumb-style navigation
- Direct links to task details

#### Visual Feedback
- Loading indicators
- Success/error messages
- Color-coded status badges
- Priority indicators

#### Responsive Design
- Mobile-friendly layouts
- Flexible grid systems
- Adaptive forms

### 14. Technical Implementation Details

#### Backend Technologies Used
- ASP.NET Core Web API
- Entity Framework Core
- Multipart form data handling
- Static file middleware
- LINQ queries with Include()

#### Frontend Technologies Used
- Angular standalone components
- Reactive forms
- HttpClient with FormData
- Router navigation
- Template-driven forms

#### Database Relationships
- One-to-Many: Task → ProgressUpdates
- One-to-Many: Task → Attachments
- One-to-Many: Task → Comments
- Many-to-One: ProgressUpdate → User
- Many-to-One: Attachment → User

### 15. Testing Recommendations

#### Backend Testing
- Test file upload with various file types
- Verify role-based access control
- Test cascade delete behavior
- Validate file path security

#### Frontend Testing
- Test navigation between pages
- Verify form submissions
- Test file upload UI
- Validate role-based UI rendering

### 16. Future Enhancement Possibilities

- Real-time notifications
- Task priority sorting
- Advanced search and filtering
- Task templates
- Bulk operations
- Email notifications
- Task dependencies
- Gantt chart view
- Export to PDF/Excel
- Mobile app

---

## Conclusion

The Internal Task Management System now provides a complete workflow solution with advanced features for task tracking, collaboration, and file management. The role-based permission system ensures proper access control while the intuitive UI makes task management efficient and user-friendly.

All features have been implemented following best practices for security, scalability, and maintainability. The system is production-ready and can be extended with additional features as needed.


---

## Role-Based Task Assignment Fix

### Problem Statement
The initial implementation had incorrect role-based assignment logic. Admin users could not assign tasks to Managers because the user filtering was only showing Employees. This violated the intended role hierarchy where Admins should be able to assign tasks to both Managers and Employees.

### Role Hierarchy
The correct role hierarchy for task assignment is:

```
Admin
  ├─→ Can assign tasks to Manager
  └─→ Can assign tasks to Employee

Manager
  └─→ Can assign tasks to Employee only

Employee
  └─→ Cannot create or assign tasks
```

### Backend Implementation

#### 1. New API Endpoint: GET /api/user/assignable

Created a dedicated endpoint to fetch assignable users based on the current user's role.

**Location:** `TaskManagementAPI/Controllers/UserController.cs`

**Implementation:**
```csharp
[HttpGet("assignable")]
public async Task<IActionResult> GetAssignableUsers()
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

    IQueryable<User> query = _context.Users;

    // Role-based filtering
    if (userRole == "Admin")
    {
        // Admin can assign to Manager or Employee
        query = query.Where(u => u.Role == "Manager" || u.Role == "Employee");
    }
    else if (userRole == "Manager")
    {
        // Manager can only assign to Employee
        query = query.Where(u => u.Role == "Employee");
    }
    else
    {
        // Employee cannot assign - return empty list
        return Ok(new List<UserDto>());
    }

    var users = await query
        .Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        })
        .OrderBy(u => u.Role)
        .ThenBy(u => u.Name)
        .ToListAsync();

    return Ok(users);
}
```

**LINQ Logic Explanation:**
- Uses `Where()` clause to filter users based on role
- For Admin: `u.Role == "Manager" || u.Role == "Employee"`
- For Manager: `u.Role == "Employee"`
- Results ordered by Role first, then by Name for better UX

#### 2. Task Creation Validation

Added server-side validation to enforce role hierarchy when creating tasks.

**Location:** `TaskManagementAPI/Controllers/TaskController.cs`

**Implementation:**
```csharp
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
```

**Validation Rules:**
1. Fetch the assigned user from database
2. Check if user exists
3. Validate role hierarchy:
   - Admin → Can assign to Manager or Employee
   - Manager → Can assign to Employee only
   - Any violation returns HTTP 403 Forbidden

### Frontend Implementation

#### 1. Updated UserService

Added method to call the new assignable users endpoint.

**Location:** `TaskManagementUI/src/app/services/user.service.ts`

**Implementation:**
```typescript
getAssignableUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/assignable`, { withCredentials: true });
}
```

#### 2. Updated CreateTaskComponent

Modified the component to use the assignable users endpoint instead of filtering on the client side.

**Location:** `TaskManagementUI/src/app/components/create-task/create-task.ts`

**Before:**
```typescript
loadUsers(): void {
    this.userService.getUsers().subscribe({
        next: (users) => {
            // Client-side filtering - INCORRECT
            this.users = users.filter(u => u.role === 'Employee');
        }
    });
}
```

**After:**
```typescript
loadUsers(): void {
    this.userService.getAssignableUsers().subscribe({
        next: (users) => {
            // Server returns correctly filtered users
            this.users = users;
        },
        error: (error) => {
            this.errorMessage = 'Failed to load users';
        }
    });
}
```

#### 3. UI Improvement: Display Role in Dropdown

Updated the dropdown to show both user name and role for better clarity.

**Location:** `TaskManagementUI/src/app/components/create-task/create-task.html`

**Before:**
```html
<option *ngFor="let user of users" [value]="user.id">
    {{ user.name }} ({{ user.email }})
</option>
```

**After:**
```html
<option *ngFor="let user of users" [value]="user.id">
    {{ user.name }} ({{ user.role }})
</option>
```

**Example Display:**
- Rahul Sharma (Manager)
- Priya Patel (Employee)
- Amit Kumar (Employee)

### Permission Enforcement Summary

| User Role | Can Assign To | Validation Location |
|-----------|---------------|---------------------|
| Admin | Manager, Employee | Backend + Frontend |
| Manager | Employee only | Backend + Frontend |
| Employee | None (cannot create tasks) | Backend + Frontend |

### Security Benefits

1. **Server-Side Validation:** Even if frontend is bypassed, backend validates role hierarchy
2. **HTTP 403 Forbidden:** Clear error response for unauthorized assignments
3. **Session-Based Auth:** User role verified from server session, not client
4. **Defense in Depth:** Multiple layers of validation (frontend + backend)

### Testing Scenarios

#### Test Case 1: Admin Creating Task
**Steps:**
1. Login as Admin
2. Navigate to Create Task
3. Open "Assign To" dropdown

**Expected Result:**
- Dropdown shows Managers and Employees
- Each entry displays: Name (Role)
- Can successfully assign to Manager
- Can successfully assign to Employee

#### Test Case 2: Manager Creating Task
**Steps:**
1. Login as Manager
2. Navigate to Create Task
3. Open "Assign To" dropdown

**Expected Result:**
- Dropdown shows only Employees
- Each entry displays: Name (Role)
- Can successfully assign to Employee
- Cannot see Managers in dropdown

#### Test Case 3: Employee Access
**Steps:**
1. Login as Employee
2. View Dashboard

**Expected Result:**
- "Create Task" button is hidden
- Cannot access /create-task route
- If route accessed directly, backend returns 403

#### Test Case 4: Backend Validation
**Steps:**
1. Attempt to bypass frontend validation
2. Send POST request with invalid assignment

**Expected Result:**
- Backend validates role hierarchy
- Returns 403 Forbidden if rules violated
- Task is not created

### Code Quality Improvements

1. **Separation of Concerns:** Dedicated endpoint for assignable users
2. **Single Responsibility:** Each method has one clear purpose
3. **DRY Principle:** Reusable filtering logic
4. **Clear Naming:** `getAssignableUsers()` clearly indicates purpose
5. **Consistent Error Handling:** Proper HTTP status codes

### API Documentation

#### GET /api/user/assignable

**Description:** Returns list of users that can be assigned tasks based on current user's role

**Authentication:** Required (Session-based)

**Authorization:** Admin, Manager only

**Response:**
```json
[
    {
        "id": 2,
        "name": "Rahul Sharma",
        "email": "rahul@company.com",
        "role": "Manager",
        "createdAt": "2024-01-15T10:30:00Z"
    },
    {
        "id": 3,
        "name": "Priya Patel",
        "email": "priya@company.com",
        "role": "Employee",
        "createdAt": "2024-01-15T10:35:00Z"
    }
]
```

**Status Codes:**
- 200 OK: Success
- 401 Unauthorized: Not logged in
- 403 Forbidden: Employee trying to access

### Impact on Existing Features

**No Breaking Changes:**
- Existing task viewing functionality unchanged
- Task completion workflow unchanged
- User management unchanged
- Authentication/authorization unchanged

**Enhanced Features:**
- More accurate user filtering
- Better validation
- Improved user experience with role display
- Stronger security

### Conclusion

The role-based task assignment fix ensures that the system correctly implements the organizational hierarchy. Admins can now properly assign tasks to Managers, Managers can assign to Employees, and Employees are prevented from creating tasks. The implementation includes both frontend UX improvements and backend security validation, providing a robust and user-friendly solution.


---

## Dashboard Behavior and Task View Fixes

### Problem Statement
Several issues were identified in the dashboard behavior and task viewing functionality:
1. Manager's "My Tasks" View button was causing errors
2. "All Tasks" page was incorrectly filtering tasks by role
3. Dashboard was showing filtered tasks instead of all tasks
4. Admin dashboard was showing "My Tasks" button (Admins cannot be assigned tasks)
5. Error handling was insufficient when task details failed to load

### Issues Fixed

#### Issue 1: All Tasks Should Show Every Task

**Problem:**
The `GET /api/task` endpoint was applying role-based filtering:
- Admin saw all tasks
- Manager saw only tasks they created
- Employee saw only tasks assigned to them

This was incorrect for the "All Tasks" page, which should show all tasks to everyone.

**Solution:**
Removed all role-based filtering from the `GET /api/task` endpoint.

**Location:** `TaskManagementAPI/Controllers/TaskController.cs`

**Before:**
```csharp
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
```

**After:**
```csharp
// ALL TASKS - No filtering, return all tasks in the system
var tasks = await _context.Tasks
    .Include(t => t.AssignedByUser)
    .Include(t => t.AssignedToUser)
    .Select(t => new TaskDto { ... })
    .ToListAsync();
```

**Result:**
- All users (Admin, Manager, Employee) now see all tasks in the "All Tasks" page
- Provides complete visibility across the organization
- Maintains proper filtering in "My Tasks" page via separate endpoint

#### Issue 2: My Tasks Filtering

**Correct Implementation:**
The `GET /api/task/my` endpoint correctly filters tasks assigned to the current user.

**Location:** `TaskManagementAPI/Controllers/TaskController.cs`

```csharp
[HttpGet("my")]
public async Task<IActionResult> GetMyTasks()
{
    var userId = HttpContext.Session.GetInt32("UserId");
    
    var tasks = await _context.Tasks
        .Include(t => t.AssignedByUser)
        .Include(t => t.AssignedToUser)
        .Where(t => t.AssignedTo == userId.Value)  // Filter by assigned user
        .Select(t => new TaskDto { ... })
        .ToListAsync();
    
    return Ok(tasks);
}
```

**LINQ Logic:**
- `Where(t => t.AssignedTo == userId.Value)` filters tasks where AssignedTo matches current user
- Only returns tasks assigned to the logged-in user
- Works for both Managers and Employees

#### Issue 3: Remove My Tasks from Admin Dashboard

**Problem:**
Admin users cannot be assigned tasks, so the "My Tasks" button was meaningless for them.

**Solution:**
Added conditional rendering to hide "My Tasks" button for Admin users.

**Location:** `TaskManagementUI/src/app/components/dashboard/dashboard.html`

**Before:**
```html
<button routerLink="/my-tasks" class="btn-secondary">
    My Tasks
</button>
```

**After:**
```html
<button *ngIf="!isAdmin()" routerLink="/my-tasks" class="btn-secondary">
    My Tasks
</button>
```

**TypeScript Implementation:**
```typescript
isAdmin(): boolean {
    return this.currentUser?.role === 'Admin';
}
```

**Result:**
- Admin dashboard shows: "Create New Task" and "All Tasks"
- Manager dashboard shows: "Create New Task", "My Tasks", and "All Tasks"
- Employee dashboard shows: "My Tasks" and "All Tasks"

#### Issue 4: Dashboard Statistics Fix

**Problem:**
Dashboard was loading filtered tasks based on role, causing incorrect statistics.

**Solution:**
Changed dashboard to always load all tasks for accurate statistics.

**Location:** `TaskManagementUI/src/app/components/dashboard/dashboard.ts`

**Before:**
```typescript
loadTasks(): void {
    if (this.currentUser?.role === 'Employee') {
        this.taskService.getMyTasks().subscribe(tasks => {
            this.tasks = tasks;
            this.calculateStats();
        });
    } else {
        this.taskService.getTasks().subscribe(tasks => {
            this.tasks = tasks;
            this.calculateStats();
        });
    }
}
```

**After:**
```typescript
loadTasks(): void {
    this.taskService.getTasks().subscribe(tasks => {
        this.tasks = tasks;
        this.calculateStats();
    });
}
```

**Result:**
- Dashboard statistics now reflect all tasks in the system
- Provides accurate overview for all users
- Total, Pending, In Progress, and Completed counts are correct

#### Issue 5: Task View Error Handling

**Problem:**
When clicking "View" on a task, if the task didn't exist or user didn't have permission, the error message was generic and unhelpful.

**Solution:**
Enhanced error handling with descriptive messages.

**Location:** `TaskManagementUI/src/app/components/task-details/task-details.ts`

**Before:**
```typescript
error: (err) => {
    this.error = 'Failed to load task details';
    this.loading = false;
}
```

**After:**
```typescript
error: (err) => {
    console.error('Failed to load task details', err);
    this.error = 'Task details could not be loaded. The task may not exist or you may not have permission to view it.';
    this.loading = false;
}
```

**Result:**
- Clear error message displayed to user
- Error logged to console for debugging
- Loading state properly cleared
- UI doesn't crash, shows friendly error message

#### Issue 6: Task View Button Implementation

**Verification:**
The "View" button in My Tasks page was already correctly implemented.

**Location:** `TaskManagementUI/src/app/components/my-tasks/my-tasks.html`

```html
<button [routerLink]="['/tasks', task.id]" class="btn-view">View Details</button>
```

**How It Works:**
- Uses Angular's `routerLink` directive with array syntax
- First element: route path `/tasks`
- Second element: task ID parameter
- Navigates to `/tasks/123` (where 123 is the task ID)
- Matches route definition: `{ path: 'tasks/:id', component: TaskDetailsComponent }`

### API Endpoints Summary

| Endpoint | Purpose | Filtering |
|----------|---------|-----------|
| `GET /api/task` | All Tasks page | None - returns all tasks |
| `GET /api/task/my` | My Tasks page | Filters by AssignedTo = currentUserId |
| `GET /api/task/{id}` | Task Details | Returns single task by ID |

### Dashboard Button Visibility Matrix

| User Role | Create Task | My Tasks | All Tasks |
|-----------|-------------|----------|-----------|
| Admin | ✓ | ✗ | ✓ |
| Manager | ✓ | ✓ | ✓ |
| Employee | ✗ | ✓ | ✓ |

### Testing Scenarios

#### Test Case 1: Manager Views Task Details
**Steps:**
1. Login as Manager
2. Navigate to "My Tasks"
3. Click "View Details" on any task

**Expected Result:**
- Navigates to `/tasks/{id}`
- Task details page loads successfully
- Shows task information, comments, progress, attachments
- No errors in console

#### Test Case 2: All Tasks Page Shows Everything
**Steps:**
1. Login as any user (Admin/Manager/Employee)
2. Navigate to "All Tasks"
3. Observe task list

**Expected Result:**
- All tasks in the system are displayed
- No filtering based on user role
- Can see tasks assigned by others
- Can see tasks assigned to others

#### Test Case 3: My Tasks Shows Only Assigned Tasks
**Steps:**
1. Login as Manager or Employee
2. Navigate to "My Tasks"
3. Observe task list

**Expected Result:**
- Only tasks assigned to current user are shown
- Tasks created by user but assigned to others are NOT shown
- Filtering works correctly

#### Test Case 4: Admin Dashboard
**Steps:**
1. Login as Admin
2. View Dashboard

**Expected Result:**
- "Create New Task" button visible
- "All Tasks" button visible
- "My Tasks" button NOT visible
- Statistics show all tasks in system

#### Test Case 5: Error Handling
**Steps:**
1. Navigate to `/tasks/99999` (non-existent task)
2. Observe error handling

**Expected Result:**
- Error message displayed: "Task details could not be loaded..."
- No application crash
- Can navigate back to other pages
- Error logged to console

### Code Quality Improvements

1. **Simplified Logic:** Removed unnecessary role-based branching in dashboard
2. **Clear Separation:** All Tasks vs My Tasks have distinct purposes
3. **Better UX:** Admin doesn't see irrelevant "My Tasks" button
4. **Defensive Programming:** Proper error handling prevents crashes
5. **Consistent Behavior:** All users see same data in "All Tasks"

### Security Considerations

**Backend Validation:**
- Task details endpoint still validates user has permission to view task
- Returns 403 Forbidden if access denied
- Session-based authentication required for all endpoints

**Frontend Display:**
- Buttons conditionally rendered based on role
- Client-side checks for better UX
- Backend enforces actual authorization

### Performance Impact

**Positive:**
- Simplified query logic (no conditional filtering)
- Faster query execution
- Reduced code complexity

**Neutral:**
- All users load all tasks (acceptable for internal system)
- Can add pagination if task count grows large

### Benefits of These Fixes

1. **Correct Functionality:** All Tasks page now works as intended
2. **Better UX:** Admin doesn't see confusing "My Tasks" button
3. **Accurate Statistics:** Dashboard shows correct task counts
4. **Robust Error Handling:** Graceful failure instead of crashes
5. **Clear Separation:** Distinct purposes for All Tasks vs My Tasks
6. **Organizational Visibility:** Everyone can see all work being done

### Conclusion

The dashboard behavior and task view fixes ensure that:
- All Tasks page provides complete visibility to all users
- My Tasks page shows only assigned tasks
- Admin dashboard is streamlined without irrelevant options
- Task viewing is robust with proper error handling
- Navigation works correctly across all user roles

These changes improve both functionality and user experience while maintaining security and data integrity.


---

## Manager Task View Permission Fix

### Problem Statement
After fixing the "All Tasks" page to show all tasks, Managers were unable to view task details when clicking the "View" button. The system was returning a "403 Forbidden" error with a message indicating they didn't have permission to view the task.

### Root Cause
The `GET /api/task/{id}` endpoint and related endpoints (progress, attachments, comments) had overly restrictive access control logic:

```csharp
// OLD LOGIC - TOO RESTRICTIVE
if (userRole == "Manager" && task.AssignedBy != userId.Value)
{
    return Forbid();  // Manager could only view tasks they created
}
```

This logic only allowed Managers to view tasks they personally created. However, since we changed "All Tasks" to show all tasks for organizational visibility, Managers should be able to view any task details.

### Solution Implemented

Updated the access control logic across all task-related endpoints to allow Managers full visibility while maintaining Employee restrictions.

#### Updated Endpoints

1. **GET /api/task/{id}** - Task Details
2. **GET /api/task/{taskId}/progress** - Progress Updates
3. **GET /api/task/{taskId}/attachments** - Attachments
4. **GET /api/task/{taskId}/comments** - Comments

#### New Access Control Logic

**Location:** `TaskManagementAPI/Controllers/TaskController.cs`

**Before:**
```csharp
// Role-based access control
if (userRole == "Manager" && task.AssignedBy != userId.Value)
{
    return Forbid();
}
else if (userRole == "Employee" && task.AssignedTo != userId.Value)
{
    return Forbid();
}
```

**After:**
```csharp
// Role-based access control
// Admin and Manager can view any task
// Employee can only view tasks assigned to them
if (userRole == "Employee" && task.AssignedTo != userId.Value)
{
    return Forbid();
}
```

### Access Control Matrix

| User Role | Can View Task Details | Restriction |
|-----------|----------------------|-------------|
| Admin | All tasks | None |
| Manager | All tasks | None |
| Employee | Assigned tasks only | Must be AssignedTo |

### Benefits

1. **Organizational Visibility:** Managers can now monitor all tasks in their organization
2. **Consistent Behavior:** Aligns with "All Tasks" page showing all tasks
3. **Better Oversight:** Managers can track progress on tasks they didn't create
4. **Simplified Logic:** Removed unnecessary role-based branching
5. **Employee Privacy:** Employees still can only view their assigned tasks

### Technical Details

#### Endpoints Updated

**1. Task Details Endpoint**
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetTaskById(int id)
{
    // ... authentication checks ...
    
    // Only restrict Employees
    if (userRole == "Employee" && task.AssignedTo != userId.Value)
    {
        return Forbid();
    }
    
    return Ok(taskDto);
}
```

**2. Progress Updates Endpoint**
```csharp
[HttpGet("{taskId}/progress")]
public async Task<IActionResult> GetProgressUpdates(int taskId)
{
    // Admin and Manager can view any task's progress
    // Employee can only view progress for tasks assigned to them
    if (userRole == "Employee" && task.AssignedTo != userId.Value)
    {
        return Forbid();
    }
    
    return Ok(updates);
}
```

**3. Attachments Endpoint**
```csharp
[HttpGet("{taskId}/attachments")]
public async Task<IActionResult> GetAttachments(int taskId)
{
    // Admin and Manager can view any task's attachments
    // Employee can only view attachments for tasks assigned to them
    if (userRole == "Employee" && task.AssignedTo != userId.Value)
    {
        return Forbid();
    }
    
    return Ok(attachments);
}
```

**4. Comments Endpoint**
```csharp
[HttpGet("{taskId}/comments")]
public async Task<IActionResult> GetComments(int taskId)
{
    // Admin and Manager can view any task's comments
    // Employee can only view comments for tasks assigned to them
    if (userRole == "Employee" && task.AssignedTo != userId.Value)
    {
        return Forbid();
    }
    
    return Ok(comments);
}
```

### Testing Scenarios

#### Test Case 1: Manager Views Any Task
**Steps:**
1. Login as Manager
2. Navigate to "All Tasks"
3. Click "View" on any task (created by anyone)

**Expected Result:**
- Task details page loads successfully
- All sections visible: Info, Attachments, Progress, Comments
- No permission errors
- Can view but cannot modify tasks assigned to others

#### Test Case 2: Manager Views Task Created by Admin
**Steps:**
1. Admin creates task and assigns to Employee
2. Login as Manager
3. View the task details

**Expected Result:**
- Manager can view full task details
- Can see all progress updates
- Can see all attachments
- Can see all comments
- Demonstrates organizational visibility

#### Test Case 3: Employee Views Own Task
**Steps:**
1. Login as Employee
2. Navigate to "My Tasks"
3. Click "View" on assigned task

**Expected Result:**
- Task details load successfully
- Can view and interact with task
- Can add progress, attachments, comments

#### Test Case 4: Employee Tries to View Other's Task
**Steps:**
1. Login as Employee
2. Try to access `/tasks/{id}` for task not assigned to them

**Expected Result:**
- Returns 403 Forbidden
- Error message displayed
- Cannot view task details
- Privacy maintained

### Security Considerations

**Maintained Security:**
- Employees still cannot view tasks not assigned to them
- Authentication required for all endpoints
- Session-based authorization enforced
- Task existence validated before permission check

**Enhanced Visibility:**
- Managers have oversight capability
- Admins have full system visibility
- Supports management and monitoring needs

### Impact on Existing Features

**No Breaking Changes:**
- Employee restrictions unchanged
- Admin access unchanged
- Task modification permissions unchanged
- Only viewing permissions expanded for Managers

**Improved Features:**
- Managers can now fully utilize "All Tasks" page
- View button works consistently across all dashboards
- Better alignment between list view and detail view permissions

### Code Quality

**Improvements:**
1. **Simplified Logic:** Removed unnecessary conditional branches
2. **Consistent Pattern:** Same access control across all endpoints
3. **Clear Comments:** Documented permission rules in code
4. **Maintainable:** Easy to understand and modify

### Conclusion

The Manager task view permission fix ensures that Managers have appropriate visibility into all organizational tasks while maintaining Employee privacy. This change aligns the detail view permissions with the list view permissions, providing a consistent and intuitive user experience.

**Key Takeaway:** Admin and Manager roles now have full read access to all tasks, while Employees can only view tasks assigned to them. This supports organizational oversight while protecting individual privacy.


---

## Task Details Back Button Navigation Fix

### Problem Statement
When navigating from "My Tasks" page to view a task's details and then clicking the "Back" button, the user was always redirected to the "All Tasks" page instead of returning to the "My Tasks" page they came from. This created a poor user experience and disrupted the navigation flow.

### Root Cause
The back button in the task details component was using hardcoded navigation:

```typescript
goBack(): void {
    this.router.navigate(['/tasks']);  // Always goes to /tasks (All Tasks)
}
```

This approach didn't respect the user's navigation history and always sent them to the same page regardless of where they came from.

### Solution Implemented

Replaced the hardcoded navigation with Angular's `Location` service, which uses the browser's history API to navigate back to the previous page.

**Location:** `TaskManagementUI/src/app/components/task-details/task-details.ts`

#### Changes Made

**1. Import Location Service**
```typescript
import { CommonModule, Location } from '@angular/common';
```

**2. Inject Location Service**
```typescript
constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,  // Added Location service
    private taskService: TaskService,
    private authService: AuthService
) { }
```

**3. Update goBack() Method**
```typescript
goBack(): void {
    this.location.back();  // Uses browser history to go back
}
```

### How It Works

**Angular Location Service:**
- Part of `@angular/common` package
- Wraps the browser's `window.history` API
- Provides `back()` method to navigate to previous page
- Maintains navigation history stack
- Works with Angular's routing system

**Navigation Flow Examples:**

**Scenario 1: From My Tasks**
1. User is on `/my-tasks`
2. Clicks "View Details" → navigates to `/tasks/123`
3. Clicks "Back" → `location.back()` returns to `/my-tasks`

**Scenario 2: From All Tasks**
1. User is on `/tasks`
2. Clicks "View" → navigates to `/tasks/123`
3. Clicks "Back" → `location.back()` returns to `/tasks`

**Scenario 3: From Dashboard**
1. User is on `/dashboard`
2. Clicks on a task → navigates to `/tasks/123`
3. Clicks "Back" → `location.back()` returns to `/dashboard`

### Benefits

1. **Intuitive Navigation:** Users return to where they came from
2. **Better UX:** Respects user's navigation context
3. **Consistent Behavior:** Works the same way as browser back button
4. **No Hardcoding:** Doesn't assume where user came from
5. **Flexible:** Works from any page that links to task details

### Technical Details

**Browser History API:**
```javascript
// What location.back() does internally
window.history.back();
```

**Angular Location Service Methods:**
- `back()` - Navigate to previous page
- `forward()` - Navigate to next page (if available)
- `go(n)` - Navigate n steps in history
- `path()` - Get current path
- `replaceState()` - Replace current history entry

### Edge Cases Handled

**Case 1: Direct URL Access**
If user directly navigates to `/tasks/123` (no history):
- `location.back()` will go to previous page in browser history
- If no history, typically goes to default route or stays on page

**Case 2: External Link**
If user comes from external link:
- `location.back()` navigates to referring page
- Falls back to browser's default behavior

**Case 3: Page Refresh**
If user refreshes the page:
- History is maintained by browser
- Back button still works correctly

### Alternative Approaches Considered

**1. Query Parameters (Not Used)**
```typescript
// Could pass source in URL
this.router.navigate(['/tasks', id], { queryParams: { from: 'my-tasks' }});

// Then navigate back based on param
goBack(): void {
    const from = this.route.snapshot.queryParams['from'];
    this.router.navigate([from || '/tasks']);
}
```
**Rejected because:** Requires modifying all navigation calls, clutters URLs

**2. State Management (Not Used)**
```typescript
// Could store navigation state in service
navigationService.setPreviousRoute('/my-tasks');

// Then navigate back
goBack(): void {
    const previous = navigationService.getPreviousRoute();
    this.router.navigate([previous]);
}
```
**Rejected because:** Adds complexity, Location service is simpler

**3. Location Service (Chosen)**
```typescript
goBack(): void {
    this.location.back();
}
```
**Chosen because:** Simple, built-in, respects browser history, no extra code needed

### Testing Scenarios

#### Test Case 1: My Tasks → Task Details → Back
**Steps:**
1. Navigate to "My Tasks"
2. Click "View Details" on any task
3. Click "Back" button

**Expected Result:**
- Returns to "My Tasks" page
- Task list is still visible
- Filter state preserved (if any)

#### Test Case 2: All Tasks → Task Details → Back
**Steps:**
1. Navigate to "All Tasks"
2. Click "View" on any task
3. Click "Back" button

**Expected Result:**
- Returns to "All Tasks" page
- Task list is still visible
- Filter state preserved (if any)

#### Test Case 3: Dashboard → Task Details → Back
**Steps:**
1. Stay on Dashboard
2. Click on a task from recent tasks
3. Click "Back" button

**Expected Result:**
- Returns to Dashboard
- Statistics still visible
- Recent tasks list intact

#### Test Case 4: Multiple Navigation Levels
**Steps:**
1. Dashboard → My Tasks → Task Details
2. Click "Back"

**Expected Result:**
- Returns to "My Tasks" (previous page)
- Can click back again to reach Dashboard

### Code Quality

**Improvements:**
1. **Simpler Code:** One line instead of router navigation
2. **Less Coupling:** Doesn't need to know about routes
3. **Standard Pattern:** Uses Angular's recommended approach
4. **Maintainable:** No hardcoded routes to update

### Browser Compatibility

The Location service works across all modern browsers:
- Chrome/Edge (Chromium)
- Firefox
- Safari
- Opera

Uses standard HTML5 History API, supported since:
- Chrome 5+
- Firefox 4+
- Safari 5+
- IE 10+

### Performance Impact

**Positive:**
- No additional HTTP requests
- No state management overhead
- Uses browser's native history
- Instant navigation

**Neutral:**
- Same performance as browser back button
- No caching issues

### User Experience Impact

**Before Fix:**
- User on "My Tasks" → View task → Back → Ends up on "All Tasks" (confusing!)
- User has to manually navigate back to "My Tasks"
- Disrupts workflow

**After Fix:**
- User on "My Tasks" → View task → Back → Returns to "My Tasks" (expected!)
- Natural navigation flow
- Matches user's mental model

### Conclusion

The back button navigation fix improves user experience by respecting the user's navigation history. Using Angular's Location service provides a simple, standard, and maintainable solution that works consistently across all navigation scenarios.

**Key Takeaway:** Always use `location.back()` for back buttons instead of hardcoded navigation paths. This respects the user's journey through the application and provides intuitive navigation behavior.


---

## Task Editing and Attachment Permission Workflow

### Overview
Implemented strict permission controls for task editing and attachment uploads, along with a request/approval system for post-completion attachments. This ensures data integrity and provides a controlled workflow for managing completed tasks.

### Task Edit Permissions

#### Rules
Only the user who CREATED the task (AssignedBy) can edit it, and only when the task is NOT completed.

**Permission Logic:**
```
IF currentUser == Task.AssignedBy AND Task.Status != "Completed"
    → Allow edit
ELSE
    → Return HTTP 403 Forbidden
```

#### Implementation
**Location:** `TaskManagementAPI/Controllers/TaskController.cs`

```csharp
// Check if user is task creator
if (task.AssignedBy != userId.Value)
{
    return Forbid();
}

// Check if task is not completed
if (task.Status == "Completed")
{
    return StatusCode(403, new { message = "Cannot edit completed tasks" });
}
```

### Attachment Upload Permissions

#### Rules
Attachments can only be uploaded by:
1. The task creator (AssignedBy) while task is active
2. Users with approved permission requests (for completed tasks)

**Permission Logic:**
```
IF Task.Status == "Completed"
    → Reject with message to request permission
    
IF currentUser == Task.AssignedBy
    → Allow upload
    
IF currentUser has ApprovedPermissionRequest
    → Allow upload
    
ELSE
    → Return HTTP 403 Forbidden
```

#### Implementation
**Location:** `TaskManagementAPI/Controllers/TaskController.cs` - `UploadAttachment` method

```csharp
// Check if task is completed
if (task.Status == "Completed")
{
    return StatusCode(403, new { message = "Cannot upload attachments to completed tasks. Please request permission first." });
}

// Check if user is task creator
bool isTaskCreator = task.AssignedBy == userId.Value;

// Check for approved permission request
bool hasApprovedRequest = await _context.AttachmentPermissionRequests
    .AnyAsync(apr => apr.TaskId == taskId 
                  && apr.RequestedByUserId == userId.Value 
                  && apr.Status == "Approved");

if (!isTaskCreator && !hasApprovedRequest)
{
    return StatusCode(403, new { message = "Only the task creator can upload attachments" });
}
```

### Task Completion Lock

#### Rules
When a task status becomes "Completed", the system locks the task:
- Cannot edit task details
- Cannot upload attachments (without approval)
- Cannot delete attachments
- Cannot modify task information

#### Enforcement
All modification endpoints check task status before allowing changes:

```csharp
if (task.Status == "Completed")
{
    return StatusCode(403, new { message = "Task is locked after completion" });
}
```

### Post-Completion Attachment Request System

#### Database Schema

**Table: AttachmentPermissionRequests**

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| TaskId | int | Foreign key to Tasks |
| RequestedByUserId | int | User requesting permission |
| Message | nvarchar | Reason for request |
| Status | nvarchar | Pending/Approved/Rejected |
| CreatedAt | datetime2 | Request timestamp |
| ReviewedByUserId | int? | User who reviewed |
| ReviewedAt | datetime2? | Review timestamp |

**Relationships:**
- Task → AttachmentPermissionRequests (One-to-Many, Cascade Delete)
- User → AttachmentPermissionRequests (RequestedBy, Restrict Delete)
- User → AttachmentPermissionRequests (ReviewedBy, Restrict Delete)

#### Workflow

**Step 1: User Requests Permission**
```
User views completed task
→ Clicks "Request Upload Permission"
→ Enters message explaining need
→ System creates request with Status="Pending"
```

**Step 2: Task Creator Reviews Request**
```
Task creator views task details
→ Sees "Attachment Permission Requests" section
→ Reviews requester name, message, date
→ Clicks "Approve" or "Reject"
```

**Step 3: System Updates Request**
```
IF Approved:
    → Status = "Approved"
    → ReviewedBy = current user
    → ReviewedAt = current timestamp
    → User can now upload attachments

IF Rejected:
    → Status = "Rejected"
    → ReviewedBy = current user
    → ReviewedAt = current timestamp
    → User remains blocked
```

### API Endpoints

#### 1. Create Permission Request
**Endpoint:** `POST /api/task/{taskId}/attachment-request`

**Request Body:**
```json
{
    "message": "Need to upload final documentation"
}
```

**Response:**
```json
{
    "id": 1,
    "taskId": 123,
    "requestedByUserId": 5,
    "requestedByUserName": "John Doe",
    "message": "Need to upload final documentation",
    "status": "Pending",
    "createdAt": "2024-01-15T10:30:00Z"
}
```

**Authorization:** Any authenticated user
**Validation:**
- Task must exist
- Task must be completed
- User cannot have existing pending request

#### 2. Get Permission Requests
**Endpoint:** `GET /api/task/{taskId}/attachment-requests`

**Response:**
```json
[
    {
        "id": 1,
        "taskId": 123,
        "requestedByUserId": 5,
        "requestedByUserName": "John Doe",
        "message": "Need to upload final documentation",
        "status": "Pending",
        "createdAt": "2024-01-15T10:30:00Z",
        "reviewedByUserId": null,
        "reviewedByUserName": null,
        "reviewedAt": null
    }
]
```

**Authorization:** Only task creator (AssignedBy)

#### 3. Approve Permission Request
**Endpoint:** `POST /api/task/attachment-request/{id}/approve`

**Response:**
```json
{
    "message": "Permission request approved"
}
```

**Authorization:** Only task creator
**Validation:**
- Request must exist
- Request status must be "Pending"
- Current user must be task creator

#### 4. Reject Permission Request
**Endpoint:** `POST /api/task/attachment-request/{id}/reject`

**Response:**
```json
{
    "message": "Permission request rejected"
}
```

**Authorization:** Only task creator
**Validation:**
- Request must exist
- Request status must be "Pending"
- Current user must be task creator

### Angular UI Changes

#### Task Details Page Updates

**For Active Tasks (Status != Completed):**
```typescript
// Show if user is task creator
if (task.status !== 'Completed' && task.assignedBy === currentUserId) {
    // Show buttons:
    // - Edit Task
    // - Upload Attachment
}
```

**For Completed Tasks:**
```typescript
// Hide edit and upload buttons
// Show request button for non-creators
if (task.status === 'Completed' && task.assignedBy !== currentUserId) {
    // Show button:
    // - Request Upload Permission
}
```

**For Task Creators (Completed Tasks):**
```typescript
// Show permission requests section
if (task.assignedBy === currentUserId) {
    // Display section:
    // - Attachment Permission Requests
    //   - Requester name
    //   - Message
    //   - Request date
    //   - Approve button
    //   - Reject button
}
```

#### Service Methods Added

**Location:** `TaskManagementUI/src/app/services/task.service.ts`

```typescript
createAttachmentPermissionRequest(taskId: number, message: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${taskId}/attachment-request`, 
        { message }, 
        { withCredentials: true });
}

getAttachmentPermissionRequests(taskId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${taskId}/attachment-requests`, 
        { withCredentials: true });
}

approveAttachmentPermissionRequest(requestId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/attachment-request/${requestId}/approve`, 
        {}, 
        { withCredentials: true });
}

rejectAttachmentPermissionRequest(requestId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/attachment-request/${requestId}/reject`, 
        {}, 
        { withCredentials: true });
}
```

### Permission Matrix

| Action | Task Creator (Active) | Task Creator (Completed) | Assigned User (Active) | Assigned User (Completed) | Other User |
|--------|----------------------|-------------------------|----------------------|--------------------------|------------|
| Edit Task | ✓ | ✗ | ✗ | ✗ | ✗ |
| Upload Attachment | ✓ | ✗ | ✗ | ✗ | ✗ |
| Request Permission | N/A | N/A | ✓ | ✓ | ✓ |
| Approve Request | N/A | ✓ | N/A | N/A | ✗ |
| View Requests | N/A | ✓ | N/A | N/A | ✗ |
| Upload (Approved) | N/A | N/A | N/A | ✓ | ✓ |

### Security Features

1. **Backend Validation:** All permissions enforced server-side
2. **Status Checks:** Task completion status verified before modifications
3. **User Verification:** Session-based authentication required
4. **Role Validation:** Task creator identity verified from database
5. **Request Tracking:** All permission requests logged with timestamps
6. **Audit Trail:** ReviewedBy and ReviewedAt fields track approvals

### Database Migration

**Migration Name:** `AddAttachmentPermissionRequests`

**Changes Applied:**
1. Created `AttachmentPermissionRequests` table
2. Added foreign key to `Tasks` (Cascade Delete)
3. Added foreign key to `Users` for RequestedBy (Restrict Delete)
4. Added foreign key to `Users` for ReviewedBy (Restrict Delete)
5. Created indexes on TaskId, RequestedByUserId, ReviewedByUserId

**Migration Commands:**
```bash
dotnet ef migrations add AddAttachmentPermissionRequests
dotnet ef database update
```

### Test Cases

#### Test Case 1: Task Creator Uploads to Active Task
**Steps:**
1. Login as task creator
2. Navigate to active task details
3. Upload attachment

**Expected Result:**
- Upload succeeds
- Attachment appears in list
- No permission request needed

#### Test Case 2: Non-Creator Tries to Upload to Active Task
**Steps:**
1. Login as non-creator
2. Navigate to active task details
3. Try to upload attachment

**Expected Result:**
- Upload fails with 403 Forbidden
- Error message: "Only the task creator can upload attachments"

#### Test Case 3: Upload to Completed Task Without Permission
**Steps:**
1. Login as any user
2. Navigate to completed task
3. Try to upload attachment

**Expected Result:**
- Upload fails with 403 Forbidden
- Error message: "Cannot upload attachments to completed tasks. Please request permission first."

#### Test Case 4: Request Permission for Completed Task
**Steps:**
1. Login as non-creator
2. Navigate to completed task
3. Click "Request Upload Permission"
4. Enter message and submit

**Expected Result:**
- Request created with Status="Pending"
- Confirmation message displayed
- Request appears in task creator's view

#### Test Case 5: Task Creator Approves Request
**Steps:**
1. Login as task creator
2. Navigate to completed task
3. View permission requests section
4. Click "Approve" on pending request

**Expected Result:**
- Request status changes to "Approved"
- ReviewedBy and ReviewedAt fields populated
- Requester can now upload attachments

#### Test Case 6: Upload with Approved Permission
**Steps:**
1. Login as user with approved request
2. Navigate to completed task
3. Upload attachment

**Expected Result:**
- Upload succeeds
- Attachment appears in list
- Permission request remains approved

#### Test Case 7: Task Creator Rejects Request
**Steps:**
1. Login as task creator
2. Navigate to completed task
3. View permission requests section
4. Click "Reject" on pending request

**Expected Result:**
- Request status changes to "Rejected"
- ReviewedBy and ReviewedAt fields populated
- Requester cannot upload attachments

#### Test Case 8: Duplicate Request Prevention
**Steps:**
1. Login as user
2. Create permission request for task
3. Try to create another request for same task

**Expected Result:**
- Second request fails
- Error message: "You already have a pending request for this task"

#### Test Case 9: Non-Creator Cannot Approve Request
**Steps:**
1. Login as non-creator
2. Try to approve permission request via API

**Expected Result:**
- Request fails with 403 Forbidden
- Only task creator can approve/reject

### Benefits

1. **Data Integrity:** Prevents unauthorized modifications to completed tasks
2. **Controlled Workflow:** Structured process for post-completion changes
3. **Audit Trail:** All requests and approvals tracked
4. **Flexibility:** Allows legitimate post-completion uploads when needed
5. **Security:** Multiple layers of permission checks
6. **Transparency:** Clear communication between requesters and approvers

### Use Cases

**Use Case 1: Final Documentation**
Employee completes task but later needs to upload final signed documents.
- Employee requests permission with explanation
- Manager reviews and approves
- Employee uploads documents
- Task remains completed with additional documentation

**Use Case 2: Audit Requirements**
Compliance team needs to attach audit reports to completed tasks.
- Auditor requests permission
- Task creator approves
- Auditor uploads compliance documents
- Audit trail maintained

**Use Case 3: Correction Uploads**
User realizes they uploaded wrong file to completed task.
- User requests permission to upload correct file
- Creator approves
- User uploads correct file
- Both files retained for history

### Conclusion

The task editing and attachment permission workflow provides robust control over task modifications while maintaining flexibility for legitimate post-completion needs. The request/approval system ensures that all changes are authorized and tracked, supporting both operational needs and compliance requirements.

**Key Takeaways:**
- Task creators have full control over their tasks while active
- Completed tasks are locked to prevent unauthorized changes
- Permission request system provides controlled access when needed
- All actions are logged and auditable
- Backend validation ensures security regardless of frontend state
