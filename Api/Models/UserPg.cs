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

        [Required, MaxLength(100)]
        [Column("first_name")]
        public required string FirstName { get; set; }

        [Required, MaxLength(100)]
        [Column("last_name")]
        public required string LastName { get; set; }

        [Required, MaxLength(100)]
        [Column("email")]
        public required string Email { get; set; }

        [Required, MaxLength(24)]
        [Column("password")]
        public required string Password { get; set; }

        [Column("is_admin")]
        public bool? IsAdmin { get; set; }

        public ICollection<UserProperty> UserProperties { get; set; } = new List<UserProperty>();

    }
}
