using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.Constants;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(IDashboardService dashboardService, ILogger<AdminDashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;

        }
        #region Statistics
        [HttpGet]
        [EndpointDescription("AdminDashboard/GetAllCompletedJobCount")]
        [EndpointSummary("Get the count of all completed jobs")]
        public IActionResult GetAllCompletedJobCount()
        {
            try
            {
                var completedJobCount = _dashboardService.GetAllCompletedJobCount();
                return Ok(new { count = completedJobCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching completed job count", error = ex.Message });
            }
        }
        [HttpGet("totalrevenue")]
        [EndpointDescription("AdminDashboard/GetAllRevenue")]
        [EndpointSummary("Get the total revenue of all completed jobs")]
        public IActionResult GetAllRevenue()
        {
            try
            {
                var totalPrice = _dashboardService.GetTotalPrice();
                return Ok(new { totalRevenue = totalPrice });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching total price", error = ex.Message });
            }
        }
        [HttpGet("averageRating")]
        [EndpointDescription("AdminDashboard/GetAverageRating")]
        [EndpointSummary("Get the average rating of all handymen")]
        public IActionResult GetAverageRating()
        {
            try
            {
                var averageRating = _dashboardService.GetAverageRating();
                return Ok(new { averageRating = averageRating });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching average rating", error = ex.Message });
            }
        }
        [HttpGet("GetCountOfActiveClient")]
        [EndpointDescription("AdminDashboard/GetCountOfActiveClient")]
        [EndpointSummary("Get the count of active clients")]
        public IActionResult GetCountOfActiveClient()
        {
            try
            {
                var activeClientCount = _dashboardService.GetCountOfActiveClient();
                return Ok(new { count = activeClientCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching count of active clients", error = ex.Message });
            }
        }
        #endregion

        [HttpGet("dashboard-data")]
        [EndpointDescription("AdminDashboard/GetDashboardData")]
        [EndpointSummary("Get paginated dashboard data with search and filters")]
        public IActionResult GetDashboardData(
        [FromQuery] string searchString = "",
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] bool? isActive = null,
        [FromQuery] List<string> filters = null)
        {
            try
            {
                // Convert string filters to enum
                var dashboardFilters = new List<DashboardFilter>();
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        if (Enum.TryParse<DashboardFilter>(filter, true, out var parsedFilter))
                        {
                            dashboardFilters.Add(parsedFilter);
                        }
                    }
                }

                var result = _dashboardService.GetAll(searchString, pageNumber, pageSize, isActive, dashboardFilters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching dashboard data.");
                return StatusCode(500, new { message = "An error occurred while fetching dashboard data", error = ex.Message });
            }
        }
    }
}
