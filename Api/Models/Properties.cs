using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantApi.Models
{

    [Table("buildings")]
    public class Building
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        [Column("street_name")]
        public required string StreetName { get; set; }

        [Required, MaxLength(16)]
        [Column("building_number")]
        public required string BuildingNumber { get; set; }
        public ICollection<Property> Units { get; set; } = new List<Property>();
    }

    [Table("properties")]
    public class Property
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        [Column("unit_name")]
        public required string UnitName { get; set; }

        [Required]
        [Column("building_id")]
        public required Guid BuildingId { get; set; }

        public Building Building { get; set; } = null!;

        public ICollection<UserProperty> UserProperty { get; set; } = new List<UserProperty>();
    }
}