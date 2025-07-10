using OstaFandy.DAL.Entities;

namespace OstaFandy.PL.BL.IBL
{
    public interface INotificationService
    {
        Task SendNotificationToHandyman(string handymanUserId, string message);
        Task SendNotificationToClient(string clientUserId, int jobId, string status);
        Task SendQuoteResponse(int userId, int quoteId, string action);
        Task SendNotificationToAdmin(string AdminUserId, string message);
        Task SendQuoteNotificationToClient(string clientUserId, int jobId, string message);
    }
}
