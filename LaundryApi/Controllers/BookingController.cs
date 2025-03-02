// using LaundryBooking.Models;
// using Laundry.Services;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Options;

// namespace LaundryBooking.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class BookingController(LaundryService laundryService) : ControllerBase
//     {
//         private readonly LaundryService _laundryService = laundryService;
//         private readonly IOptions<MongoDBSettings> mongoSettings = null!;

//         [HttpGet]
//         public async Task<List<Booking>> Get()
//         {
//             var bookings = await _laundryService.GetAll<Booking>(mongoSettings.Value.BookingsCollectionName);
//             return bookings;
//         }

//         [HttpGet("{id:length(24)}")]
//         public async Task<ActionResult<Booking>> Get(string id)
//         {
//             var booking = await _laundryService.GetById<Booking>(mongoSettings.Value.BookingsCollectionName, id);
//             if (booking is null)
//                 return NotFound();

//             return booking;
//         }
//         [HttpPost]
//         public async Task<IActionResult> Post(Booking newBooking)
//         {
//             await _laundryService.Create<Booking>(mongoSettings.Value.BookingsCollectionName, newBooking);
//             return CreatedAtAction(nameof(Get), new { id = newBooking._id }, newBooking);
//         }
//     }
// }