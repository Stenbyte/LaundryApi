using LaundryBooking.Models;
using Laundry.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using LaundryBooking.Exceptions;
using MongoDB.Bson;
using LaundryBooking.Validators;

namespace LaundryBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignUpController(LaundryService laundryService, IOptions<MongoDBSettings> mongoSettings, SignUpValidator signupValidator) : ControllerBase
    {
        private readonly LaundryService _laundryService = laundryService;
        private readonly IOptions<MongoDBSettings> _mongoSettings = mongoSettings;
        private readonly SignUpValidator _validator = signupValidator;

        [HttpPost]
        public async Task<IActionResult> Post(SignUpUser newUser)
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

            try
            {
                await _laundryService.CreateUser(_mongoSettings.Value.UsersCollectionName, newUser);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Unable to create new user", ex, 400);
            }
            return CreatedAtAction(nameof(Post), new { id = newUser.Id });
        }
    }
}