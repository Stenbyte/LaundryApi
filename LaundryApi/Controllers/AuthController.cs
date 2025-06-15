using Microsoft.AspNetCore.Mvc;
using LaundryApi.Services;
using LaundryApi.Models;
using LaundryApi.Validators;
using LaundryApi.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using LaundryApi.Helpers;

namespace LaundryApi.Controllers
{

    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly LaundryService _layndryService;
        private readonly LoginValidator _loginValidator;
        private readonly LogOutValidator _logOutValidator;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;


        public AuthController(JwtService jwtService, LaundryService laundryService, LoginValidator validator, IConfiguration configuration, LogOutValidator logOutValidator, IMemoryCache cache)
        {
            _cache = cache;
            _jwtService = jwtService;
            _layndryService = laundryService;
            _loginValidator = validator;
            _configuration = configuration;
            _logOutValidator = logOutValidator;
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
            var user = await _layndryService.FindUserByEmail<User>(request.email);
            // if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.password))
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
            await _layndryService.UpdateUser(user);

            new HelperFunctions().ResetFailedAttempts(request.email, _cache);

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationMinutes"]!))
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

            var existingUser = await _layndryService.FindUserByEmail<User>(request.email);
            if (existingUser == null) return Unauthorized();

            existingUser.refreshToken = null;

            await _layndryService.UpdateUser(existingUser);

            Response.Cookies.Delete("refresh_token");

            return Ok(new { message = "Logged out" });
        }

        [HttpGet("userInfo")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var streetName = User.FindFirst("streetName")?.Value;
            return Ok(new { userId, email, streetName });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            User? user = await _layndryService.FindUserByRefreshToken<User>(request.refreshToken);

            if (user == null || user.refreshTokenExpiry < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            var newAccessToken = _jwtService.GenerateJwtToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.refreshToken = newRefreshToken;
            user.refreshTokenExpiry = DateTime.UtcNow
            .AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!));

            await _layndryService.UpdateUser(user);

            Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationMinutes"]!))
            });

            return Ok(new { accessToken = newAccessToken });
        }
    }
}