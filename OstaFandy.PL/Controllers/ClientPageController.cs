using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.DAL.Repos;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientPageController : ControllerBase
    {
        private readonly IClientPageService _clientPageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClientPageController> _logger;

        public ClientPageController(IUnitOfWork unitOfWork, ILogger<ClientPageController> logger, IClientPageService clientPageService)
        {
            _clientPageService = clientPageService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        [HttpPut("ApproveJobStatusChange")]
        [EndpointDescription("ClientPage/ApproveJobStatusChange")]
        [EndpointSummary("Approve the job status change by the client. " +
            "This will update the job status and mark all notifications for that job as read.")]
        public IActionResult ApproveJobStatusChange(int jobId, string approvedStatus, int clientUserId)
        {
            if (approvedStatus == null)
            {
                return BadRequest(new { message = "Approved status cannot be null or empty." });
            }
            bool result = _clientPageService.ApproveJobStatusChange(jobId, approvedStatus, clientUserId);
            if (!result)
            {
                return BadRequest(new { message = $"Failed to update status for job {jobId}. Please ensure the transition is valid and the job exists." });
            }
            _logger.LogInformation($"Job {jobId} status successfully updated to {approvedStatus} by client approval.");
            return Ok(new { message = $"Job {jobId} status successfully updated to {approvedStatus} by client approval." });
        }

        [HttpPut("ApproveQuoteStatusChange")]
        [EndpointDescription("ClientPage/ApproveQuoteStatusChange")]
        [EndpointSummary("Approve the quote status change by the client. " +
            "This will update the job status and mark all notifications for that job as read.")]
        public IActionResult ApproveQuoteStatusChange(int jobId, string approvedStatus, int clientUserId)
        {
            if (approvedStatus == null)
            {
                return BadRequest(new { message = "Approved status cannot be null or empty." });
            }
            //_clientPageService.ApproveJobStatusChange(jobId, approvedStatus, clientUserId);
            bool result = _clientPageService.ApproveQuoteStatusChange(jobId, approvedStatus, clientUserId);
            if (result == null)
            {
                return BadRequest(new { message = $"Failed to update status for job {jobId}. Please ensure the transition is valid and the job exists." });
            }
            _logger.LogInformation($"Job {jobId} status successfully updated to {approvedStatus} by client approval.");
            return Ok(new { message = $"Job {jobId} status successfully updated to {approvedStatus} by client approval." });
        }

    }
}
