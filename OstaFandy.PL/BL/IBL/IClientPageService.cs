using OstaFandy.DAL.Entities;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IClientPageService
    {
        bool ApproveJobStatusChange(int jobId, string approvedStatus, int clientUserId);
        bool ApproveQuoteStatusChange(int jobId, string approvedStatus, int clientUserId);

        Task<List<AvailableTimeSlotForHandyman>> GetAvailableTimeSlotForHandymanAsync(int HandymanId, DateTime Day, int estimatedMinutes);
    }
}
