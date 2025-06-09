using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IAnalyticsService
    {
        List<ServiceUsageStatsDTO> GetServiceUsageStats();
        List<BookingLocationStatsDTO> GetBookingLocationStats();
    }
}
