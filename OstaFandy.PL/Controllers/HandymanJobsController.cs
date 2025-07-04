using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;
using Stripe;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandymanJobsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHandymanJobsService _handymanJobService;
        private readonly ILogger<HandymanJobsController> _logger;

        public HandymanJobsController(INotificationService notificationService, IHandymanJobsService handymanJobsService, ILogger<HandymanJobsController> logger, IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _handymanJobService = handymanJobsService;
            _logger = logger;
        }

       


        #region get handyman notification

        [HttpGet("GetNotificationsOfHandyman/{handymanUserId}")]
        [EndpointDescription("HandymanJobs/GetNotificationsOfHandyman")]
        [EndpointSummary("get all notification for the handyman")]
        public IActionResult GetNotifications(int handymanUserId)
        {
            try
            {
                var notifications =   _unitOfWork.NotificationRepo
                    .GetAll(n => n.UserId == handymanUserId);

                var orderedNotifications = notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(50)  
                    .ToList();

                return Ok(orderedNotifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving notifications", error = ex.Message });
            }
        }
        #endregion

        #region send notification with status to update it
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

                    var job = _unitOfWork.JobAssignmentRepo.GetById(jobId);
                    if (job?.Booking?.ClientId != null)
                    {
                        var client = _unitOfWork.ClientRepo.GetById(job.Booking.ClientId);
                        if (client?.UserId != null)
                        {
                              _notificationService.SendNotificationToClient(
                                client.UserId.ToString(),
                                jobId,
                                status
                            );
                        }
                    }
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
        #endregion

        #region add quote
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
                var result = _handymanJobService.AddQuote(model.JobId, model.Price, model.Notes, model.EstimatedMinutes);
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
        #endregion

        #region process quote response, this when the client make accept or reject
        [HttpPost("quote-response")]
        [EndpointDescription("ClientPage/ProcessQuoteResponse")]
        [EndpointSummary("Process client response to a quote (accept/reject)")]
        public async Task<IActionResult> ProcessQuoteResponseAsync([FromBody] QuoteResponseDTO model)
        {
            try
            {
                // Validate the quote belongs to the user
                var quoteDetails = _handymanJobService.GetQuoteDetails(model.QuoteId);
                if (quoteDetails == null)
                {
                    return NotFound(new { message = "Quote not found." });
                }

                // Verify the client owns this quote
                //var userClient = _handymanJobService.GetById(model.ClientUserId);
                //if (userClient == null || userClient.UserId != model.ClientUserId)
                //{
                //    return Unauthorized(new { message = "Unauthorized access to this quote." });
                //}

                if (model.Action.ToLower() == "reject")
                {
                    var result = await _handymanJobService.ProcessQuoteResponseAsync(model.QuoteId, model.Action, model.ClientUserId);
                    if (result)
                    {
                        var handyman = _unitOfWork.HandyManRepo.GetById(quoteDetails.HandymanId);
                        if (handyman?.UserId != null)
                        {
                            await _notificationService.SendQuoteResponse(
                             handyman.UserId.ToString(),
                             model.QuoteId,
                             model.Action
                         );
                        }
                        return Ok(new { message = "Quote rejected successfully." });
                    }
                    return BadRequest(new { message = "Failed to reject quote." });
                }
                else if (model.Action.ToLower() == "accept")
                {
                    if (model.BookingData == null)
                    {
                        return BadRequest(new { message = "Booking data is required for quote acceptance." });
                    }

                    // Validate booking data
                    if (model.BookingData.PreferredDate <= DateTime.UtcNow)
                    {
                        return BadRequest(new { message = "Preferred date must be in the future." });
                    }

                    // Ensure the booking data matches the quote
                    model.BookingData.ClientId = quoteDetails.ClientId;
                    model.BookingData.HandymanId = quoteDetails.HandymanId;
                    model.BookingData.TotalPrice = quoteDetails.Price;
                    model.BookingData.EstimatedMinutes = quoteDetails.EstimatedMinutes;

                    // Use original services if not provided
                    if (model.BookingData.serviceDto == null || !model.BookingData.serviceDto.Any())
                    {
                        model.BookingData.serviceDto = quoteDetails.OriginalServices;
                    }

                    var result = await _handymanJobService.ProcessQuoteResponseAsync(model.QuoteId, model.Action, model.ClientUserId, model.BookingData);
                    if (result)
                    {
                        var handyman = _unitOfWork.HandyManRepo.GetById(quoteDetails.HandymanId);
                        if (handyman?.UserId != null)
                        {
                            await _notificationService.SendNotificationToHandyman(
                                handyman.UserId.ToString(),
                                $"Quote with ID {model.QuoteId} has been accepted by the client and booking has been created."
                            );
                        }
                        return Ok(new { message = "Quote accepted and booking created successfully." });
                    }
                    return BadRequest(new { message = "Failed to accept quote and create booking." });
                }

                return BadRequest(new { message = "Invalid action. Use 'accept' or 'reject'." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing quote response.");
                return StatusCode(500, new { message = "An error occurred while processing quote response.", error = ex.Message });
            }
        }
        #endregion

        #region get quote details, stop implement for a while DO NOT DELETE IT
        [HttpGet("quote-details/{quoteId}")]
        [EndpointDescription("ClientPage/GetQuoteDetails")]
        [EndpointSummary("Get quote details for notification")]
        public IActionResult GetQuoteDetails(int quoteId)
        {
            try
            {
                var quoteDetails = _handymanJobService.GetQuoteDetails(quoteId);
                if (quoteDetails == null)
                {
                    return NotFound(new { message = "Quote not found." });
                }

                return Ok(quoteDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting quote details.");
                return StatusCode(500, new { message = "An error occurred while getting quote details.", error = ex.Message });
            }
        }
        #endregion


        #region get the handyan quote
        [HttpGet("quotes/{handymanId}")]
        [EndpointDescription("HandymanJobs/GetHandymanQuotes")]
        [EndpointSummary("Get all quotes for a specific handyman by their ID.")]
        public IActionResult GetHandymanQuotes(int handymanId, int pageNumber = 1, int pageSize = 5, string searchString="")
        {
            try
            {
                if (handymanId <= 0)
                {
                    return BadRequest("Handyman ID must be a positive integer.");
                }

                if (pageNumber <= 0)
                {
                    return BadRequest("Page number must be greater than 0.");
                }

                if (pageSize <= 0 || pageSize > 100)
                {
                    return BadRequest("Page size must be between 1 and 100.");
                }

                var result = _handymanJobService.GetHandymanQuotes(handymanId, pageNumber, pageSize, searchString ?? string.Empty);

                if (result.Data == null || !result.Data.Any())
                {
                    return Ok(new
                    {
                        message = "There are no handyman jobs found.",
                        data = new List<AllQuotes>(),
                        currentPage = result.CurrentPage,
                        totalPages = result.TotalPages,
                        totalCount = result.TotalCount,
                        searchString = result.SearchString
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching quotes for handyman {HandymanId}", handymanId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        #endregion

        #region Handyman availability stop implement for a while DOT NOT DELETE IT
        //[HttpGet("check-availability/{handymanId}")]
        //[EndpointDescription("ClientPage/CheckHandymanAvailability")]
        //[EndpointSummary("Check if handyman is available for the selected date")]
        //public IActionResult CheckHandymanAvailability(int handymanId, [FromQuery] DateTime preferredDate)
        //{
        //    try
        //    {
        //        // Check if handyman has blocked dates for this time
        //        var blockedDates = _unitOfWork.BlockDateRepo
        //            .GetAll()
        //            .Where(bd => bd.UserId == handymanId &&
        //                        bd.IsActive &&
        //                        bd.StartDate <= preferredDate &&
        //                        bd.EndDate >= preferredDate)
        //            .Any();

        //        if (blockedDates)
        //        {
        //            return Ok(new { isAvailable = false, message = "Handyman is not available on this date." });
        //        }

        //        // Check for existing bookings at the same time (optional - depends on your business logic)
        //        var existingBookings = _unitOfWork.JobAssignmentRepo
        //            .GetAll()
        //            .Where(ja => ja.HandymanId == handymanId &&
        //                        ja.IsActive &&
        //                        ja.Status != "Cancelled")
        //            .Join(_unitOfWork.BookingRepo.GetAll(),
        //                  ja => ja.BookingId,
        //                  b => b.Id,
        //                  (ja, b) => new { JobAssignment = ja, Booking = b })
        //            .Where(x => x.Booking.PreferredDate.Date == preferredDate.Date)
        //            .Any();

        //        if (existingBookings)
        //        {
        //            return Ok(new { isAvailable = false, message = "Handyman has another booking on this date." });
        //        }

        //        return Ok(new { isAvailable = true, message = "Handyman is available." });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while checking handyman availability.");
        //        return StatusCode(500, new { message = "An error occurred while checking availability.", error = ex.Message });
        //    }
        //}
        #endregion

        #region Get quote detials by quote id
        //[HttpGet("quote/{quoteId}")]
        //[EndpointDescription("HandymanJobs/GetQuoteById")]
        //[EndpointSummary("Get quote details by quote ID including job information")]
        //public IActionResult GetQuoteById(int quoteId)
        //{
        //    try
        //    {
        //        if (quoteId <= 0)
        //        {
        //            return BadRequest("Quote ID must be a positive integer.");
        //        }

        //        var quote = _unitOfWork.QuoteRepo.GetById(quoteId);
        //        if (quote == null)
        //        {
        //            return NotFound(new { message = $"Quote with ID {quoteId} not found." });
        //        }

        //        // Get related job assignment to fetch handyman and other details
        //        var jobAssignment = _unitOfWork.JobAssignmentRepo.GetById(quote.JobAssignmentId);
        //        if (jobAssignment == null)
        //        {
        //            return NotFound(new { message = "Related job assignment not found." });
        //        }

        //        // Get handyman details
        //        var handyman = _unitOfWork.HandyManRepo.GetById(jobAssignment.HandymanId);
        //        var handymanUser = _unitOfWork.UserRepo.GetById(handyman?.UserId ?? 0);

        //        // Get client details
        //        var client = _unitOfWork.ClientRepo.GetById(jobAssignment.Booking.ClientId);

        //        // Get booking if exists
        //        var booking = jobAssignment.BookingId > 0 ?
        //            _unitOfWork.BookingRepo.GetById(jobAssignment.BookingId) : null;

        //        // Get services from the original booking or job
        //        var services = booking != null ?
        //            _unitOfWork.BookingServiceRepo.GetAll()
        //                .Where(bs => bs.BookingId == booking.Id)
        //                .Select(bs => new { serviceId = bs.ServiceId, quantity = bs.Quantity })
        //                .Cast<object>()
        //                .ToList() :
        //            new List<object>();

        //        var result = new
        //        {
        //            quoteId = quote.Id,
        //            jobId = quote.JobAssignmentId,
        //            price = quote.Price,
        //            notes = quote.Notes,
        //            estimatedMinutes = quote.EstimatedMinutes,
        //            createdAt = quote.CreatedAt,
        //            handymanId = jobAssignment.HandymanId,
        //            handymanName = handymanUser?.FirstName + " " + handymanUser?.LastName,
        //            clientId = jobAssignment.Booking.ClientId,
        //            addressId = booking?.AddressId ?? client?.DefaultAddressId ?? 1,
        //            originalServices = services,
        //            jobStatus = jobAssignment.Status
        //        };

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while fetching quote by ID {QuoteId}", quoteId);
        //        return StatusCode(500, "An error occurred while processing your request.");
        //    }
        //}
        #endregion

        #region get all jobs
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
        #endregion

        //[HttpPut("MarkAsRead/{notificationId}")]
        //public IActionResult MarkAsRead(int notificationId)
        //{
        //    try
        //    {
        //        var notification =  _unitOfWork.NotificationRepo.GetById(notificationId);
        //        if (notification == null)
        //        {
        //            return NotFound(new { message = "Notification not found" });
        //        }

        //        notification.IsRead = true;
        //        //notification.CreatedAt = DateTime.UtcNow;

        //        _unitOfWork.NotificationRepo.Update(notification);
        //          _unitOfWork.SaveAsync();

        //        return Ok(new { message = "Notification marked as read" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error marking notification as read", error = ex.Message });
        //    }
        //}

        //// PUT: api/HandymanPage/MarkAllAsRead/{handymanUserId}
        //[HttpPut("MarkAllAsRead/{handymanUserId}")]
        //public IActionResult MarkAllAsRead(int handymanUserId)
        //{
        //    try
        //    {
        //        var notifications = _unitOfWork.NotificationRepo
        //            .GetAll(n => n.UserId == handymanUserId && !n.IsRead);

        //        foreach (var notification in notifications)
        //        {
        //            notification.IsRead = true;
        //            //notification.ReadAt = DateTime.UtcNow;
        //            _unitOfWork.NotificationRepo.Update(notification);
        //        }

        //        _unitOfWork.SaveAsync();

        //        return Ok(new { message = "All notifications marked as read" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error marking all notifications as read", error = ex.Message });
        //    }
        //}

      
    }
}
