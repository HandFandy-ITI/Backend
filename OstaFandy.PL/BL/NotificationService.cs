using Microsoft.AspNetCore.SignalR;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;

namespace OstaFandy.PL.BL
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork, ILogger<NotificationService> logger)
        {
            _logger = logger;
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
        }

        public async Task SendNotificationToHandyman(string handymanUserId, string message)
        {
            _logger.LogInformation($"Sending notification to handyman {handymanUserId}: {message}");

             if (NotificationHub.TryGetConnectionId(handymanUserId, out var connectionId))
            {
                 await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotificationhandyman", message);
                _logger.LogInformation($"Notification sent to handyman {handymanUserId} via connection {connectionId}");
            }
            else
            {
                // Fallback: Send to all clients with user ID filter (client-side filtering)
                await _hubContext.Clients.All.SendAsync("ReceiveNotificationhandyman", handymanUserId, message);
                _logger.LogWarning($"Handyman {handymanUserId} not found in active connections, sent to all clients");
            }
            }

        public async Task SendNotificationToAdmin(string AdminUserId, string message)
        {

            if (NotificationHub.TryGetConnectionId(AdminUserId, out var connectionId))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotificationAdmin", message);
                _logger.LogInformation($"Notification sent to admin {AdminUserId} via connection {connectionId}");
            }
            else
            {
                 await _hubContext.Clients.All.SendAsync("ReceiveNotificationAdmin", AdminUserId, message);
                _logger.LogWarning($"amdin {AdminUserId} not found in active connections");
            }
        }

        public async Task SendNotificationToClient(string clientUserId, int jobId, string status)
        {
            var message = $"You Have A New Job Update";
            _logger.LogInformation($"Sending job status update to client {clientUserId} for job {jobId}: {status}");

            if (NotificationHub.TryGetConnectionId(clientUserId, out var connectionId))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveJobStatusUpdate", clientUserId, jobId, status);
                _logger.LogInformation($"Job status update sent to client {clientUserId} via connection {connectionId}");
            }
            else
            {
                // Fallback: Send to all clients with user ID filter
                await _hubContext.Clients.All.SendAsync("ReceiveJobStatusUpdate", clientUserId, jobId, status);
                _logger.LogWarning($"Client {clientUserId} not found in active connections, sent to all clients");
            }
        }


        public async Task SendQuoteResponse(int userId, int quoteId, string action)
        {
            var message = action.ToLower() == "accept"
                ? $"Your quote #{quoteId} has been accepted by the client"
                : $"Your quote #{quoteId} has been rejected by the client";

            _logger.LogInformation($"Sending quote response to user {userId}: {message}");

            if (NotificationHub.TryGetConnectionId(userId.ToString(), out var connectionId))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveQuoteResponse", quoteId, action, message);
                _logger.LogInformation($"Quote response sent to user {userId} via connection {connectionId}");
            }
            else
            {
                // Fallback: Send to all clients with user ID filter
                await _hubContext.Clients.All.SendAsync("ReceiveQuoteResponse", userId, quoteId, action, message);
                _logger.LogWarning($"User {userId} not found in active connections, sent to all clients");
            }
        }
    }
}