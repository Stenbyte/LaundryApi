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
    public class BookingController(ILaundryService laundryService, IBookingService bookingService) : ControllerBase
    {
        private readonly ILaundryService _laundryService = laundryService;
        private readonly IBookingService _bookingService = bookingService;



        [HttpGet("getAll")]
        public async Task<List<Booking>> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            User? user = await _laundryService.FindUserById(userId!);
            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }

            List<Booking> bookings = await _bookingService.GetAll(user);
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



            Booking? existingBooking = await _bookingService.GetBookingsById(userId!, user.dbName);
            List<Booking> getAllBookings = await _bookingService.GetAll(user);
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
                await _bookingService.UpdateBooking(bookingToReturn, user.dbName);
            }
            else
            {
                var newBooking = new Booking {
                    userId = userId!,
                    slots = new List<BookingSlot> { request },
                    reservationsLeft = 2,
                    buildingId = user.adress.id
                };
                bookingToReturn = newBooking;
                await _bookingService.CreateBooking(bookingToReturn, user.dbName);
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


            var existingBooking = await _bookingService.FindByUserAndSlotId(request.id, userId, user.dbName);
            if (existingBooking == null)
            {
                return NotFound("Bookings is not found");
            }
            existingBooking.slots = existingBooking.slots.Where(slot => slot.id != request.id).ToList();
            existingBooking.reservationsLeft++;

            await _bookingService.UpdateBooking(existingBooking, user.dbName);
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


            var existingBooking = await _bookingService.FindBookingsByUserId(userId!, user.dbName);
            if (existingBooking == null)
            {
                return NotFound("Bookings is not found");
            }
            existingBooking.slots = new List<BookingSlot>();
            existingBooking.reservationsLeft = 3;

            await _bookingService.UpdateBooking(existingBooking, user.dbName);
            return CreatedAtAction(nameof(CancelBookings), new { existingBooking.id });
        }
    }
}