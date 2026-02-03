using System.IdentityModel.Tokens.Jwt;

namespace TenantApi.Shared.Constansts
{
    public class TenantClaims
    {
        public const string UserId = JwtRegisteredClaimNames.Sub;
        public const string Email = JwtRegisteredClaimNames.Email;
        public const string Street = "street";
        public const string Role = "role";
        public const string TenantId = "tid";
    }
}