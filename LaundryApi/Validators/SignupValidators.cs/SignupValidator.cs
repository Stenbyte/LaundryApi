using FluentValidation;
using LaundryBooking.Models;

namespace LaundryBooking.Validators
{
    public class SignUpValidator : AbstractValidator<SignUp>
    {
        public SignUpValidator()
        {
            RuleFor(x => x.firstName).NotEmpty()
            .WithMessage("First Name is required")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.lastName).NotEmpty()
            .WithMessage("Last Name is required")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"\d").WithMessage("Password must contain at least one number")
            .Matches(@"[@$!%*?&]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.phoneNumber).SetValidator(new PhoneNumberValidator());
            RuleFor(x => x.adress).SetValidator(new AdressValidator());
        }
    }
}