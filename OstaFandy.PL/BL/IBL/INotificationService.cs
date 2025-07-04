using OstaFandy.DAL.Entities;

namespace OstaFandy.PL.BL.IBL
{
    public interface INotificationService
    {
        Task SendNotificationToHandyman(string handymanUserId, string message);
        Task SendNotificationToClient(string clientUserId, int jobId, string status);
        Task SendQuoteResponse(string userId, int quoteId, string action);
 

        // NEW: Methods for handyman notifications
        //Task SendJobAssignmentToHandyman(string handymanUserId, int jobId, string jobTitle);
        //Task SendQuoteRequestToHandyman(string handymanUserId, int jobId, string jobTitle);
        //Task SendGeneralNotificationToHandyman(string handymanUserId, string title, string message);
        //Task SendJobStatusUpdateToHandyman(string handymanUserId, int jobId, string status);

        // NEW: Bulk notification methods
        //Task SendBulkNotificationToHandymen(List<string> handymanUserIds, string title, string message);
        //Task SendNotificationToAllHandymen(string title, string message);

        // NEW: Notification management methods
        //Task<List<Notification>> GetNotificationsByUserId(int userId);
        //Task<int> GetUnreadNotificationCount(int userId);
        //Task MarkNotificationAsRead(int notificationId);
        //Task MarkAllNotificationsAsRead(int userId);
        //Task DeleteNotification(int notificationId);
        //Task DeleteOldNotifications(int daysOld = 30);
    }
}
