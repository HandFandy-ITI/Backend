using OstaFandy.DAL.Entities;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IAutoBookingService
    {
         List<BookingViewDto> GetAllBookings();
         BookingViewDto GetBookingById(int id);

        List<BookingViewDto> GetBookingsByClientId(int clientId);

        List<BookingViewDto> GetBookingsByHandyManId(int HandyManId);

        Task<List<AvailableTimeSlot>> GetAvailableTimeSlotAsync(AvailableTimeSlotsRequestDto reqdata);

        //Task<int> CreateBooking(CreateBookingDTO bookingdto);
        Task<BookingChatResponse?> CreateBooking(CreateBookingDTO bookingdto);

    }
}
