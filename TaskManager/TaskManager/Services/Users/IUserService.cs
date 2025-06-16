using TaskManager.DTOs;

namespace TaskManager.Services.Users
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterDTO registerDTO);
        Task<LoginResponseDto> LoginAsync(LoginDTO loginDto);
    }
}
