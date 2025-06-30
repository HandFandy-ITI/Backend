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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HandymanJobsService> _logger;

        public HandymanJobsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HandymanJobsService> logger)
        {
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
                    Type = $"{jobId},{status}",
                    Title = $"Job Status Update Request",
                    Message = $"The handyman has requested to change the status of job #{jobId} from '{alljob.Status}' to '{status}'. Please approve or reject this request.",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
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



        #region add quote to the job
        //public bool AddQuote(int jobId, decimal price, string Notes)
        //{
        //    try
        //    {
        //        var job = _unitOfWork.JobAssignmentRepo.GetById(jobId);
        //        if (job == null)
        //        {
        //            _logger.LogWarning($"Job with ID {jobId} not found.");
        //            return false;
        //        }
        //        if (price <= 0)
        //        {
        //            _logger.LogWarning("Price must be greater than zero.");
        //            return false;
        //        }
        //        if (string.IsNullOrWhiteSpace(Notes))
        //        {
        //            _logger.LogWarning("Notes cannot be empty.");
        //            return false;
        //        }
        //        _unitOfWork.QuoteRepo.Insert(new OstaFandy.DAL.Entities.Quote
        //        {
        //            JobAssignmentId = jobId,
        //            Price = price,
        //            Notes = Notes,
        //            Status = "Pending",
        //            CreatedAt = DateTime.UtcNow
        //        });
        //        var booking = _unitOfWork.BookingRepo.GetById(job.BookingId);
        //        if (booking == null)
        //        {
        //            _logger.LogWarning($"Booking with ID {job.BookingId} not found.");
        //            return false;
        //        }
        //        var client = _unitOfWork.ClientRepo.GetById(booking.ClientId);
        //        if (client == null)
        //        {
        //            _logger.LogWarning($"Client with ID {booking.ClientId} not found.");
        //            return false;
        //        }
        //        var user = _unitOfWork.UserRepo.GetById(client.UserId);
        //        var notification = new Notification
        //        {
        //            UserId = user.Id,
        //            Type = "{Change job status}",
        //            Title = $"Acceptance for new job assign",
        //            Message = $"The status of your job with ID {jobId} need your approve to be provide.",
        //            CreatedAt = DateTime.UtcNow,
        //            IsRead = false
        //        };
        //        _unitOfWork.NotificationRepo.Insert(notification);
        //        return _unitOfWork.Save() > 1;

        //    } catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error occurred while adding quote for job ID {jobId}.");
        //        throw new Exception($"An error occurred while adding quote for job ID {jobId}.", ex);
        //    }
        //}
        public bool AddQuote(int jobId, decimal price, string Notes)
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
                if (string.IsNullOrWhiteSpace(Notes))
                {
                    _logger.LogWarning("Notes cannot be empty.");
                    return false;
                }
                _unitOfWork.QuoteRepo.Insert(new OstaFandy.DAL.Entities.Quote
                {
                    JobAssignmentId = jobId,
                    Price = price,
                    Notes = Notes,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });
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
                    Type = "{Change job status}",
                    Title = $"Acceptance for new job assign",
                    Message = $"The status of your job with ID {jobId} need your approve to be provide.",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
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
