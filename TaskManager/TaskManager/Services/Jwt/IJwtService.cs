using TaskManager.Models;

namespace TaskManager.Services.Jwt
{
    public interface IJwtService
    {
        Task<string> GenerateToken(User user);
    }
}