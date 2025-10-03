namespace LaundryApi.PostgresModels
{
    using System.ComponentModel.DataAnnotations;

    public class UserPg
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(100)]
        public string LastName { get; set; } = "";

        [Required, MaxLength(200)]
        public string Email { get; set; } = "";

        [Required]
        public required string Password { get; set; } = "";

        public bool IsAdmin { get; set; } = false;

        public ICollection<UserProperty> UserProperties { get; set; } = new List<UserProperty>();

    }
}
