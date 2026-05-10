using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ICustomerRepository _customerRepository;

        public AuthService(
            UserManager<Users> userManager,
            RoleManager<Role> roleManager,
            SignInManager<Users> signInManager,
            IJwtService jwtService,
            IConfiguration configuration,
            ICustomerRepository customerRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _configuration = configuration;
            _customerRepository = customerRepository;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {
            // Check if user exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "User with this email already exists!"
                };
            }

            existingUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUser != null)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Username is already taken!"
                };
            }

            // Create new user
            var user = new Users
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Address = registerDto.Address,
                PhoneNumber = registerDto.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = $"Registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                };
            }

            // Assign default role "User" if it exists
            const string defaultRole = "User";
            if (await _roleManager.RoleExistsAsync(defaultRole))
            {
                await _userManager.AddToRoleAsync(user, defaultRole);
            }

            // Generate token
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Store refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                Convert.ToDouble(_configuration["JWT:RefreshTokenValidityInDays"] ?? "7"));
            await _userManager.UpdateAsync(user);

            return new AuthResponseDTO
            {
                Success = true,
                Message = "Registration successful!",
                Token = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"] ?? "60")),
                UserId = user.Id.ToString(),
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList()
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            // Find user by username or email
            var user = await _userManager.FindByNameAsync(loginDto.UserName)
                       ?? await _userManager.FindByEmailAsync(loginDto.UserName);

            if (user == null)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid username or password!"
                };
            }

            if (user.Status != UserStatus.Active)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Your account is deactivated. Please contact administrator."
                };
            }

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid username or password!"
                };
            }

            // Generate tokens
            var roles = await _userManager.GetRolesAsync(user);

            int? customerId = null;

            if (roles.Contains("Customer"))
            {
                var customer = await _customerRepository.GetByUserIdAsync(user.Id);
                customerId = customer?.CustomerId;
            }

            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Store refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                Convert.ToDouble(_configuration["JWT:RefreshTokenValidityInDays"] ?? "7"));
            await _userManager.UpdateAsync(user);

            return new AuthResponseDTO
            {
                Success = true,
                Message = "Login successful!",
                Token = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"] ?? "60")),
                UserId = user.Id.ToString(),
                Email = user.Email,
                UserName = user.UserName,
            };
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            // Clean the tokens
            accessToken = accessToken?.Trim().Trim('"') ?? string.Empty;
            refreshToken = refreshToken?.Trim().Trim('"') ?? string.Empty;

            // Validate formats
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Access token is required"
                };
            }

            // Check JWT format (should have 3 segments)
            var tokenSegments = accessToken.Split('.');
            if (tokenSegments.Length != 3)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = $"Invalid access token format. Expected 3 segments, got {tokenSegments.Length}. Make sure you're sending the JWT access token."
                };
            }

            try
            {
                var principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);
                if (principal == null)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "Invalid access token"
                    };
                }

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "Invalid token claims"
                    };
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "Invalid refresh token"
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);

                int? customerId = null;

                if (roles.Contains("Customer"))
                {
                    var customer = await _customerRepository.GetByUserIdAsync(user.Id);
                    customerId = customer?.CustomerId;
                }

                var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                    Convert.ToDouble(_configuration["JWT:RefreshTokenValidityInDays"] ?? "7"));
                await _userManager.UpdateAsync(user);

                return new AuthResponseDTO
                {
                    Success = true,
                    Message = "Token refreshed successfully!",
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(
                        Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"] ?? "60")),
                    UserId = user.Id.ToString(),
                    Email = user.Email,
                    UserName = user.UserName,
                };
            }
            catch (SecurityTokenException ex)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = $"Token validation failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDTO> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);
            }

            return new AuthResponseDTO
            {
                Success = true,
                Message = "Logged out successfully!"
            };
        }

        public async Task<Users?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}