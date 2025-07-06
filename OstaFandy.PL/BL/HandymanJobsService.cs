using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;
using Stripe;

namespace OstaFandy.PL.BL
{
    public class HandymanJobsService : IHandymanJobsService
    {
        private readonly AppDbContext _db;
        private readonly IAutoBookingService _autoBookingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HandymanJobsService> _logger;

        public HandymanJobsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HandymanJobsService> logger, IAutoBookingService autoBookingService, AppDbContext db)
        {
            _db = db;
            _autoBookingService = autoBookingService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _db = db;

        }

        #region get all jobs for the handyman
        public PaginationHelper<HandymanJobsDTO> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, string? status = null, int? handymanId = null)
        {
            try
            {
                var jobs = _unitOfWork.JobAssignmentRepo.GetAll(a => a.HandymanId == handymanId, includeProperties: "Booking,Booking.Client.User,Booking.Address");
                // search by frist name and email
                if (!string.IsNullOrEmpty(searchString))
                {
                    jobs = jobs.Where(j => j.Booking.Client.User.FirstName.Contains(searchString) || j.Booking.Client.User.Email.Contains(searchString));
                }
                // search by status 
                if (status != null)
                {
                    jobs = jobs.Where(j => j.Status == status);
                }
                var alljobs = _mapper.Map<IEnumerable<HandymanJobsDTO>>(jobs);
                return PaginationHelper<HandymanJobsDTO>.Create(alljobs, pageNumber, pageSize, searchString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching handyman jobs.");
                throw new Exception("An error occurred while fetching handyman jobs.", ex);
            }
        }
        #endregion

        #region send notification to client to update job status
        public bool SentNotificationToClientToUpdataStatus(int jobId, string status)
        {
            try
            {
                var alljob = _unitOfWork.JobAssignmentRepo.FirstOrDefault(a => a.Id == jobId);
                if (alljob == null)
                {
                    _logger.LogWarning($"Job with ID {jobId} not found.");
                    return false;
                }
                var booking = _unitOfWork.BookingRepo.GetById(alljob.BookingId);
                if (booking == null)
                {
                    _logger.LogWarning($"Booking with ID {alljob.BookingId} not found.");
                    return false;
                }
                var client = _unitOfWork.ClientRepo.GetById(booking.ClientId);
                if (client == null)
                {
                    _logger.LogWarning($"Client with ID {booking.ClientId} not found.");
                    return false;
                }
                var user = _unitOfWork.UserRepo.GetById(client.UserId);
                //var notification = new Notification
                //{
                //    UserId = user.Id,
                //    Type = $"{jobId},{status}",
                //    Title = $"Job Status Update Request",
                //    Message = $"The handyman has requested to change the status of job #{jobId} from '{alljob.Status}' to '{status}'. Please approve or reject this request.",
                //    CreatedAt = DateTime.UtcNow,
                //    IsRead = false
                //};
                //_unitOfWork.NotificationRepo.Insert(notification);
                //_unitOfWork.Save();
                var notification = new Notification
                {
                    UserId = user.Id,
                    Type = $"{jobId},{status}",
                    Title = "Job Status Update Request",
                    Message = $"The handyman has requested to change the status of job #{jobId} from '{alljob.Status}' to '{status}'. Please approve or reject this request.",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    RelatedEntityType = "JobAssignment",
                    RelatedEntityId = jobId
                };
                _unitOfWork.NotificationRepo.Insert(notification);
                _unitOfWork.Save();
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating job status for job ID {jobId}.");
                throw new Exception($"An error occurred while updating job status for job ID {jobId}.", ex);
            }
        }
        #endregion



       
        #region add quote
        public bool AddQuote(int jobId, decimal price, string Notes, int EstimatedMinutes)
        {
            try
            {
                var job = _unitOfWork.JobAssignmentRepo.GetById(jobId);
                if (job == null)
                {
                    _logger.LogWarning($"Job with ID {jobId} not found.");
                    return false;
                }
                if (price <= 0)
                {
                    _logger.LogWarning("Price must be greater than zero.");
                    return false;
                }
                if (EstimatedMinutes <= 0)
                {
                    _logger.LogWarning("EstimatedMinutes must be greater than zero.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(Notes))
                {
                    _logger.LogWarning("Notes cannot be empty.");
                    return false;
                }
                var quote = new OstaFandy.DAL.Entities.Quote
                {
                    JobAssignmentId = jobId,
                    Price = price,
                    Notes = Notes,
                    EstimatedMinutes = EstimatedMinutes,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.QuoteRepo.Insert(quote);
                _unitOfWork.Save();
                var quote2 = _db.Quotes
                    .Where(q => q.JobAssignmentId == jobId && q.Price == price && q.Notes == Notes && q.EstimatedMinutes == EstimatedMinutes)
                    .OrderByDescending(q => q.CreatedAt)
                    .FirstOrDefault();
                var booking = _unitOfWork.BookingRepo.GetById(job.BookingId);
                if (booking == null)
                {
                    _logger.LogWarning($"Booking with ID {job.BookingId} not found.");
                    return false;
                }
                var client = _unitOfWork.ClientRepo.GetById(booking.ClientId);
                if (client == null)
                {
                    _logger.LogWarning($"Client with ID {booking.ClientId} not found.");
                    return false;
                }
                var user = _unitOfWork.UserRepo.GetById(client.UserId);
                var notification = new Notification
                {
                    UserId = user.Id,
                    Type = "QuoteApproval",
                    Title = "New Quote Received",
                    Message = $"You have received a new quote for job #{jobId}. Price: ${price}. Please review and approve.",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    RelatedEntityType = "Quote",
                    RelatedEntityId = quote2.Id
                };

                _unitOfWork.NotificationRepo.Insert(notification);
                return _unitOfWork.Save() > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while adding quote for job ID {jobId}.");
                throw new Exception($"An error occurred while adding quote for job ID {jobId}.", ex);
            }

        }
        #endregion

        #region process quote response async function
        public async Task<bool> ProcessQuoteResponseAsync(int quoteId, string action, int clientUserId, CreateBookingDTO bookingDto = null)
        {
            try
            {
                var quote = _unitOfWork.QuoteRepo.GetById(quoteId);
                if (quote == null)
                {
                    _logger.LogWarning($"Quote with ID {quoteId} not found.");
                    return false;
                }

                if (action.ToLower() == "reject")
                {
                    quote.Status = "Rejected";
                    _unitOfWork.QuoteRepo.Update(quote);

                    // Mark notification as read
                    var notification = _unitOfWork.NotificationRepo
                        .GetAll()
                        .FirstOrDefault(n => n.RelatedEntityId == quoteId && n.RelatedEntityType == "Quote" && n.UserId == clientUserId);

                    if (notification != null)
                    {
                        notification.IsRead = true;
                        _unitOfWork.NotificationRepo.Update(notification);
                    }

                    // Notify handyman about rejection
                    var jobAssignment = _unitOfWork.JobAssignmentRepo.GetById(quote.JobAssignmentId);
                    var handyman = _unitOfWork.HandyManRepo.GetById(jobAssignment.HandymanId);

                    var handymanNotification = new Notification
                    {
                        UserId = handyman.UserId,
                        Type = "QuoteRejected",
                        Title = "Quote Rejected",
                        Message = $"Your quote for job #{quote.JobAssignmentId} has been rejected by the client.",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        RelatedEntityType = "Quote",
                        RelatedEntityId = quoteId
                    };

                    _unitOfWork.NotificationRepo.Insert(handymanNotification);

                    return _unitOfWork.Save() > 0;
                }
                else if (action.ToLower() == "accept" && bookingDto != null)
                {
                    quote.Status = "Accepted";
                    _unitOfWork.QuoteRepo.Update(quote);

                    // Create the booking using the provided DTO
                    var bookingResult = await _autoBookingService.CreateBooking(bookingDto);

                    if (bookingResult != null && bookingResult.BookingId > 0)
                    {
                        // Mark notification as read
                        var notification = _unitOfWork.NotificationRepo
                            .GetAll()
                            .FirstOrDefault(n => n.RelatedEntityId == quoteId && n.RelatedEntityType == "Quote" && n.UserId == clientUserId);

                        if (notification != null)
                        {
                            notification.IsRead = true;
                            _unitOfWork.NotificationRepo.Update(notification);
                        }

                        // Notify handyman about acceptance
                        var jobAssignment = _unitOfWork.JobAssignmentRepo.GetById(quote.JobAssignmentId);
                        var handyman = _unitOfWork.HandyManRepo.GetById(jobAssignment.HandymanId);

                        var handymanNotification = new Notification
                        {
                            UserId = handyman.UserId,
                            Type = "QuoteAccepted",
                            Title = "Quote Accepted - New Booking Created",
                            Message = $"Your quote has been accepted! New booking #{bookingResult.BookingId} has been created for {bookingDto.PreferredDate:MMM dd, yyyy HH:mm}.",
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false,
                            RelatedEntityType = "Booking",
                            RelatedEntityId = bookingResult.BookingId
                        };

                        _unitOfWork.NotificationRepo.Insert(handymanNotification);

                        return _unitOfWork.Save() > 0;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while processing quote response for quote ID {quoteId}.");
                throw new Exception($"An error occurred while processing quote response.", ex);
            }
        }
        #endregion

        #region get quote details
        public QuoteDetailsDTO GetQuoteDetails(int quoteId)
        {
            try
            {
                var quote = _unitOfWork.QuoteRepo.GetById(quoteId);
                if (quote == null) return null;

                var jobAssignment = _unitOfWork.JobAssignmentRepo.GetById(quote.JobAssignmentId);
                var booking = _unitOfWork.BookingRepo.GetById(jobAssignment.BookingId);
                var handyman = _unitOfWork.HandyManRepo.GetById(jobAssignment.HandymanId);
                var user = _unitOfWork.UserRepo.GetById(handyman.UserId);

                return new QuoteDetailsDTO
                {
                    QuoteId = quote.Id,
                    JobId = quote.JobAssignmentId,
                    HandymanId = jobAssignment.HandymanId,
                    HandymanName = $"{user.FirstName} {user.LastName}",
                    Price = quote.Price,
                    EstimatedMinutes = (int)quote.EstimatedMinutes,
                    Notes = quote.Notes,
                    OriginalBookingId = booking.Id,
                    ClientId = booking.ClientId,
                    AddressId = booking.AddressId,
                    OriginalPreferredDate = booking.PreferredDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting quote details for quote ID {quoteId}.");
                return null;
            }
        }
        #endregion


        #region get the origin book this not supposed to be here but it is fine
        public List<BookingServiceDTO> GetOriginalBookingServices(int originalBookingId)
        {
            try
            {
                var bookingServices = _unitOfWork.BookingServiceRepo
                    .GetAll()
                    .Where(bs => bs.BookingId == originalBookingId)
                    .Select(bs => new BookingServiceDTO
                    {
                        ServiceId = bs.ServiceId,
                        Quantity = 1 // Default quantity, adjust as needed
                    })
                    .ToList();

                return bookingServices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting original booking services for booking ID {originalBookingId}.");
                return new List<BookingServiceDTO>();
            }
        }

        // Enhanced GetQuoteDetails method with original booking services
        public QuoteDetailsDTO GetQuoteDetailsWithServices(int quoteId)
        {
            try
            {
                var quote = _unitOfWork.QuoteRepo.GetById(quoteId);
                if (quote == null) return null;

                var jobAssignment = _unitOfWork.JobAssignmentRepo.GetById(quote.JobAssignmentId);
                var booking = _unitOfWork.BookingRepo.GetById(jobAssignment.BookingId);
                var handyman = _unitOfWork.HandyManRepo.GetById(jobAssignment.HandymanId);
                var user = _unitOfWork.UserRepo.GetById(handyman.UserId);

                // Get original booking services
                var originalServices = GetOriginalBookingServices(booking.Id);

                return new QuoteDetailsDTO
                {
                    QuoteId = quote.Id,
                    JobId = quote.JobAssignmentId,
                    HandymanId = jobAssignment.HandymanId,
                    HandymanName = $"{user.FirstName} {user.LastName}",
                    Price = quote.Price,
                    EstimatedMinutes = (int)quote.EstimatedMinutes,
                    Notes = quote.Notes,
                    OriginalBookingId = booking.Id,
                    ClientId = booking.ClientId,
                    AddressId = booking.AddressId,
                    OriginalPreferredDate = booking.PreferredDate,
                    OriginalServices = originalServices
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting quote details for quote ID {quoteId}.");
                return null;
            }
        }

        #endregion

        #region get handyman quotes
        public PaginationHelper<AllQuotes> GetHandymanQuotes(int handymanId, int pageNumber,int pageSize,string searchString)
        {
            if (handymanId <= 0)
            {
                _logger.LogWarning("Handyman ID must be a positive integer.");
                return new PaginationHelper<AllQuotes>();
            }

            var quotes = _unitOfWork.QuoteRepo.GetAll(q => q.JobAssignment.HandymanId == handymanId, includeProperties: "JobAssignment,JobAssignment.Booking,JobAssignment.Booking.Client.User");
            //, includeProperties: "JobAssignment,JobAssignment.Booking,JobAssignment.Booking.Client.User"
            var validQuotes = new List<OstaFandy.DAL.Entities.Quote>();
            foreach (var quote in quotes)
            {
                if (quote.JobAssignment == null ||
                    quote.JobAssignment.Booking == null ||
                    quote.JobAssignment.Booking.Client == null ||
                    quote.JobAssignment.Booking.Client.User == null)
                {
                    _logger.LogWarning($"Quote with ID {quote.Id} has incomplete job assignment or booking information.");
                    continue;
                }
                validQuotes.Add(quote);
            }
            var mappedQuotes = _mapper.Map<List<AllQuotes>>(validQuotes);
            return PaginationHelper<AllQuotes>.Create(mappedQuotes, pageNumber, pageSize, searchString);
        }
        #endregion
    }
}
