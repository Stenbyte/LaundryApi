using TenantApi.Models;
using TenantApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TenantApi.Exceptions;
using TenantApi.Validators;
using TenantApi.Dto;


namespace TenantApi.SignUp.Controllers
{
    [Route("api/[controller]")]
    public class SignUpController(IOptions<MongoDBSettings> mongoSettings, SignUpValidator signupValidator, IBookingService bookingService, IUserService userService) : ControllerBase
    {

        private readonly IUserService _userService = userService;
        private readonly IBookingService _bookingService = bookingService;
        private readonly IOptions<MongoDBSettings> _mongoSettings = mongoSettings;
        private readonly SignUpValidator _validator = signupValidator;

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new CustomException("Validation", validationResult.Errors, 400);
            }

            var userExists = await _userService.FindUserByEmail(request.Email);
            if (userExists != null)
            {
                throw new CustomException("User already exists", "", 403);
            }

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            request.Password = hashPassword;



            Building building = new Building {
                StreetName = request.adress.StreetName,
                BuildingNumber = request.adress.BuildingNumber
            };

            Property property = new Property {
                UnitName = "ST.TH",
                Building = building,
                BuildingId = building.Id
            };

            UserPg user = new UserPg {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password
            };

            UserProperty userProperties = new UserProperty {
                User = user,
                UserId = user.Id,
                Property = property,
                PropertyId = property.Id
            };

            user.UserProperties.Add(userProperties);

            try
            {
                await _userService.Create(user);
            }
            catch (CustomException ex)
            {
                // Revisit, db throw another exception which is caught in global
                throw new CustomException("Unable to create new user", ex, 400);
            }
            return CreatedAtAction(nameof(CreateUser), new { _id = user.Id });
        }
    }
}