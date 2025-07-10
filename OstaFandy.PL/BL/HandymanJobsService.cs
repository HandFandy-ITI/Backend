using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;
using RTools_NTS.Util;
using Stripe;

namespace OstaFandy.PL.BL
{
    public class HandymanJobsService : IHandymanJobsService
    {
        private readonly INotificationService _notificationService;
        private readonly AppDbContext _db;
        private readonly IAutoBookingService _autoBookingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HandymanJobsService> _logger;

        public HandymanJobsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HandymanJobsService> logger, IAutoBookingService autoBookingService, AppDbContext db, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _db = db;
            _autoBookingService = autoBookingService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;

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
                var client = _unitOfWork.ClientRepo.GetByIdSync(booking.ClientId);
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
            public async Task<bool> AddQuote(int jobId, decimal price, string Notes, int EstimatedMinutes)
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
                    var client = _unitOfWork.ClientRepo.GetByIdSync(booking.ClientId);
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
                    //await _notificationService.SendNotificationToClient(user.Id.ToString(), jobId, "QuoteReceived");
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
                        Quantity = 1 
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
        public PaginationHelper<AllQuotes> GetHandymanQuotes(int handymanId, int pageNumber, int pageSize, string searchString)
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

        #region ask for a vacation
        public async Task<bool> ApplyForBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate)
        {
            try
            {
                var handyman = _unitOfWork.HandyManRepo.FirstOrDefault(h => h.UserId == HandymanId, includeProperties: "User,Specialization");
                if (handyman == null)
                {
                    _logger.LogError("handyman does not exist");
                    return false;
                }
                if (!handyman.User.IsActive)
                {
                    _logger.LogError("handyman is NOT active");
                    return false;
                }
                var jobs = _unitOfWork.JobAssignmentRepo.CheckJobInSpecificDate(StartDate, EndDate);
                if (jobs)
                {
                    _logger.LogError("you have jobs in this period");
                    return false;
                }

                var existingBlockDates = _unitOfWork.BlockDateRepo.GetAll(a => a.UserId == HandymanId);
                foreach (var blockDate in existingBlockDates)
                {
                    if (AreDatesOverlapping(DateOnly.FromDateTime(blockDate.StartDate), DateOnly.FromDateTime(blockDate.EndDate), StartDate, EndDate))
                    {
                        _logger.LogError("you already has a vacation in this period");
                        return false;
                    }
                }

                var x = new BlockDate()
                {
                    UserId = HandymanId,
                    Reason = Reason,
                    StartDate = StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = EndDate.ToDateTime(TimeOnly.MaxValue),
                    IsActive = false,
                    Status = "Pending"
                };
                _unitOfWork.BlockDateRepo.Insert(x);
                _unitOfWork.Save();
                await _notificationService.SendNotificationToHandyman(HandymanId.ToString(), $"you have applied for a days OFF from {StartDate} to {EndDate} waiting for admin to approve");
                var notification = new Notification
                {
                    UserId = HandymanId,
                    Title = "Days OFF",
                    Message = $"you have applied for a days OFF from {StartDate} to {EndDate} waiting for admin to approve",
                    Type = "vacation",
                    IsRead = false,
                    IsActive = true
                };
                var admins = _unitOfWork.UserRepo.GetAll(a => a.UserTypes.Any(u => u.Id == 2)).ToList();
                foreach (var admin in admins)
                {
                    var notification2 = new Notification
                    {
                        UserId = admin.Id,
                        Title = "Days OFF",
                        Message = $"handyman with Id {HandymanId} and name {handyman.User.FirstName}{handyman.User.LastName} in specialize {handyman.Specialization.Name} applied for a days OFF from {StartDate} to {EndDate} the reason is {Reason}waiting for your approve",
                        Type = $"{HandymanId},{Reason},{StartDate},{EndDate}",
                        IsRead = false,
                        IsActive = true
                    };
                   await _notificationService.SendNotificationToAdmin(admin.Id.ToString(), $"Handyman with Id {HandymanId} and name {handyman.User.FirstName} {handyman.User.LastName} in specialization {handyman.Specialization.Name} has applied for a days OFF from {StartDate} to {EndDate} waiting for you to approve");
                    _unitOfWork.NotificationRepo.Insert(notification2);
                }
                _unitOfWork.NotificationRepo.Insert(notification);
                _unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error will adding block date");
                throw;
            }
            return true;
        }
        private bool AreDatesOverlapping(DateOnly startDate1, DateOnly endDate1, DateOnly startDate2, DateOnly endDate2)
        {
            return !(endDate1 < startDate2 || endDate2 < startDate1);
        }
        #endregion

        #region get handyman block date

        public PaginationHelper<HandymanBlockDateDTO> GetHandymanBlockDate(int pageNumber = 1, int pageSize = 5, string? status = null, DateTime? Date = null, int? handymanId = null, string searchString = "")
        {
            try
            {
                if (handymanId == null)
                {
                    _logger.LogError("handymanId does not exist");
                    return PaginationHelper<HandymanBlockDateDTO>.Create(new List<HandymanBlockDateDTO>(), pageNumber, pageSize, searchString);
                }
                var blockDates = _unitOfWork.BlockDateRepo.GetAll(a => a.UserId == handymanId, includeProperties: "User.User").AsQueryable();
                if (Date != null)
                {
                    blockDates = blockDates
                        .Where(b => b.StartDate.Date == Date || b.EndDate.Date == Date);
                }
                if (!string.IsNullOrEmpty(status))
                {
                    blockDates = blockDates
                        .Where(b => b.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
                }
                var x = blockDates.Select(b => new HandymanBlockDateDTO
                {
                    userId = b.UserId,
                    Name = $"{b.User.User.FirstName ?? ""} {b.User.User.LastName ?? ""}",
                    Status = b.Status,
                    Reason = b.Reason,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate
                });
                var dtolist = x.ToList();
                return PaginationHelper<HandymanBlockDateDTO>.Create(x, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting block dates.");
                throw;
            }
        }

        #endregion
    }
}