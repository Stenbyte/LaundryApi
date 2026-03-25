using TenantApi.Models;
using TenantApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TenantApi.Exceptions;
using MongoDB.Bson;
using TenantApi.Validators;
using TenantApi.Enums;


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
            //             var validationResult = await _validator.ValidateAsync(newUser);
            // 
            //             if (!validationResult.IsValid)
            //             {
            //                 throw new CustomException("Validation", validationResult.Errors, 400);
            //             }

            var userExists = await _userService.FindUserByEmail(request.Email);
            if (userExists != null)
            {
                throw new CustomException("User already exists", "", 403);
            }

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            request.Password = hashPassword;

            UserPg user = new UserPg {
                FirstName = request.FirstName,
                LastName = request.LastName
            }

            try
            {
                await _userService.Create(newUser);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Unable to create new user", ex, 400);
            }
            return CreatedAtAction(nameof(CreateUser), new { _id = newUser.Id });
        }

        private async Task<User> AddDbNameToUser(User newUser)
        {
            var existingUserWithSameDb = await _userService.FindExistingUserWithDbName(newUser);
            if (existingUserWithSameDb != null)
            {
                newUser.dbName = existingUserWithSameDb.dbName;
                newUser.adress._id = existingUserWithSameDb.adress._id;
            }
            else
            {
                newUser.dbName = $"Laundry_{newUser.adress.streetName}";
                newUser.adress._id = ObjectId.GenerateNewId().ToString();
            }
            return newUser;
        }
    }
}