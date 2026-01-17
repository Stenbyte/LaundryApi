using Microsoft.AspNetCore.Mvc;
using LaundryApi.Services;
using LaundryApi.Models;
using LaundryApi.Validators;
using LaundryApi.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using LaundryApi.Helpers;
using System.IdentityModel.Tokens.Jwt;

namespace LaundryApi.Controllers
{

    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IUserService _userService;
        private readonly LoginValidator _loginValidator;
        private readonly LogOutValidator _logOutValidator;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;


        public AuthController(JwtService jwtService, LoginValidator validator, IConfiguration configuration, LogOutValidator logOutValidator, IMemoryCache cache, IUserService userService)
        {
            _cache = cache;
            _jwtService = jwtService;
            _loginValidator = validator;
            _configuration = configuration;
            _logOutValidator = logOutValidator;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CustomLoginRequest request)
        {
            string cachKey = $"lockout_{request.email}";

            if (_cache.TryGetValue(cachKey, out DateTime lockOutTime))
            {
                if (lockOutTime > DateTime.UtcNow)
                {
                    throw new CustomException("Email is locked for 10 min", null, 403);
                }
            }
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new CustomException("Validation", validationResult.Errors, 400);
            }
            var user = await _userService.FindUserByEmail(request.email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.password, user.password))
                if (user == null)
                {
                new HelperFunctions().TrackFailedAttempt(request.email, _cache);
                return Unauthorized(new { message = "Invalid credentials" });
            }
            var token = _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.refreshToken = refreshToken;
            user.refreshTokenExpiry = DateTime.UtcNow
            .AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!));
            await _userService.UpdateUser(user);

            new HelperFunctions().ResetFailedAttempts(request.email, _cache);

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!))
            });

            return Ok(new { token });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogOutRequest request)
        {
            var validationResult = await _logOutValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new CustomException("Validation", validationResult.Errors, 400);
            }

            var existingUser = await _userService.FindUserByEmail(request.email);
            if (existingUser == null) return Unauthorized();

            existingUser.refreshToken = null;

            await _userService.UpdateUser(existingUser);

            Response.Cookies.Delete("refresh_token");

            return Ok(new { message = "Logged out" });
        }

        [HttpGet("userInfo")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? User.Identity?.Name;

            var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                        ?? User.FindFirst(ClaimTypes.Email)?.Value;

            var streetName = User.FindFirst("streetName")?.Value;
            return Ok(new { userId, email, streetName });
        }


        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("refresh_token", out string? refreshToken))
            {
                return Unauthorized(new { message = "Refresh token cookie missing" });
            }
            var jwtSettings = _configuration.GetSection("JwtSettings");
            User? user = await _userService.FindUserByRefreshToken(refreshToken);

            if (user == null || user.refreshTokenExpiry < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            var newAccessToken = _jwtService.GenerateJwtToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.refreshToken = newRefreshToken;
            user.refreshTokenExpiry = DateTime.UtcNow
            .AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!));

            await _userService.UpdateUser(user);

            Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!))
            });

            return Ok(new { accessToken = newAccessToken });
        }
    }
}