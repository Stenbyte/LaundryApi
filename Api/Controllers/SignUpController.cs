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
        public async Task<IActionResult> CreateUser(User newUser)
        {
            if (string.IsNullOrEmpty(newUser._id))
            {
                newUser._id = ObjectId.GenerateNewId().ToString();
            }
            var validationResult = await _validator.ValidateAsync(newUser);

            if (!validationResult.IsValid)
            {
                throw new CustomException("Validation", validationResult.Errors, 400);
            }

            var userExists = await _userService.FindUserByEmail(newUser.email);
            if (userExists != null)
            {
                throw new CustomException("User already exists", "", 403);
            }

            await AddDbNameToUser(newUser);
            MachineModel newMachine = new MachineModel() {
                status = MachineStatus.available,
                name = MachineName.washing,
                buildingId = newUser.adress._id!,
                _id = ObjectId.GenerateNewId().ToString()
            };
            await _bookingService.CreateMachine(newUser.dbName, newMachine);
            // enable it only when admin panel will be ready or com up with better idea
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(newUser.password);
            newUser.password = hashPassword;

            // add here adding machine collection for same db

            try
            {
                await _userService.CreateUser(_mongoSettings.Value.UsersCollectionName, newUser);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Unable to create new user", ex, 400);
            }
            return CreatedAtAction(nameof(CreateUser), new { _id = newUser._id });
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