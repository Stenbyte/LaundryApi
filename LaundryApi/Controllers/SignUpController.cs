using LaundryBooking.Models;
using Laundry.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LaundryBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignUpController(LaundryService laundryService) : ControllerBase
    {
        private readonly LaundryService _laundryService = laundryService;
        private readonly IOptions<MongoDBSettings> mongoSettings = null!;

        [HttpPost]
        public async Task<IActionResult> Post(SignUp newUser)
        {
            await _laundryService.Create<SignUp>(mongoSettings.Value.UsersCollectionName, newUser);
            return CreatedAtAction("Create", new { id = newUser.Id });
        }
    }
}