using FluentValidation;
using LaundryApi.Models;

namespace LaundryApi.Validators
{
    public class AdressValidator : AbstractValidator<Adress>
    {
        public AdressValidator()
        {
            RuleFor(x => x.streetName).NotEmpty()
            .WithMessage("Street name is required")
            .MaximumLength(100)
            .WithMessage("Max street name is 100 characters");

            RuleFor(x => x.houseNumber).NotEmpty().Matches(@"^[1-9]\d*$")
            .WithMessage("House Number is required");
        }
    }
}