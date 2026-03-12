namespace TaskManagementAPI.Services;

public interface IAuthService
{
    Task<object?> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<object?> GetSessionAsync();
}
