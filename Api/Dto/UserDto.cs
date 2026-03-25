using System.ComponentModel.DataAnnotations;
using TenantApi.Models;

public class CreateUserRequest
{
    [Required, MaxLength(100)]
    public required string FirstName { get; set; }

    [Required, MaxLength(100)]
    public required string LastName { get; set; }

    [Required, MaxLength(100)]
    public required string Email { get; set; }

    [Required, MaxLength(16)]
    public required string Password { get; set; }

    public bool? IsAdmin { get; set; }
    public ICollection<UserProperty> UserProperties { get; set; } = new List<UserProperty>();
}