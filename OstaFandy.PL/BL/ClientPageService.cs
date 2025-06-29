using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;

namespace OstaFandy.PL.BL
{
    public class ClientPageService : IClientPageService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ClientPageService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ClientPageService(IMapper mapper, ILogger<ClientPageService> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public bool ApproveJobStatusChange(int jobId, string approvedStatus, int clientUserId)
        {
            try
            {
                var jobAssignment = _unitOfWork.JobAssignmentRepo.FirstOrDefault(a => a.Id == jobId);
                if (jobAssignment == null)
                {
                    _logger.LogWarning($"Job with ID {jobId} not found for approval.");
                    return false;
                }

                if (jobAssignment.Status == "Completed" || jobAssignment.Status == "Cancelled")
                {
                    _logger.LogWarning($"Invalid status transition for job {jobId}: from {jobAssignment.Status} to {approvedStatus}");
                    return false;
                }
                if (jobAssignment.Status == "InProgress" && approvedStatus == "Assigned")
                {
                    _logger.LogWarning($"Invalid status transition for job {jobId}: from {jobAssignment.Status} to {approvedStatus}");
                    return false;
                }


                jobAssignment.Status = approvedStatus;
                _unitOfWork.JobAssignmentRepo.Update(jobAssignment);

                var notifications = _unitOfWork.NotificationRepo.GetAll(n => n.UserId == clientUserId);

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    _unitOfWork.NotificationRepo.Update(notification);
                }
                _unitOfWork.Save();
                _logger.LogInformation($"Job {jobId} status successfully updated to {approvedStatus} by client approval.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while approving job status for job ID {jobId}.");
                throw new Exception($"An error occurred while approving job status for job ID {jobId}.", ex);
            }
        }
    




    public bool ApproveQuoteStatusChange(int jobId, string approvedStatus, int clientUserId)
        {
            try
            {
                var quote = _unitOfWork.QuoteRepo.GetQuoteByJobId(jobId);
                if (quote == null)
                {
                    _logger.LogWarning($"quote with JobID {jobId} not found for approval.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(approvedStatus))
                {
                    _logger.LogWarning($"Status must have a value.");
                    return false;
                }
                var jobAssignment = _unitOfWork.JobAssignmentRepo.GetById(jobId);
                if (jobAssignment == null)
                {
                    _logger.LogWarning($"Job assignment with ID {jobId} not found.");
                    return false;
                }
                var booking = _unitOfWork.BookingRepo.GetById(jobAssignment.BookingId);
                if (booking == null)
                {
                    _logger.LogWarning($"Booking with ID {jobAssignment.BookingId} not found for job assignment {jobId}.");
                    return false;
                }

                if (clientUserId != booking.ClientId)
                {
                    _logger.LogWarning($"Client user ID {clientUserId} is not authorized to approve quote for job assignment ID {jobId}. Expected client ID: {booking.ClientId}");
                    return false;
                }



                quote.Status = approvedStatus;
                _unitOfWork.QuoteRepo.Update(quote);

                var notifications = _unitOfWork.NotificationRepo.GetAll(n => n.UserId == clientUserId);

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    _unitOfWork.NotificationRepo.Update(notification);
                }
                _unitOfWork.Save();
                _logger.LogInformation($"Job {jobId} status successfully updated to {approvedStatus} by client approval.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while approving quote status for job assignment ID {jobId}.");
                throw new Exception($"An error occurred while approving quote status for job assignment ID {jobId}.", ex);
            }
        }
    }
}