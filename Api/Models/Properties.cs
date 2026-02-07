using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantApi.Models
{

    [Table("Buildings")]
    public class Building
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public required string StreetName { get; set; }

        [Required, MaxLength(16)]
        public required string BuildingNumber { get; set; }

        public ICollection<Property> Units { get; set; } = new List<Property>();
    }

    [Table("Properties")]
    public class Property
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public required string UnitName { get; set; }

        public Guid BuildingId { get; set; }
        public Building Building { get; set; } = null!;

        public ICollection<UserProperty> UserProperties { get; set; } = new List<UserProperty>();
    }
}