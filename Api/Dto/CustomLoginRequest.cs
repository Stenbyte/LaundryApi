namespace TenantApi.Dto
{
    public class CustomLoginRequest
    {
        public required string Email { get; init; }
        public required string Password { get; init; }

    }
}