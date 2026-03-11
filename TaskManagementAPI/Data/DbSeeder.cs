using TaskManagementAPI.Models;

namespace TaskManagementAPI.Data
{
    public static class DbSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Name = "Admin User",
                        Email = "admin@test.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Role = "Admin",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Name = "Manager User",
                        Email = "manager@test.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                        Role = "Manager",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Name = "Employee One",
                        Email = "employee1@test.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("employee123"),
                        Role = "Employee",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Name = "Employee Two",
                        Email = "employee2@test.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("employee123"),
                        Role = "Employee",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
    }
}
