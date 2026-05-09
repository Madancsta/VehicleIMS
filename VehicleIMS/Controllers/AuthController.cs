using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                throw new UnauthorizedAccessException(result.Message);
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            // Add debugging
            Console.WriteLine($"=== Refresh Token Request ===");
            Console.WriteLine($"AccessToken received: {(string.IsNullOrEmpty(refreshTokenDto.AccessToken) ? "NULL or EMPTY" : $"Length: {refreshTokenDto.AccessToken.Length}, Preview: {refreshTokenDto.AccessToken.Substring(0, Math.Min(50, refreshTokenDto.AccessToken.Length))}")}");
            Console.WriteLine($"RefreshToken received: {(string.IsNullOrEmpty(refreshTokenDto.RefreshToken) ? "NULL or EMPTY" : $"Length: {refreshTokenDto.RefreshToken.Length}")}");

            // Validate input
            if (string.IsNullOrWhiteSpace(refreshTokenDto.AccessToken))
            {
                return BadRequest(new { success = false, message = "Access token is required" });
            }

            if (string.IsNullOrWhiteSpace(refreshTokenDto.RefreshToken))
            {
                return BadRequest(new { success = false, message = "Refresh token is required" });
            }

            // Check if access token has 3 segments
            var segments = refreshTokenDto.AccessToken.Trim().Split('.');
            if (segments.Length != 3)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Invalid access token format. Expected 3 segments, got {segments.Length}. Please ensure you're sending the JWT access token, not the refresh token."
                });
            }

            var result = await _authService.RefreshTokenAsync(refreshTokenDto.AccessToken, refreshTokenDto.RefreshToken);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User not found" });
            }

            var result = await _authService.LogoutAsync(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Address,
                user.PhoneNumber,
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)
            });
        }
    }
}