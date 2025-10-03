using System.ComponentModel.DataAnnotations;

namespace LaundryApi.PostgresModels
{
    public class Property
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string StreetName { get; set; } = "";

        [Required, MaxLength(50)]
        public string BuildingNumber { get; set; } = "";

        public ICollection<UserProperty> UserProperties { get; set; } = new List<UserProperty>();
    }
}