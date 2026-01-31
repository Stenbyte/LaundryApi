namespace TenantApi.Models
{
    public class CustomLoginRequest
    {
        public required string email { get; init; }
        public required string password { get; init; }

    }
}