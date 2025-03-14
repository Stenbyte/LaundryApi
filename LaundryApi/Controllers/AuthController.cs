using Microsoft.AspNetCore.Mvc;
using LaundryApi.Services;
using Microsoft.AspNetCore.Identity.Data;
using LaundryApi.Models;
using LaundryApi.Validators;
using LaundryApi.Exceptions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace LaundryApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Consumes("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly LaundryService _layndryService;
        private readonly LoginValidator _validator;
        private readonly IConfiguration _configuration;


        public AuthController(JwtService jwtService, LaundryService laundryService, LoginValidator validator, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _layndryService = laundryService;
            _validator = validator;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new CustomException("Validation", validationResult.Errors, 400);
            }
            var user = await _layndryService.FindUserByEmail<User>(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
            var token = _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.refreshToken = refreshToken;
            user.refreshTokenExpiry = DateTime.UtcNow
            .AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!));
            await _layndryService.UpdateUser(user);

            Response.Cookies.Append("access_token", token, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationMinutes"]!))
            });

            return Ok(new { refreshToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LoginRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new CustomException("Validation", validationResult.Errors, 400);
            }

            var existingUser = await _layndryService.FindUserByEmail<User>(request.Email);
            if (existingUser == null) return Unauthorized();

            existingUser.refreshToken = null;

            await _layndryService.UpdateUser(existingUser);

            Response.Cookies.Delete("access_token");

            return Ok(new { message = "Logged out" });
        }

        [HttpGet("userInfo")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
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

            Response.Cookies.Append("access_token", newAccessToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationMinutes"]!))
            });

            return Ok(new { refreshToken = newRefreshToken });
        }
    }

    public class TokenRequest
    {
        public required string refreshToken;
    }
}