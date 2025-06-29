using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class HandymanJobsController : ControllerBase
    {
        private readonly IHandymanJobsService _handymanJobService;
        private readonly ILogger<HandymanJobsController> _logger;

        public HandymanJobsController(IHandymanJobsService handymanJobsService, ILogger<HandymanJobsController> logger)
        {
            _handymanJobService = handymanJobsService;
            _logger = logger;
        }
        [HttpGet]
        [EndpointDescription("HandymanJobs/GetAllJobs")]
        [EndpointSummary("return all jobs for specific handyman. you MUST add handyman Id")]
        public IActionResult GetAllJobs([FromQuery] string searchString = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? status = null, [FromQuery] int handymanId = -1)
        {
            try
            {
                var result = _handymanJobService.GetAll(searchString, pageNumber, pageSize, status, handymanId);
                if (result.Data == null || !result.Data.Any())
                {
                    return Ok(new
                    {
                        message = "There are no handyman jobs found.",
                        data = new List<HandymanJobsDTO>(),
                        currentPage = result.CurrentPage,
                        totalPages = result.TotalPages,
                        totalCount = result.TotalCount,
                        searchString = result.SearchString
                    });
                }
                return Ok(new
                {
                    data = result.Data,
                    currentPage = result.CurrentPage,
                    totalPages = result.TotalPages,
                    totalCount = result.TotalCount,
                    searchString = result.SearchString
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching handyman jobs.");
                return StatusCode(500, new { message = "An error occurred while fetching handyman jobs.", error = ex.Message });
            }
        }

        [HttpPut("{jobId}")]
        [EndpointDescription("HandymanJobs/UpdateJobStatus")]
        [EndpointSummary("this will NOT update this will add notification on the client table to show for it" +
            "if the accept it should update the status if not it should not")]
        public IActionResult SentNotificationToClientToUpdataStatus(int jobId, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Status cannot be null or empty." });
            }
            try
            {
                var result = _handymanJobService.SentNotificationToClientToUpdataStatus(jobId, status);
                if (result)
                {
                    return Ok(new { message = "Job send to get client approve successfully" });
                }
                else
                {
                    return NotFound(new { message = $"Job with ID {jobId} not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating job status.");
                return StatusCode(500, new { message = "An error occurred while updating job status.", error = ex.Message });
            }
        }

        [HttpPost("quote")]
        [EndpointDescription("HandymanJobs/AddQuote")]
        [EndpointSummary("Add a quote for a specific job. You must provide the jobId, price, and notes in the request body.")]
        public IActionResult AddQuote([FromBody] AddQuoteDTO model)
        {
            if (model.Price <= 0 || string.IsNullOrEmpty(model.Notes))
            {
                return BadRequest(new { message = "Invalid quote data." });
            }
            try
            {
                var result = _handymanJobService.AddQuote(model.JobId, model.Price, model.Notes);
                if (result)
                {
                    return Ok(new { message = "Quote added successfully." });
                }
                else
                {
                    return NotFound(new { message = $"Job with ID {model.JobId} not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a quote.");
                return StatusCode(500, new { message = "An error occurred while adding a quote.", error = ex.Message });
            }
        }
    }
}
