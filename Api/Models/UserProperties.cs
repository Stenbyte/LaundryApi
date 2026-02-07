using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantApi.Models
{
    [Table("UserSProperties")]
    public class UserProperty
    {
        [Required, ForeignKey(nameof(UserPg))]
        public required Guid UserId { get; set; }

        [Required]
        public required UserPg User { get; set; }

        [Required, ForeignKey(nameof(Property))]
        public required Guid PropertyId { get; set; }

        [Required]
        public required Property Property { get; set; }
    }
}
