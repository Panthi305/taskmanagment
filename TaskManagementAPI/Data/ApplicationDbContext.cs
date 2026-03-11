using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

/*
 * ============================================================================
 * APPLICATION DB CONTEXT - ENTITY FRAMEWORK CORE DATABASE CONTEXT
 * ============================================================================
 * 
 * ENTITY FRAMEWORK CORE EXPLANATION:
 * ----------------------------------
 * EF Core is an Object-Relational Mapper (ORM) that:
 * - Maps C# classes (entities) to database tables
 * - Translates LINQ queries to SQL
 * - Tracks changes to entities
 * - Generates and executes SQL commands
 * - Manages database connections
 * - Handles transactions
 * 
 * DBCONTEXT ROLE:
 * --------------
 * DbContext is the bridge between your application and database.
 * It provides:
 * - DbSet<T> properties for each entity (table)
 * - SaveChanges() method to persist changes
 * - Change tracking for updates
 * - Query composition with LINQ
 * - Database connection management
 * 
 * DBSET EXPLANATION:
 * -----------------
 * DbSet<User> represents the Users table in database.
 * You can query it using LINQ:
 * - _context.Users.Where(u => u.Email == email)
 * - _context.Users.Include(u => u.AssignedTasks)
 * - _context.Users.FirstOrDefaultAsync()
 * 
 * EF Core translates these to SQL queries automatically.
 * 
 * ONMODELCREATING:
 * ---------------
 * This method configures entity relationships and constraints.
 * Called once when DbContext is first created.
 * Defines:
 * - Foreign key relationships
 * - Delete behaviors (Cascade, Restrict, SetNull)
 * - Indexes and constraints
 * - Table and column names (if different from class/property names)
 * 
 * ENTITY RELATIONSHIPS:
 * --------------------
 * This application has the following relationships:
 * 
 * 1. User → Tasks (One-to-Many via AssignedBy)
 *    - One user can create many tasks
 *    - DeleteBehavior.Restrict: Cannot delete user if they created tasks
 * 
 * 2. User → Tasks (One-to-Many via AssignedTo)
 *    - One user can be assigned many tasks
 *    - DeleteBehavior.Restrict: Cannot delete user if they have assigned tasks
 * 
 * 3. Task → TaskComments (One-to-Many)
 *    - One task can have many comments
 *    - DeleteBehavior.Cascade: Deleting task deletes all its comments
 * 
 * MIGRATIONS:
 * ----------
 * When you modify entities or relationships:
 * 1. Run: dotnet ef migrations add MigrationName
 * 2. EF Core compares current model with last migration
 * 3. Generates migration file with Up() and Down() methods
 * 4. Run: dotnet ef database update
 * 5. EF Core applies migration to database
 * 
 * Benefits:
 * - Version control for database schema
 * - Team collaboration on database changes
 * - Easy rollback with Down() method
 * - Automatic schema generation from code
 */

namespace TaskManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor receives options from dependency injection
        // Options include connection string and database provider
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties - each represents a table in database
        // EF Core creates tables named Users, Tasks, and TaskComments
        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }

        /*
         * ========================================================================
         * ON MODEL CREATING - CONFIGURE ENTITY RELATIONSHIPS
         * ========================================================================
         * 
         * This method uses Fluent API to configure entity relationships.
         * 
         * FLUENT API vs DATA ANNOTATIONS:
         * ------------------------------
         * Two ways to configure EF Core:
         * 
         * 1. Data Annotations (attributes on entity classes):
         *    [Required], [MaxLength], [ForeignKey], etc.
         *    - Simple and readable
         *    - Limited configuration options
         * 
         * 2. Fluent API (this method):
         *    - More powerful and flexible
         *    - Keeps entity classes clean
         *    - Preferred for complex relationships
         * 
         * RELATIONSHIP CONFIGURATION:
         * --------------------------
         * HasOne() - HasMany() - WithMany() pattern defines relationships
         * 
         * Example breakdown:
         * modelBuilder.Entity<TaskItem>()           // Configure TaskItem entity
         *     .HasOne(t => t.AssignedByUser)        // TaskItem has one User (creator)
         *     .WithMany(u => u.CreatedTasks)        // User has many created tasks
         *     .HasForeignKey(t => t.AssignedBy)     // Foreign key column
         *     .OnDelete(DeleteBehavior.Restrict);   // Cannot delete user if they created tasks
         * 
         * DELETE BEHAVIORS:
         * ----------------
         * - Cascade: Deleting parent deletes children (Task → Comments)
         * - Restrict: Cannot delete parent if children exist (User → Tasks)
         * - SetNull: Deleting parent sets foreign key to null
         * - NoAction: No automatic action (must handle manually)
         * 
         * WHY RESTRICT FOR USERS?
         * ----------------------
         * We use Restrict for User → Task relationships because:
         * - Preserves data integrity
         * - Prevents accidental data loss
         * - Maintains audit trail (who created/assigned tasks)
         * - If user needs to be deleted, tasks must be reassigned first
         * 
         * WHY CASCADE FOR COMMENTS?
         * ------------------------
         * We use Cascade for Task → Comments because:
         * - Comments don't make sense without their task
         * - Simplifies cleanup when task is deleted
         * - Comments are dependent data, not independent entities
         */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TaskItem → User relationship (AssignedBy)
            // One user can create many tasks
            // Restrict delete: Cannot delete user if they created tasks
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedByUser)           // Navigation property
                .WithMany(u => u.CreatedTasks)           // Inverse navigation
                .HasForeignKey(t => t.AssignedBy)        // Foreign key column
                .OnDelete(DeleteBehavior.Restrict);      // Delete behavior

            // Configure TaskItem → User relationship (AssignedTo)
            // One user can be assigned many tasks
            // Restrict delete: Cannot delete user if they have assigned tasks
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure TaskComment → Task relationship
            // One task can have many comments
            // Cascade delete: Deleting task deletes all its comments
            modelBuilder.Entity<TaskComment>()
                .HasOne(tc => tc.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(tc => tc.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
