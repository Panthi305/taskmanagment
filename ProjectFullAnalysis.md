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
22. [Future Enhancements](#future-enhancements)

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
