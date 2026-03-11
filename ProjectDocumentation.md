# Internal Task Management System - Project Documentation

## Project Overview

The Internal Task Management System is a full-stack web application that enables organizations to manage task assignments and track progress across different user roles. The system implements role-based access control with three distinct user types: Admin, Manager, and Employee. Each role has specific permissions for creating, assigning, and managing tasks throughout their lifecycle.

## Technology Stack

### Frontend
- **Angular** (v21.2.1)
- **TypeScript**
- **Angular Components** (Standalone components)
- **Angular Routing**
- **Angular Services**
- **Angular HTTP Client**
- **Angular Forms** (Template-driven forms with two-way binding)

### Backend
- **ASP.NET Core Web API** (.NET 8.0)
- **Controllers** (RESTful API endpoints)
- **DTOs** (Data Transfer Objects)
- **LINQ** (Language Integrated Query)
- **Entity Framework Core** (v8.0.0)
- **EF Core Migrations**

### Database
- **SQL Server** (LocalDB)
- **Entity Framework Core** for ORM

### Authentication
- **Session-based Authentication**
- **Cookie-based Session Management**
- **Role-Based Authorization**
- **BCrypt** for password hashing

## System Architecture

### Backend Structure
```
TaskManagementAPI/
├── Controllers/
│   ├── AuthController.cs       # Authentication endpoints
│   ├── UserController.cs       # User management endpoints
│   └── TaskController.cs       # Task management endpoints
├── Data/
│   ├── ApplicationDbContext.cs # EF Core DbContext
│   └── DbSeeder.cs            # Database seeding
├── DTOs/
│   ├── LoginDto.cs
│   ├── CreateUserDto.cs
│   ├── CreateTaskDto.cs
│   ├── UserDto.cs
│   └── TaskDto.cs
├── Models/
│   ├── User.cs
│   ├── TaskItem.cs
│   └── TaskComment.cs
├── Migrations/
│   └── [EF Core Migration Files]
├── Program.cs                  # Application configuration
└── appsettings.json           # Configuration settings
```

### Frontend Structure
```
TaskManagementUI/src/app/
├── components/
│   ├── login/
│   │   ├── login.ts
│   │   ├── login.html
│   │   └── login.css
│   ├── dashboard/
│   │   ├── dashboard.ts
│   │   ├── dashboard.html
│   │   └── dashboard.css
│   ├── create-task/
│   │   ├── create-task.ts
│   │   ├── create-task.html
│   │   └── create-task.css
│   ├── task-list/
│   │   ├── task-list.ts
│   │   ├── task-list.html
│   │   └── task-list.css
│   └── my-tasks/
│       ├── my-tasks.ts
│       ├── my-tasks.html
│       └── my-tasks.css
├── services/
│   ├── auth.service.ts
│   ├── task.service.ts
│   └── user.service.ts
├── models/
│   ├── user.ts
│   └── task.ts
├── app.routes.ts
├── app.config.ts
├── app.ts
└── app.html
```

## Database Schema

### Users Table
| Column       | Type         | Description                    |
|--------------|--------------|--------------------------------|
| Id           | int          | Primary key (Identity)         |
| Name         | nvarchar     | User's full name               |
| Email        | nvarchar     | User's email (unique)          |
| PasswordHash | nvarchar     | BCrypt hashed password         |
| Role         | nvarchar     | Admin, Manager, or Employee    |
| CreatedAt    | datetime2    | Account creation timestamp     |

### Tasks Table
| Column        | Type         | Description                    |
|---------------|--------------|--------------------------------|
| Id            | int          | Primary key (Identity)         |
| Title         | nvarchar     | Task title                     |
| Description   | nvarchar     | Task description               |
| AssignedBy    | int          | Foreign key to Users (creator) |
| AssignedTo    | int          | Foreign key to Users (assignee)|
| Priority      | nvarchar     | Low, Medium, or High           |
| Status        | nvarchar     | Pending, In Progress, Completed|
| CreatedAt     | datetime2    | Task creation timestamp        |
| StartDate     | datetime2    | When task was started (nullable)|
| CompletedDate | datetime2    | When task was completed (nullable)|

### TaskComments Table
| Column    | Type         | Description                    |
|-----------|--------------|--------------------------------|
| Id        | int          | Primary key (Identity)         |
| TaskId    | int          | Foreign key to Tasks           |
| UserId    | int          | Foreign key to Users           |
| Comment   | nvarchar     | Comment text                   |
| CreatedAt | datetime2    | Comment creation timestamp     |

### Relationships
- **Users → Tasks (AssignedBy)**: One-to-Many (Restrict delete)
- **Users → Tasks (AssignedTo)**: One-to-Many (Restrict delete)
- **Tasks → TaskComments**: One-to-Many (Cascade delete)
- **Users → TaskComments**: One-to-Many (Cascade delete)

## API Endpoints

### Authentication Endpoints

#### POST /api/auth/login
Authenticates a user and creates a session.

**Request Body:**
```json
{
  "email": "admin@test.com",
  "password": "admin123"
}
```

**Response:**
```json
{
  "id": 1,
  "name": "Admin User",
  "email": "admin@test.com",
  "role": "Admin"
}
```

#### POST /api/auth/logout
Logs out the current user and clears the session.

**Response:**
```json
{
  "message": "Logged out successfully"
}
```

#### GET /api/auth/session
Retrieves the current user's session information.

**Response:**
```json
{
  "id": 1,
  "name": "Admin User",
  "role": "Admin"
}
```

### User Management Endpoints

#### GET /api/user
Retrieves all users (Admin and Manager only).

**Response:**
```json
[
  {
    "id": 1,
    "name": "Admin User",
    "email": "admin@test.com",
    "role": "Admin",
    "createdAt": "2026-03-11T18:38:32Z"
  }
]
```

#### POST /api/user
Creates a new user (Admin only).

**Request Body:**
```json
{
  "name": "New Employee",
  "email": "employee@test.com",
  "password": "password123",
  "role": "Employee"
}
```

#### DELETE /api/user/{id}
Deletes a user by ID (Admin only).

### Task Management Endpoints

#### GET /api/task
Retrieves tasks based on user role:
- **Admin**: All tasks
- **Manager**: Tasks assigned by the manager
- **Employee**: Tasks assigned to the employee

**Response:**
```json
[
  {
    "id": 1,
    "title": "Complete Project Documentation",
    "description": "Write comprehensive documentation",
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

#### POST /api/task
Creates a new task (Admin and Manager only).

**Request Body:**
```json
{
  "title": "Task Title",
  "description": "Task Description",
  "assignedTo": 3,
  "priority": "Medium"
}
```

#### PUT /api/task/start/{id}
Starts a task (changes status from Pending to In Progress).
- Only the assigned employee can start their task
- Task must be in Pending status

#### PUT /api/task/complete/{id}
Completes a task (changes status from In Progress to Completed).
- Only the assigned employee can complete their task
- Task must be in In Progress status

## Role Permissions

### Admin Role
- ✅ Create users
- ✅ Delete users
- ✅ View all users
- ✅ Create tasks
- ✅ Assign tasks to employees
- ✅ View all tasks
- ❌ Start tasks (only employees can start their assigned tasks)
- ❌ Complete tasks (only employees can complete their assigned tasks)

### Manager Role
- ❌ Create users
- ❌ Delete users
- ✅ View all users
- ✅ Create tasks
- ✅ Assign tasks to employees
- ✅ View tasks assigned by them
- ❌ Start tasks
- ❌ Complete tasks

### Employee Role
- ❌ Create users
- ❌ Delete users
- ❌ View all users
- ❌ Create tasks
- ❌ Assign tasks
- ✅ View tasks assigned to them
- ✅ Start their assigned tasks
- ✅ Complete their assigned tasks

## Task Status Workflow

The task lifecycle follows a strict workflow:

```
Pending → In Progress → Completed
```

### Status Transitions

1. **Pending → In Progress**
   - Triggered by: Employee clicking "Start Task"
   - Endpoint: PUT /api/task/start/{id}
   - Sets: StartDate to current timestamp
   - Validation: Only assigned employee can start

2. **In Progress → Completed**
   - Triggered by: Employee clicking "Complete Task"
   - Endpoint: PUT /api/task/complete/{id}
   - Sets: CompletedDate to current timestamp
   - Validation: Only assigned employee can complete

### Business Rules
- Tasks cannot skip statuses (e.g., Pending → Completed directly)
- Only the assigned employee can change task status
- Admins and Managers cannot complete tasks assigned to employees
- Status changes are tracked with timestamps

## LINQ Query Examples

The system extensively uses LINQ for data filtering and querying:

### Filter Tasks by Assigned User
```csharp
var tasks = await _context.Tasks
    .Where(t => t.AssignedTo == userId)
    .ToListAsync();
```

### Filter Pending Tasks
```csharp
var pendingTasks = await _context.Tasks
    .Where(t => t.Status == "Pending")
    .ToListAsync();
```

### Filter Tasks by Manager
```csharp
var managerTasks = await _context.Tasks
    .Where(t => t.AssignedBy == managerId)
    .ToListAsync();
```

### Complex Query with Includes
```csharp
var tasks = await _context.Tasks
    .Include(t => t.AssignedByUser)
    .Include(t => t.AssignedToUser)
    .Where(t => t.AssignedTo == userId.Value)
    .Select(t => new TaskDto
    {
        Id = t.Id,
        Title = t.Title,
        AssignedByName = t.AssignedByUser!.Name,
        AssignedToName = t.AssignedToUser!.Name,
        // ... other properties
    })
    .ToListAsync();
```

## Angular Features Implementation

### Components
All components are standalone components using Angular's modern architecture:
- **LoginComponent**: Handles user authentication
- **DashboardComponent**: Displays task statistics and overview
- **CreateTaskComponent**: Form for creating new tasks
- **TaskListComponent**: Displays all tasks in a table
- **MyTasksComponent**: Shows tasks assigned to current user with action buttons

### Routing
Configured in `app.routes.ts`:
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

### Data Binding

#### Two-Way Binding
Used in forms with `[(ngModel)]`:
```html
<input [(ngModel)]="email" name="email" />
```

#### Event Binding
Used for button clicks and form submissions:
```html
<button (click)="logout()">Logout</button>
<form (ngSubmit)="onSubmit()">
```

#### Property Binding
Used for conditional rendering:
```html
<button *ngIf="canStart(task)" (click)="startTask(task.id)">
```

### Services

#### AuthService
- Manages authentication state
- Stores current user in BehaviorSubject
- Provides login/logout functionality
- Checks user roles and permissions

#### TaskService
- Handles all task-related API calls
- CRUD operations for tasks
- Task status updates

#### UserService
- Manages user-related API calls
- User creation and deletion

### HTTP Client
All services use Angular's HttpClient with:
- `withCredentials: true` for session cookie handling
- Observable-based async operations
- Error handling with RxJS operators

## UI Flow

### 1. Login Flow
```
User enters credentials → AuthService.login() → 
Backend validates → Session created → 
Navigate to Dashboard
```

### 2. Dashboard Flow
```
Load user session → Fetch tasks based on role →
Calculate statistics → Display overview →
Provide navigation to other pages
```

### 3. Create Task Flow (Admin/Manager)
```
Navigate to Create Task → Load employee list →
Fill form → Submit → TaskService.createTask() →
Backend creates task → Redirect to Dashboard
```

### 4. Employee Task Flow
```
Navigate to My Tasks → Load assigned tasks →
Filter by status → Click "Start Task" →
Status changes to In Progress → Work on task →
Click "Complete Task" → Status changes to Completed
```

## How the System Works

### Authentication Flow
1. User submits login credentials through Angular form
2. AuthService sends POST request to `/api/auth/login`
3. Backend validates credentials using BCrypt
4. If valid, session is created with user ID and role
5. Session cookie is sent back to client
6. Angular stores user info in BehaviorSubject
7. All subsequent requests include session cookie

### Task Creation Flow
1. Admin/Manager navigates to Create Task page
2. UserService fetches list of employees
3. Manager fills task details and selects employee
4. TaskService sends POST request to `/api/task`
5. Backend validates user role (Admin/Manager only)
6. Task is created with status "Pending"
7. AssignedBy is set to current user ID
8. Task is saved to database via EF Core
9. Success message shown, redirect to dashboard

### Task Status Update Flow
1. Employee views their tasks in My Tasks page
2. For Pending tasks, "Start Task" button is visible
3. Employee clicks "Start Task"
4. TaskService sends PUT request to `/api/task/start/{id}`
5. Backend validates:
   - User is authenticated
   - Task is assigned to this user
   - Task status is "Pending"
6. If valid, status changes to "In Progress"
7. StartDate is set to current timestamp
8. Changes saved via EF Core
9. Task list refreshes to show updated status
10. "Complete Task" button now appears
11. Employee clicks "Complete Task"
12. Similar validation for completion
13. Status changes to "Completed"
14. CompletedDate is set

### Role-Based Access Control
1. Every API request checks session for user role
2. Controllers use `HttpContext.Session.GetString("UserRole")`
3. Unauthorized actions return 403 Forbidden
4. Angular components check role via AuthService
5. UI elements conditionally rendered based on role
6. Navigation guards could be added for route protection

## Demo Credentials

The system is seeded with the following test users:

| Role     | Email                  | Password     |
|----------|------------------------|--------------|
| Admin    | admin@test.com         | admin123     |
| Manager  | manager@test.com       | manager123   |
| Employee | employee1@test.com     | employee123  |
| Employee | employee2@test.com     | employee123  |

## Running the Application

### Backend (ASP.NET Core API)
```bash
cd TaskManagementAPI
dotnet run
```
API runs on: http://localhost:5150

### Frontend (Angular)
```bash
cd TaskManagementUI
ng serve
```
Application runs on: http://localhost:4200

### Database Migrations
```bash
cd TaskManagementAPI
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Key Features Demonstrated

### Entity Framework Core
- DbContext configuration
- Entity relationships (One-to-Many)
- Migrations for schema management
- LINQ queries for data filtering
- Include() for eager loading
- Cascade and Restrict delete behaviors

### ASP.NET Core
- RESTful API design
- Controller-based routing
- Session management
- CORS configuration
- Dependency injection
- DTO pattern for data transfer

### Angular
- Standalone components
- Reactive programming with RxJS
- Template-driven forms
- Two-way data binding
- Event binding
- HTTP client with credentials
- Service-based architecture
- Component communication
- Routing and navigation

### Security
- Password hashing with BCrypt
- Session-based authentication
- Role-based authorization
- CORS policy for API protection
- HttpOnly cookies for session

## Project Highlights

1. **Clean Architecture**: Separation of concerns with Controllers, Services, DTOs, and Models
2. **Type Safety**: TypeScript on frontend, strongly-typed C# on backend
3. **Modern Angular**: Standalone components, signals-ready architecture
4. **RESTful Design**: Proper HTTP methods and status codes
5. **Database Design**: Normalized schema with proper relationships
6. **Role-Based Security**: Granular permissions based on user roles
7. **Responsive UI**: Clean, modern interface with CSS styling
8. **Real-time Updates**: Task status changes reflect immediately
9. **Data Validation**: Both client-side and server-side validation
10. **Scalable Structure**: Easy to extend with new features

## Conclusion

This Internal Task Management System successfully demonstrates a complete full-stack application using the specified technology stack. It implements all required features including role-based access control, task lifecycle management, and LINQ-based data querying. The system follows best practices for both Angular and ASP.NET Core development, providing a solid foundation for enterprise task management needs.
