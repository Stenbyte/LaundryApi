using Microsoft.AspNetCore.Mvc;
using TenantApi.Services;
using TenantApi.Models;
using TenantApi.Validators;
using TenantApi.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using TenantApi.Helpers;
using TenantApi.Controllers;
using TenantApi.Shared.Constansts;


namespace TenantApi.Auth.Controllers
{

    [Route("api/[controller]")]
    public class AuthController : AppControllerBase
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

            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new CustomException("Validation", validationResult.Errors, 400);
            }

            string cachKey = $"lockout_{request.email}";

            if (_cache.TryGetValue(cachKey, out DateTime lockOutTime))
            {
                if (lockOutTime > DateTime.UtcNow)
                {
                    throw new CustomException("Email is locked for 10 min", null, 403);
                }
            }

            var user = await _userService.FindUserByEmail(request.email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.password, user.password))
            {
                new HelperFunctions().TrackFailedAttempt(request.email, _cache);
                return Unauthorized(new { message = "Invalid credentials" });
            }
            var token = _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var jwtSettings = _configuration.GetSection("JwtSettings");

            user.refreshToken = null;
            user.refreshToken = refreshToken;
            user.refreshTokenExpiry = DateTime.UtcNow
            .AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!));

            await _userService.UpdateUser(user);

            new HelperFunctions().ResetFailedAttempts(request.email, _cache);

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!)),
                Path = "/api/auth"
            });

            return Ok(new { token });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            string email = User.FindFirstValue(TenantClaims.Email) ?? "";

            var existingUser = await _userService.FindUserByEmail(email);
            if (existingUser != null)
            {
                existingUser.refreshToken = null;
                await _userService.UpdateUser(existingUser);
            }

            var cookieOptions = new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/auth"
            };
            Response.Cookies.Delete("refresh_token", cookieOptions);

            return Ok(new { message = "Logged out" });
        }

        [HttpGet("userInfo")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirstValue(TenantClaims.UserId);

            var email = User.FindFirstValue(TenantClaims.Email);

            var streetName = User.FindFirstValue("street");
            // here call to db 
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

            if (user == null || user.refreshTokenExpiry < DateTime.UtcNow.AddSeconds(30))
            {
                return Unauthorized(new { message = "Invalid token" });
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
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!)),
                Path = "/api/auth"
            });

            return Ok(new { accessToken = newAccessToken });
        }
    }
}



// add this later 

// public async Task<IActionResult> RefreshToken()
// {
//     if (!Request.Cookies.TryGetValue("refresh_token", out string? refreshToken))
//         return Unauthorized();
// 
//     // 1. Find the token in your database/collection
//     var tokenRecord = await _userService.GetTokenWithUser(refreshToken);
// 
//     if (tokenRecord == null)
//         return Unauthorized("Invalid Token");
// 
//     // 2. DETECT REUSE (The Security Recommendation)
//     if (tokenRecord.IsUsed || tokenRecord.IsRevoked)
//     {
//         // This token has been used before! 
//         // This means either a race condition OR an attacker stole it.
//         // ACTION: Revoke ALL tokens for this user to be safe.
//         await _userService.RevokeAllTokensForUser(tokenRecord.UserId);
// 
//         return Unauthorized("Security Alert: Session compromised.");
//     }
// 
//     // 3. Handle the "Grace Period" (The Race Condition Fix)
//     // If the token was created < 30 seconds ago, you could allow it, 
//     // but usually, with Rotation, you just move to the new one.
// 
//     // 4. Rotate
//     var newAccessToken = _jwtService.GenerateJwtToken(tokenRecord.User);
//     var newRefreshTokenString = _jwtService.GenerateRefreshToken();
// 
//     // 5. Update the OLD token and save the NEW one
//     tokenRecord.IsUsed = true;
//     tokenRecord.ReplacedByToken = newRefreshTokenString;
// 
//     await _userService.SaveNewRefreshToken(tokenRecord.UserId, newRefreshTokenString);
//     await _userService.UpdateTokenRecord(tokenRecord);
// 
//     // ... append cookie and return Ok ...
// }