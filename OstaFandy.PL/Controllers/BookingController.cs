using System.Timers;
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

        [HttpGet("paged")]
        public ActionResult<PaginatedResult<BookingViewDto>> GetPagedBookings([FromQuery] string handymanName = "",[FromQuery] string status = "",[FromQuery] bool? isActive = null,[FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            var result = _autoBookingService.GetBookings(handymanName, status, isActive, pageNumber, pageSize);

            return Ok(result);
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
            if (freeslot == null)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "We’re sorry, but we don’t cover that area yet.",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            else if (!freeslot.Any())
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = " No times are available for this day. Kindly select a different day.",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            else
            {
                return Ok(new ResponseDto<List<AvailableTimeSlot>>
                {
                    IsSuccess = true,
                    Data = freeslot,
                    Message = "",
                    StatusCode = StatusCodes.Status200OK
                });
            }
        }

        [HttpPost("createbooking")]
        public async Task<IActionResult> CreateBooking(CreateBookingDTO dto)
        {
            var res = await _autoBookingService.CreateBooking(dto);
            if (res == null || res.BookingId <= 0)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Booking failed",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            return StatusCode(StatusCodes.Status201Created, new ResponseDto<object>
            {
                IsSuccess = true,
                Message = "Booking confirmed successfully",
                Data = new { bookingId = res.BookingId, chatId = res.ChatId },
                StatusCode = StatusCodes.Status201Created
            });
        }

        [HttpPatch("CancelBooking")]
        public IActionResult CancelBooking([FromQuery] int bookingId)
        {
            var res = _autoBookingService.CancelBooking(bookingId);
            if (res == 0)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "You cannot cancel a booking that has already been completed.",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            else if (res == -1)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "You must cancel the booking at least 24 hours in advance.",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            else if (res == -2)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Failed to cancel the booking. Please try again.",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            else
            {
                return Ok(new ResponseDto<string>
                {
                    IsSuccess = true,
                    Message = "Booking has been canceled successfully.",
                    Data = null,
                    StatusCode = StatusCodes.Status200OK
                });
            }
        }



    }
}
