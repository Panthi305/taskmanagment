using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
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

            // Admin can see all users (Manager and Employee)
            // Manager can only see Employees
            if (userRole == "Manager")
            {
                query = query.Where(u => u.Role == "Employee");
            }
            else if (userRole == "Admin")
            {
                query = query.Where(u => u.Role == "Manager" || u.Role == "Employee");
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
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("assignable")]
        public async Task<IActionResult> GetAssignableUsers()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (userId == null)
            {
                return Unauthorized();
            }

            // Only Admin and Manager can assign tasks
            if (userRole != "Admin" && userRole != "Manager")
            {
                return Forbid();
            }

            IQueryable<User> query = _context.Users;

            // Role-based filtering for assignable users
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
                // Employee cannot assign tasks - return empty list
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

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return Forbid();
            }

            var existingUser = await _context.Users
                .Where(u => u.Email == createUserDto.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }

            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                Role = createUserDto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
    }
}
