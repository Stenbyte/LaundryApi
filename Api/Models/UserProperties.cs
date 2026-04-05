using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantApi.Models
{
    [Table("user_properties")]
    public class UserProperty
    {
        [Required, ForeignKey(nameof(UserPg))]
        [Column("user_id")]
        public required Guid UserId { get; set; }

        [Required]
        public required UserPg User { get; set; }

        [Required, ForeignKey(nameof(Property))]
        [Column("property_id")]
        public required Guid PropertyId { get; set; }

        public required Property Property { get; set; }
    }
}
