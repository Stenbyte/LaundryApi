using LaundryApi.Models;
using LaundryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using LaundryApi.Exceptions;
using MongoDB.Bson;
using LaundryApi.Validators;
using LaundryApi.Enums;


namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    public class SignUpController(ILaundryService laundryService, IOptions<MongoDBSettings> mongoSettings, SignUpValidator signupValidator, IBookingService bookingService) : ControllerBase
    {
        private readonly ILaundryService _laundryService = laundryService;
        private readonly IBookingService _bookingService = bookingService;
        private readonly IOptions<MongoDBSettings> _mongoSettings = mongoSettings;
        private readonly SignUpValidator _validator = signupValidator;

        [HttpPost]
        public async Task<IActionResult> CreateUser(User newUser)
        {
            if (string.IsNullOrEmpty(newUser.id))
            {
                newUser.id = ObjectId.GenerateNewId().ToString();
            }
            var validationResult = await _validator.ValidateAsync(newUser);

            if (!validationResult.IsValid)
            {
                throw new CustomException("Validation", validationResult.Errors, 400);
            }

            var userExists = await _laundryService.FindUserByEmail(newUser.email);
            if (userExists != null)
            {
                throw new CustomException("User already exists", "", 403);
            }

            await AddDbNameToUser(newUser);
            MachineModel newMachine = new MachineModel() {
                status = MachineStatus.available,
                name = MachineName.washingMachine,
                buildingId = newUser.adress.id!,
                id = ObjectId.GenerateNewId().ToString()
            };
            await _bookingService.CreateMachine(newUser.dbName, newMachine);
            // enable it only when admin panel will be ready or com up with better idea
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(newUser.password);
            newUser.password = hashPassword;

            // add here adding machine collection for same db

            try
            {
                await _laundryService.CreateUser(_mongoSettings.Value.UsersCollectionName, newUser);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Unable to create new user", ex, 400);
            }
            return CreatedAtAction(nameof(CreateUser), new { id = newUser.id });
        }

        private async Task<User> AddDbNameToUser(User newUser)
        {
            var existingUserWithSameDb = await _laundryService.FindExistingUserWithDbName(newUser);
            if (existingUserWithSameDb != null)
            {
                newUser.dbName = existingUserWithSameDb.dbName;
                newUser.adress.id = existingUserWithSameDb.adress.id;
            }
            else
            {
                newUser.dbName = $"Laundry_{newUser.adress.streetName}";
                newUser.adress.id = ObjectId.GenerateNewId().ToString();
            }
            return newUser;
        }
    }
}