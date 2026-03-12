using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Repositories;
using BCrypt.Net;

namespace TaskManagementAPI.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<object?> LoginAsync(string email, string password)
    {
        _logger.LogInformation("Attempting login for email: {Email}", email);

        var user = await _context.Users
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", email);
            return null;
        }

        _logger.LogInformation("Successful login for user: {UserId} ({Email})", user.Id, email);
        return new
        {
            id = user.Id,
            name = user.Name,
            email = user.Email,
            role = user.Role
        };
    }

    public Task LogoutAsync()
    {
        _logger.LogInformation("User logged out");
        return Task.CompletedTask;
    }

    public Task<object?> GetSessionAsync()
    {
        // Session management is handled in the controller
        return Task.FromResult<object?>(null);
    }
}
