using LaundryBooking.Models;
using LaundryBooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaundryBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService) => _bookingService = bookingService;

        [HttpGet]
        public async Task<List<Booking>> Get() => await _bookingService.GetBookings();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Booking>> Get(string id)
        {
            var booking = await _bookingService.GetBookingById(id);
            if (booking is null)
                return NotFound();

            return booking;
        }
        [HttpPost]
        public async Task<IActionResult> Post(Booking newBooking)
        {
            await _bookingService.CreateBooking(newBooking);
            return CreatedAtAction(nameof(Get), new { id = newBooking._id }, newBooking);
        }
    }
}