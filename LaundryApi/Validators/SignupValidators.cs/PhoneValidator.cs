using FluentValidation;
using LaundryBooking.Models;

namespace LaundryBooking.Validators
{
    public class PhoneNumberValidator : AbstractValidator<PhoneNumber>
    {
        public PhoneNumberValidator()
        {
            RuleFor(x => x.countryCode).NotEmpty()
            .WithMessage("Country code is required")
            .Matches(@"^\+\d{1,4}$")
            .WithMessage("Invalid country code format (e.g., +45)");

            RuleFor(x => x.number).NotEmpty()
            .WithMessage("Number is required")
            .Matches(@"^\d{6,15}$")
            .WithMessage("number must be between 6 - 15 digits");
        }
    }
}