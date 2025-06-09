using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpGet("service-usage-stats")]
        [EndpointDescription("AdminAnalytics/GetServiceUsageStats")]
        [EndpointSummary("returns usage count for each service")]
        public IActionResult GetServiceUsageStats()
        {
            try
            {
                var result = _analyticsService.GetServiceUsageStats();

                if (result == null || !result.Any())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "No service usage data found",
                        data = new List<ServiceUsageStatsDTO>(),
                        count = 0
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Found usage statistics for {result.Count} services",
                    data = result,
                    count = result.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service usage stats");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error occurred while retrieving service usage statistics"
                });
            }
        }

        [HttpGet("booking-location-stats")]
        [EndpointDescription("AdminAnalytics/GetBookingLocationStats")]
        [EndpointSummary("returns booking counts by cities and addresses")]
        public IActionResult GetBookingLocationStats()
        {
            try
            {
                var result = _analyticsService.GetBookingLocationStats();

                if (result == null || !result.Any())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "No booking location data found",
                        data = new List<BookingLocationStatsDTO>(),
                        count = 0
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Found booking statistics for {result.Count} cities",
                    data = result,
                    count = result.Count,
                    totalBookings = result.Sum(x => x.BookingCount)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting booking location stats");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error occurred while retrieving booking location statistics"
                });
            }
        }
    }
}