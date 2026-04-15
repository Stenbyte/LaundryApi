using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TenantApi.Models
{

    [Table("users")]
    public class UserPg
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        [Column("first_name")]
        public required string FirstName { get; set; }

        [Required, MaxLength(200)]
        [Column("last_name")]
        public required string LastName { get; set; }

        [Required, MaxLength(200)]
        [Column("email")]
        public required string Email { get; set; }

        [Required, MaxLength(100)]
        [Column("password")]
        public required string Password { get; set; }

        public string? refreshToken { get; set; }

        public DateTime? refreshTokenExpiry { get; set; }

        [Column("is_admin")]
        public bool? IsAdmin { get; set; }

        public ICollection<UserProperty> UserProperties { get; set; } = new List<UserProperty>();

    }
}
