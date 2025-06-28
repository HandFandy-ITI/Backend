using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.DAL.Entities;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IAutoBookingService _autoBookingService;

        public BookingController(IAutoBookingService autoBookingService)
        {
            _autoBookingService = autoBookingService;
        }

        [HttpGet("GetAllBookings")]
        public IActionResult GetAllBookings()
        {
            var bookings = _autoBookingService.GetAllBookings();
            if (bookings == null)
            {
                return NotFound(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "No bookings found",
                    Data = null,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            return Ok(bookings);
        }

        [HttpGet("GetBookingById/{id}")]
        public IActionResult GetBookingById(int id)
        {

            var booking = _autoBookingService.GetBookingById(id);

            return Ok(new ResponseDto<BookingViewDto>
            {
                IsSuccess = true,
                Message = "Booking retrieved successfully.",
                Data = booking,
                StatusCode = StatusCodes.Status200OK
            });
        }

        [HttpGet("GetBookingsByClientId/{clientId}")]
        public IActionResult GetBookingsByClientId(int clientId)
        {
            var bookings = _autoBookingService.GetBookingsByClientId(clientId);

            return Ok(new ResponseDto<List<BookingViewDto>>
            {
                IsSuccess = true,
                Message = "Bookings retrieved successfully.",
                Data = bookings,
                StatusCode = StatusCodes.Status200OK
            });


        }

        [HttpGet("GetBookingsByHandyManId/{handyManId}")]
        public IActionResult GetBookingsByHandyManId(int handyManId)
        {
            var bookings = _autoBookingService.GetBookingsByHandyManId(handyManId);
            return Ok(new ResponseDto<List<BookingViewDto>>
            {
                IsSuccess = true,
                Message = "Bookings retrieved successfully.",
                Data = bookings,
                StatusCode = StatusCodes.Status200OK
            });


        }

        [HttpGet("FreeSlot")]
        public async Task<IActionResult> GetFreeSlot([FromQuery] int categoryId,[FromQuery] DateTime day,[FromQuery] decimal userLatitude,[FromQuery] decimal userLongitude,[FromQuery] int estimatedMinutes)
        {
            var reqdata = new AvailableTimeSlotsRequestDto
            {
                CategoryId = categoryId,
                Day = day,
                UserLatitude = userLatitude,
                UserLongitude = userLongitude,
                EstimatedMinutes = estimatedMinutes
            };
           
            var freeslot = await _autoBookingService.GetAvailableTimeSlotAsync(reqdata);
            if (freeslot == null || !freeslot.Any())
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "No Avliable time please choose another day",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            else
            {
                return Ok(new ResponseDto<List<AvailableTimeSlot>>
                {
                    IsSuccess=true,
                    Data=freeslot,
                    Message="",
                    StatusCode= StatusCodes.Status200OK
                });
            }
        }
    }
}
