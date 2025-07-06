using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OstaFandy.DAL.Repos;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL;
using OstaFandy.PL.BL.IBL;
using Stripe;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientPageController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IClientPageService _clientPageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClientPageController> _logger;

        public ClientPageController(IUnitOfWork unitOfWork, ILogger<ClientPageController> logger, IClientPageService clientPageService, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _clientPageService = clientPageService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region approve status change
        [HttpPut("ApproveJobStatusChange")]
        [EndpointDescription("ClientPage/ApproveJobStatusChange")]
        [EndpointSummary("Approve the job status change by the client. " +
    "This will update the job status and mark all notifications for that job as read.")]
        public async Task<IActionResult> ApproveJobStatusChange([FromQuery] int jobId, [FromQuery] string approvedStatus, [FromQuery] int clientUserId)
        {
            _logger.LogInformation($"Received: jobId={jobId}, approvedStatus={approvedStatus}, clientUserId={clientUserId}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(approvedStatus))
            {
                _logger.LogWarning("Approved status is null or empty");
                return BadRequest(new { message = "Approved status cannot be null or empty." });
            }

            try
            {
                bool result = _clientPageService.ApproveJobStatusChange(jobId, approvedStatus, clientUserId);
                if (!result)
                {
                    return BadRequest(new { message = $"Failed to update status for job {jobId}. Please ensure the transition is valid and the job exists." });
                }

                _logger.LogInformation($"Job {jobId} status successfully updated to {approvedStatus} by client approval.");

                var handyman = _unitOfWork.HandyManRepo.GetHandymanByJobId(jobId);
                _logger.LogInformation($"Found handyman: {handyman?.UserId} for job {jobId}");

                if (handyman?.UserId != null)
                {
                    var message = $"Quote status for job {jobId} has been updated to {approvedStatus} by client.";
                    await _notificationService.SendNotificationToHandyman(
                        handyman.UserId.ToString(),
                        message
                    );
                    _logger.LogInformation($"Notification sent to handyman {handyman.UserId} for job {jobId}");
                }
                else
                {
                    _logger.LogWarning($"No handyman found for job {jobId}");
                }

                return Ok(new { message = $"Job {jobId} status successfully updated to {approvedStatus} by client approval." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving job status change for job {jobId}");
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }


        #endregion

        #region Mark as a read
        [HttpPut("/api/new/MarkAsRead/{notificationId}")]
        [EndpointDescription("ClientPage/MarkAsRead")]
        [EndpointSummary("Mark a specific notification as read.")]
        public IActionResult MarkNotificationAsRead(int notificationId)
        {
            var notification = _unitOfWork.NotificationRepo.GetById(notificationId);
            if (notification == null)
            {
                return NotFound(new { message = $"Notification with ID {notificationId} not found." });
            }
            notification.IsRead = true;
            _unitOfWork.NotificationRepo.Update(notification);
            _unitOfWork.Save();

            var handyman = _unitOfWork.HandyManRepo.GetHandymanByNotificationId(notificationId);
            var jobId = _unitOfWork.JobAssignmentRepo.GetJobIdByNotificationId(notificationId);
            _logger.LogInformation($"Found handyman: {handyman?.UserId} for job {jobId}");

            if (handyman?.UserId != null)
            {
                var message = $"Quote status for job {jobId} has been updated to Reject by client.";
                  _notificationService.SendNotificationToHandyman(
                    handyman.UserId.ToString(),
                    message
                );
                _logger.LogInformation($"Notification sent to handyman {handyman.UserId} for job {jobId}");
            }
            return Ok(new { message = "Notification marked as read." });
        }
        #endregion

        #region get notifications
        [HttpGet("GetNotifications/{userId}")]
        [EndpointDescription("ClientPage/GetNotifications")]
        [EndpointSummary("Get all notifications for a specific user.")]
        public IActionResult GetNotifications(int userId)
        {
            var notifications = _unitOfWork.NotificationRepo.GetAll(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt);
            var notificationDtos = notifications.Select(n => new
            {
                n.Id,
                n.UserId,
                n.Type,
                n.Title,
                n.Message,
                n.CreatedAt,
                n.IsRead,
                CurrentJobStatus = n.RelatedEntityType == "JobAssignment" && n.RelatedEntityId.HasValue
                    ? _unitOfWork.JobAssignmentRepo.GetByIdOrDefault(n.RelatedEntityId.Value)?.Status
                    : null,
                RelatedEntityId = n.RelatedEntityId,

            }).ToList();
            return Ok(notificationDtos);
        }

        #endregion

        #region approve quote change status
        [HttpPut("new/ApproveQuoteStatusChange")]
        [EndpointDescription("ClientPage/ApproveQuoteStatusChange")]
        [EndpointSummary("Approve the quote status change by the client. " +
            "This will update the job status and mark all notifications for that job as read.")]
        public IActionResult ApproveQuoteStatusChange([FromQuery] int jobId, [FromQuery] string approvedStatus, [FromQuery] int clientUserId)
        {
            _logger.LogInformation($"Received: jobId={jobId}, approvedStatus={approvedStatus}, clientUserId={clientUserId}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(approvedStatus))
            {
                _logger.LogWarning("Approved status is null or empty");
                return BadRequest(new { message = "Approved status cannot be null or empty." });
            }

            try
            {
                bool result = _clientPageService.ApproveQuoteStatusChange(jobId, approvedStatus, clientUserId);

                if (!result)
                {
                    return BadRequest(new { message = $"Failed to update status for job {jobId}. Please ensure the transition is valid and the job exists." });
                }
                var handymanId = _unitOfWork.JobAssignmentRepo.gethandymanbyjobid(jobId);
                var job = _unitOfWork.JobAssignmentRepo.GetById(jobId);
                if (job?.HandymanId != null)
                {
                    var handyman = _unitOfWork.HandyManRepo.GetById(job.HandymanId);
                    if (handyman?.UserId != null)
                    {
                          _notificationService.SendNotificationToHandyman(
                            handyman.UserId.ToString(),
                            $"Quote status for job {jobId} has been updated to {approvedStatus} by client."
                        );
                    }
                }
                _logger.LogInformation($"Job {jobId} status successfully updated to {approvedStatus} by client approval.");
                return Ok(new { message = $"Job {jobId} status successfully updated to {approvedStatus} by client approval." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while approving quote status change.");
                return StatusCode(500, new { message = "An error occurred while processing the request.", error = ex.Message });
            }
        }
        #endregion

        #region get available time slot for a handyman
        [HttpGet("GetAvailableTimeSlotForHandyman")]
        [EndpointDescription("ClientPage/GetAvailableTimeSlotForHandyman")]
        [EndpointSummary("Get available time slots for a handyman on a specific day.")]
        public async Task<IActionResult> GetAvailableTimeSlotForHandymanAsync(int handymanId, DateTime day, int estimatedMinutes)
        {
            if (handymanId <= 0 || estimatedMinutes <= 0)
            {
                return BadRequest(new { message = "Invalid handyman ID or estimated minutes." });
            }
            var availableSlots = await _clientPageService.GetAvailableTimeSlotForHandymanAsync(handymanId, day, estimatedMinutes);
            if (availableSlots == null || !availableSlots.Any())
            {
                return NotFound(new { message = "No available time slots found for the specified handyman on this day." });
            }
            return Ok(availableSlots);
        }
        #endregion

    }
}