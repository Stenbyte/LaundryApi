using FluentValidation;
using LaundryApi.Controllers;
using LaundryApi.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace LaundryApi.Validators
{
    public class SignUpValidator : AbstractValidator<User>
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

            // RuleFor(x => x.password)
            // .NotEmpty().WithMessage("Password is required")
            // .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            // .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            // .Matches(@"\d").WithMessage("Password must contain at least one number")
            // .Matches(@"[@$!%*?&]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.email).NotEmpty().WithMessage("email is required").Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Invalid email format");

            // RuleFor(x => x.phoneNumber).SetValidator(new PhoneNumberValidator());
            RuleFor(x => x.adress).SetValidator(new AdressValidator());
        }
    }

    public class LoginValidator : AbstractValidator<CustomLoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.email).NotEmpty().WithMessage("email is required").Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Invalid email format");

            // RuleFor(x => x.Password)
            // .NotEmpty().WithMessage("Password is required")
            // .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            // .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            // .Matches(@"\d").WithMessage("Password must contain at least one number")
            // .Matches(@"[@$!%*?&]").WithMessage("Password must contain at least one special character");
        }
    }

    public class LogOutValidator : AbstractValidator<LogOutRequest>
    {
        public LogOutValidator()
        {
            RuleFor(x => x.email).NotEmpty().WithMessage("email is required").Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Invalid email format");

        }
    }

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

    public class AdressValidator : AbstractValidator<Adress>
    {
        public AdressValidator()
        {
            RuleFor(x => x.streetName).NotEmpty()
            .WithMessage("Street name is required")
            .MaximumLength(100)
            .WithMessage("Max street name is 100 characters");

            RuleFor(x => x.buildingNumber).NotEmpty().Matches(@"^[1-9]\d*$")
            .WithMessage("House Number is required");
        }
    }

    public class TimeSlotValidator
    {
        public static bool IsTimeSlotInThePast(List<string> timeSlot)
        {

            var nowUtc = DateTime.UtcNow;
            string[] timeParts = timeSlot[0].Split("-");
            var endTime = timeParts[1];
            var endHour = int.Parse(endTime.Split(":")[0]);
            var endMinute = int.Parse(endTime.Split(":")[1]);

            var slotEndUtc = DateTime.UtcNow.AddHours(endHour).AddMinutes(endMinute);
            return slotEndUtc < nowUtc;
        }
    }

}