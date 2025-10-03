using System;
using System.ComponentModel.DataAnnotations;

namespace LaundryApi.PostgresModels
{
    public class UserProperty
    {
        public Guid UserId { get; set; }
        public UserPg User { get; set; } = null!;
        public Guid PropertyId { get; set; }
        public Property Property { get; set; } = null!;
    }
}
