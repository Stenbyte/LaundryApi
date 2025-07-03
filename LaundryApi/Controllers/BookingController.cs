using LaundryApi.Models;
using LaundryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using LaundryBooking.Services;
using System.Security.Claims;
using LaundryApi.Exceptions;
using MongoDB.Bson;
using LaundryApi.Validators;

namespace LaundryBooking.Controllers
{
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

            User? user = await _laundryService.FindUserById(userId!);
            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }

            var _bookingService = new BookingService(_mongoSettings, user.dbName);
            List<Booking> bookings = await _bookingService.GetAll();
            return bookings;
        }


        [HttpPost("create")]

        public async Task<IActionResult> CreateBooking([FromBody] BookingSlot request)
        {

            if (TimeSlotValidator.IsTimeSlotInThePast(request.timeSlots))
            {
                throw new CustomException("Cannot create a reservation for a past time", null, 400);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User? user = await _laundryService.FindUserById(userId!);
            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }

            var _bookingService = new BookingService(_mongoSettings, user.dbName);

            Booking? existingBooking = await _bookingService.GetBookingsById(userId!);
            List<Booking> getAllBookings = await _bookingService.GetAll();
            Booking bookingToReturn;
            request.id = ObjectId.GenerateNewId().ToString();
            request.ConvertToUtc();
            request.booked = true;
            if (existingBooking != null)
            {
                if (existingBooking?.reservationsLeft == 0)
                {
                    throw new CustomException("You can not add new reservation", null, 403);
                }

                BookingSlot? foundExistingSlotMatch;

                foreach (var booking in getAllBookings)
                {
                    foundExistingSlotMatch = booking.slots.FirstOrDefault(slot => {
                        if (slot.day.Date == request.day.Date)
                        {
                            return slot.timeSlots.Any(slot => slot == request.timeSlots[0]);
                        }
                        return false;

                    });

                    if (foundExistingSlotMatch != null)
                    {
                        throw new CustomException("Time slot is already taken", request.timeSlots[0], 403);
                    }
                }

                existingBooking!.slots.Add(request);
                existingBooking!.reservationsLeft--;
                bookingToReturn = existingBooking;
                await _bookingService.UpdateBooking(bookingToReturn);
            }
            else
            {
                var newBooking = new Booking {
                    userId = userId!,
                    slots = new List<BookingSlot> { request },
                    reservationsLeft = 2
                };
                bookingToReturn = newBooking;
                await _bookingService.CreateBooking(bookingToReturn);
            }


            return CreatedAtAction(nameof(CreateBooking), new {
                id = bookingToReturn.id
            });

        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditBookingById([FromBody] EditBookingRequest request)
        {
            if (!ObjectId.TryParse(request.id, out _))
            {
                return BadRequest("Invalid booking ID");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                throw new CustomException("Check your userId", null, 403);
            }

            User? user = await _laundryService.FindUserById(userId!);

            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }

            var _bookingService = new BookingService(_mongoSettings, user.dbName);

            var existingBooking = await _bookingService.FindByUserAndSlotId(request.id, userId);
            if (existingBooking == null)
            {
                return NotFound("Bookings is not found");
            }
            existingBooking.slots = existingBooking.slots.Where(slot => slot.id != request.id).ToList();
            existingBooking.reservationsLeft++;

            await _bookingService.UpdateBooking(existingBooking);
            return CreatedAtAction(nameof(EditBookingById), new { existingBooking.id });
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User? user = await _laundryService.FindUserById(userId!);

            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }

            var _bookingService = new BookingService(_mongoSettings, user.dbName);

            var existingBooking = await _bookingService.FindBookingsByUserId(userId!);
            if (existingBooking == null)
            {
                return NotFound("Bookings is not found");
            }
            existingBooking.slots = new List<BookingSlot>();
            existingBooking.reservationsLeft = 3;

            await _bookingService.UpdateBooking(existingBooking);
            return CreatedAtAction(nameof(CancelBookings), new { existingBooking.id });
        }
    }
}