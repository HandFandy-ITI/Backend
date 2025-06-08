using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IAutoBookingService
    {
        public List<BookingViewDto> GetAllBookings();
    }
}
