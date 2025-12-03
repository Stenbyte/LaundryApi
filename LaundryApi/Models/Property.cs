using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundryApi.Models
{
    [Table("Properties")]
    public class Property
    {
        [Required, Key]
        public required Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public required string StreetName { get; set; }

        [Required, MaxLength(16)]
        public required string BuildingNumber { get; set; }

        public ICollection<UserProperty> UserProperties { get; set; } = new List<UserProperty>();
    }
}