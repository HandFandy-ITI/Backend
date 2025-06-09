using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var bookings =_autoBookingService.GetAllBookings();
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
    }
}
