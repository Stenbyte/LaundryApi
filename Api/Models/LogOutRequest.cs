namespace TenantApi.Models
{
    public class LogOutRequest
    {
        /// <summary>
        /// The user's email address which acts as a user name.
        /// </summary>
        public required string email { get; init; }
    }
}