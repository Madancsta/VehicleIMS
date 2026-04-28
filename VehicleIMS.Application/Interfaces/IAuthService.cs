using VehicleIMS.Application.DTOs;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
        Task<AuthResponseDTO> RefreshTokenAsync(string accessToken, string refreshToken);
        Task<AuthResponseDTO> LogoutAsync(string userId);
        Task<Users?> GetUserByIdAsync(string userId);
    }
}