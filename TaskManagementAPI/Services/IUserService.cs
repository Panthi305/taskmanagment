using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> CreateUserAsync(CreateUserDto dto);
    Task<bool> DeleteUserAsync(int id);
}
