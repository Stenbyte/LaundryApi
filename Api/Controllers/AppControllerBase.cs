using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TenantApi.Controllers
{
    public abstract class AppControllerBase : ControllerBase
    {
        protected string? UserId => User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        protected string? UserEmail => User.FindFirstValue(JwtRegisteredClaimNames.Email);
        protected string? UserStreet => User.FindFirstValue("street");
        protected string? UserRole => User.FindFirstValue("role");
    }

}