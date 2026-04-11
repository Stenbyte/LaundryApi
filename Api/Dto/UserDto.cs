using System.ComponentModel.DataAnnotations;
using TenantApi.Models;

namespace TenantApi.Dto
{
    public class CreateUserRequest
    {
        [Required, MaxLength(200)]
        public required string FirstName { get; set; }

        [Required, MaxLength(200)]
        public required string LastName { get; set; }

        [Required, MaxLength(200)]
        public required string Email { get; set; }

        [Required, MaxLength(24)]
        public required string Password { get; set; }

        public AdressDto adress { get; set; } = null!;
        public bool? IsAdmin { get; set; }
        public ICollection<UserProperty> UserProperties { get; set; } = new List<UserProperty>();
    }

    public class AdressDto
    {
        [Required, MaxLength(200)]
        public required string StreetName { get; set; }

        [Required, MaxLength(16)]
        public required string BuildingNumber { get; set; }
    }

}