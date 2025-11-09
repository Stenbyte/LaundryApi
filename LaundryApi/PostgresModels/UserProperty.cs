using System;
using System.ComponentModel.DataAnnotations;

namespace LaundryApi.PostgresModels
{
    public class UserProperty
    {
        public required Guid UserId { get; set; }
        public required UserPg User { get; set; }
        public required Guid PropertyId { get; set; }
        public required Property Property { get; set; }
    }
}
