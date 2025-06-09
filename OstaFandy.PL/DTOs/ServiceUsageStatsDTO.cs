using OstaFandy.DAL.Entities;

namespace OstaFandy.PL.DTOs
{
    // service usage statistics
    public class ServiceUsageStatsDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string CategoryName { get; set; }
        public int UsageCount { get; set; }
    }
}
