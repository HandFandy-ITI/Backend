using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

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
                    Type = "Change job status",
                    Title = "Job Status Update",
                    Message = $"The status of your job with ID {jobId} need your approve to updated to '{status}' status.",
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
        
    }
}
