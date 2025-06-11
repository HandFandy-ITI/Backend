using Microsoft.Extensions.Primitives;

namespace OstaFandy.PL.DTOs
{
    public class DashboardDTO
    {
        public string Service { get; set; }
        public string location { get; set; }
        public string client { get; set; }
        public string handyman { get; set; }
        public string review { get; set; }
        public decimal Revenue { get; set; }
    }
}
