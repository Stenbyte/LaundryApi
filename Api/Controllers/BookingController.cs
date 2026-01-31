using TenantApi.Models;
using TenantApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TenantApi.Exceptions;
using MongoDB.Bson;
using TenantApi.Validators;
using TenantApi.Enums;
using TenantApi.Controllers;


namespace TenantApi.Laundry.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController(IBookingService bookingService, IUserService userService) : AppControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IBookingService _bookingService = bookingService;



        [HttpGet("getAll")]
        public async Task<List<Booking>> GetAllBookingsByMachineId()
        // add request string for machine _id
        {
            var userId = User.FindFirstValue("sub");
            User? user = await _userService.FindUserById(userId!);
            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }

            var machines = await _bookingService.GetAllMachinesByBuildingId(user);
            if (machines == null || machines.Count == 0)
            {
                throw new CustomException("No machines found for the user's building", null, 404);
            }
            string machineId = machines[0]._id;

            if (!ObjectId.TryParse(machineId, out var _) || !ObjectId.TryParse(userId, out var _))
            {
                throw new CustomException("invalid Id");
            }

            MachineModel? machineExists = await _bookingService.GetMachine(user.dbName, machineId);

            if (machineExists == null || string.IsNullOrEmpty(machineExists._id))
            {
                throw new CustomException("Machine _id is null or empty", null, 400);
            }

            if (machineExists.status == MachineStatus.maintenance)
            {
                throw new CustomException($"Can not fetch data for machine with status: {machineExists.status}");
            }

            List<Booking> bookings = await _bookingService.GetAllBookingsByMachineId(user, machineExists._id);
            return bookings;
        }


        [HttpPost("create")]

        public async Task<IActionResult> CreateBooking([FromBody] BookingSlot request)
        {
            if (TimeSlotValidator.IsTimeSlotInThePast(request.timeSlots))
            {
                throw new CustomException("Cannot create a reservation for a past time", null, 400);
            }
            var userId = User.FindFirstValue("sub");

            User? user = await _userService.FindUserById(userId!);
            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }



            Booking? existingBooking = await _bookingService.GetBookingsByUserId(userId!, user.dbName);
            List<Booking> getAllBookings = await _bookingService.GetAllBookingsByBuildingId(user);
            Booking bookingToReturn;
            request._id = ObjectId.GenerateNewId().ToString();
            request.ConvertToUtc();
            request.booked = true;
            var machines = await _bookingService.GetAllMachinesByBuildingId(user);
            if (machines == null || machines.Count == 0)
            {
                throw new CustomException("No machines found for the user's building", null, 404);
            }
            string machineId = machines[0]._id;
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
                existingBooking.reservationsLeft--;
                bookingToReturn = existingBooking;
                await _bookingService.UpdateBooking(bookingToReturn, user.dbName);
            }
            else
            {
                var newBooking = new Booking {
                    userId = userId!,
                    machineId = machineId,
                    slots = new List<BookingSlot> { request },
                    reservationsLeft = 2,
                    buildingId = user.adress._id
                };
                bookingToReturn = newBooking;
                await _bookingService.CreateBooking(bookingToReturn, user.dbName);
            }


            return CreatedAtAction(nameof(CreateBooking), new {
                _id = bookingToReturn._id
            });

        }
        [HttpPost("createnew")]

        public async Task<IActionResult> CreateBookingNew([FromBody] Booking request)
        {
            if (!ObjectId.TryParse(request.machineId, out var _))
            {
                throw new CustomException("Machine _id is not valid", null, 400);
            }
            request.ConvertToUtc();


            if (request.startTime == null || request.endTime == null)
            {
                throw new CustomException("Start time or end time cannot be null", null, 400);
            }

            if (request.endTime.HasValue)
            {
                TimeSlotValidator.IsTimeSlotInThePast(request.endTime.Value);
            }

            TimeSlotValidator.IsBookingHoursWithinRange(request.startTime.Value, request.endTime.Value);


            var userId = User.FindFirstValue("sub");

            User? user = await _userService.FindUserById(userId!);
            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }

            // TODO fix here later machine search
            request.machineId = "687cb55ee117a61dcb72974f";
            List<Booking> getAllBookingsByMachineId = await _bookingService.GetAllBookingsByMachineId(user, request.machineId!);

            Booking bookingToReturn;
            request._id = ObjectId.GenerateNewId().ToString();

            request.booked = true;

            if (getAllBookingsByMachineId != null && getAllBookingsByMachineId.Count > 0)
            {
                if (getAllBookingsByMachineId.Count == 3)
                {
                    throw new CustomException("You can not add new reservation", null, 403);
                }

                bool foundExistingSlotMatch = false;

                foreach (var booking in getAllBookingsByMachineId)
                {
                    foundExistingSlotMatch = booking.startTime == request.startTime;


                    if (foundExistingSlotMatch)
                    {
                        throw new CustomException("Time slot is already taken", request.startTime, 403);
                    }
                }
            }


            var newBooking = new Booking {
                userId = userId!,
                machineId = request.machineId,
                startTime = request.startTime,
                endTime = request.endTime,
                reservationsLeft = getAllBookingsByMachineId != null ? getAllBookingsByMachineId.Count : 2,
                buildingId = user.adress._id
            };
            bookingToReturn = newBooking;
            await _bookingService.CreateBooking(bookingToReturn, user.dbName);


            return CreatedAtAction(nameof(CreateBooking), new {
                _id = bookingToReturn._id
            });

        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditBookingById([FromBody] EditBookingRequest request)
        {
            if (!ObjectId.TryParse(request._id, out _))
            {
                return BadRequest("Invalid booking ID");
            }
            var userId = User.FindFirstValue("sub");

            if (userId == null)
            {
                throw new CustomException("Check your userId", null, 403);
            }

            User? user = await _userService.FindUserById(userId!);

            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }


            var machines = await _bookingService.GetAllMachinesByBuildingId(user);
            if (machines == null || machines.Count == 0)
            {
                throw new CustomException("No machines found for the user's building", null, 404);
            }
            string machineId = machines[0]._id;
            var existingBooking = await _bookingService.GetAllBookingsByMachineId(user, machineId!);
            if (existingBooking == null)
            {
                return NotFound("Bookings is not found");
            }
            existingBooking[0].slots = existingBooking[0].slots.Where(slot => slot._id != request._id).ToList();
            // TODO enable when FE will be ready
            // existingBooking.startTime = null;
            // existingBooking.endTime = null;
            existingBooking[0].reservationsLeft++;

            await _bookingService.UpdateBooking(existingBooking[0], user.dbName);
            return CreatedAtAction(nameof(EditBookingById), new { existingBooking[0]._id });
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBookings()
        {
            var userId = User.FindFirstValue("sub");

            User? user = await _userService.FindUserById(userId!);

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

            // TODO enable this when front end is done ?
            // await _bookingService.CancelBooking(user._id!, user.dbName);
            await _bookingService.UpdateBooking(existingBooking, user.dbName);
            return CreatedAtAction(nameof(CancelBookings), new { existingBooking._id });
        }


        [HttpGet("getAllMachines")]
        public async Task<List<MachineModel>> GetAllMachinesByBuildingId()
        {
            var userId = User.FindFirstValue("sub");

            User? user = await _userService.FindUserById(userId!);
            if (user == null)
            {
                throw new CustomException("user is not found", null, 404);
            }

            if (!ObjectId.TryParse(userId, out var _))
            {
                throw new CustomException("invalid Id");
            }

            List<MachineModel> bookings = await _bookingService.GetAllMachinesByBuildingId(user);
            return bookings;
        }
    }
}