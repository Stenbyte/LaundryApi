using LaundryApi.Models;
using LaundryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using LaundryApi.Exceptions;
using MongoDB.Bson;
using LaundryApi.Validators;


namespace LaundryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignUpController(LaundryService laundryService, IOptions<MongoDBSettings> mongoSettings, SignUpValidator signupValidator) : ControllerBase
    {
        private readonly LaundryService _laundryService = laundryService;
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

            var userExists = await _laundryService.FindUserByEmail<User>(newUser.email);
            if (userExists != null)
            {
                throw new CustomException("User already exists", "", 403);
            }

            await AddDbNameToUser(newUser);
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(newUser.password);
            newUser.password = hashPassword;

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
            var existingUser = await _laundryService.FindExistingUserWithDbName(newUser);
            if (existingUser != null)
            {
                newUser.dbName = existingUser.dbName;
            }
            else
            {
                newUser.dbName = $"Laundry_{newUser.adress.streetName.Replace(" ", "_")}";
            }
            return newUser;
        }
    }
}