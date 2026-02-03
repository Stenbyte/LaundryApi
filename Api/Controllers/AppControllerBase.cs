using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TenantApi.Shared.Constansts;

namespace TenantApi.Controllers
{
    public abstract class AppControllerBase : ControllerBase
    {
        protected string? UserId => User.FindFirstValue(TenantClaims.UserId);
        protected string? UserEmail => User.FindFirstValue(TenantClaims.Email);
        protected string? UserStreet => User.FindFirstValue(TenantClaims.Street);
        protected string? UserRole => User.FindFirstValue(TenantClaims.Role);
    }

}