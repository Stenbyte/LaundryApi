using LaundryApi.Models;
using LaundryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using LaundryBooking.Services;
using System.Security.Claims;
using LaundryApi.Exceptions;

namespace LaundryBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController(LaundryService laundryService, IOptions<MongoDBSettings> mongoSettings) : ControllerBase
    {
        private readonly LaundryService _laundryService = laundryService;
        private readonly IOptions<MongoDBSettings> _mongoSettings = mongoSettings;

        [HttpGet("getAll")]
        public async Task<List<Booking>> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            User? user = await _laundryService.FindUserById<User>(userId!);
            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }

            var _bookingService = new BookingService(_mongoSettings, user.dbName);
            var bookings = await _bookingService.GetAll();
            return bookings;
        }

        //         [HttpGet("{id:length(24)}")]
        //         public async Task<ActionResult<Booking>> Get(string id)
        //         {
        //             var booking = await _laundryService.GetById<Booking>(mongoSettings.Value.BookingsCollectionName, id);
        //             if (booking is null)
        //                 return NotFound();
        // 
        //             return booking;
        //         }
        // [HttpPost]
        // public async Task<IActionResult> Post(Booking newBooking)
        // {
        //     await _laundryService.Create<Booking>(mongoSettings.Value.BookingsCollectionName, newBooking);
        //     return CreatedAtAction(nameof(Get), new { id = newBooking._id }, newBooking);
        // }
    }
}