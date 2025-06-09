using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(IUnitOfWork unitOfWork, ILogger<AnalyticsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public List<ServiceUsageStatsDTO> GetServiceUsageStats()
        {
            try
            {
                var data = _unitOfWork.AnalyticsRepo.GetServiceUsageStats().ToList();

                var result = data.Select(item =>
                {
                    var props = item.GetType().GetProperties();
                    return new ServiceUsageStatsDTO
                    {
                        ServiceId = (int)props.First(p => p.Name == "ServiceId").GetValue(item),
                        ServiceName = (string)props.First(p => p.Name == "ServiceName").GetValue(item),
                        CategoryName = (string)props.First(p => p.Name == "CategoryName").GetValue(item),
                        UsageCount = (int)props.First(p => p.Name == "UsageCount").GetValue(item)
                    };
                }).ToList();

                _logger.LogInformation($"Retrieved service usage stats for {result.Count} services");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service usage stats");
                return new List<ServiceUsageStatsDTO>();
            }
        }

        public List<BookingLocationStatsDTO> GetBookingLocationStats()
        {
            try
            {
                var data = _unitOfWork.AnalyticsRepo.GetBookingLocationStats().ToList();

                var result = data.Select(item =>
                {
                    var props = item.GetType().GetProperties();
                    var addresses = (IEnumerable<object>)props.First(p => p.Name == "Addresses").GetValue(item);

                    return new BookingLocationStatsDTO
                    {
                        City = (string)props.First(p => p.Name == "City").GetValue(item),
                        BookingCount = (int)props.First(p => p.Name == "BookingCount").GetValue(item),
                        Addresses = addresses.Select(addr =>
                        {
                            var addrProps = addr.GetType().GetProperties();
                            return new AddressBookingStatsDTO
                            {
                                Address = (string)addrProps.First(p => p.Name == "Address").GetValue(addr),
                                BookingCount = (int)addrProps.First(p => p.Name == "BookingCount").GetValue(addr)
                            };
                        }).ToList()
                    };
                }).ToList();

                _logger.LogInformation($"Retrieved booking location stats for {result.Count} cities");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting booking location stats");
                return new List<BookingLocationStatsDTO>();
            }
        }
    }
}
