using LaundryBooking.Models;
using Laundry.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using LaundryBooking.Exceptions;
using MongoDB.Bson;

namespace LaundryBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignUpController(LaundryService laundryService, IOptions<MongoDBSettings> mongoSettings) : ControllerBase
    {
        private readonly LaundryService _laundryService = laundryService;
        private readonly IOptions<MongoDBSettings> _mongoSettings = mongoSettings;

        [HttpPost]
        public async Task<IActionResult> Post(SignUp newUser)
        {
            if (string.IsNullOrEmpty(newUser.Id))
            {
                newUser.Id = ObjectId.GenerateNewId().ToString();
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