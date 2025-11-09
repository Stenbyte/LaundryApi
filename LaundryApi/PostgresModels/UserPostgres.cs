namespace LaundryApi.PostgresModels
{
    using System.ComponentModel.DataAnnotations;

    public class UserPg
    {
        [Key]
        public required Guid Id { get; set; } = Guid.NewGuid();

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
}
