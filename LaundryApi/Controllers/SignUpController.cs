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
        public async Task<IActionResult> Post(User newUser)
        {
            if (string.IsNullOrEmpty(newUser.Id))
            {
                newUser.Id = ObjectId.GenerateNewId().ToString();
            }
            var validationResult = await _validator.ValidateAsync(newUser);

            if (!validationResult.IsValid)
            {
                throw new CustomException("Validation", validationResult.Errors, 400);
            }

            var isUserExist = await _laundryService.FindUser(newUser);

            try
            {
                AddDbNameToUser(newUser, isUserExist);
                await _laundryService.CreateUser(_mongoSettings.Value.UsersCollectionName, newUser);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Unable to create new user", ex, 400);
            }
            return CreatedAtAction(nameof(Post), new { id = newUser.Id });
        }

        private static User AddDbNameToUser(User newUser, User? existingUser)
        {
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