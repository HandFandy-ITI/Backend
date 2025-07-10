using System.Collections.Concurrent;
using System;
using Microsoft.AspNetCore.SignalR;
using OstaFandy.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OstaFandy.PL.BL
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("NameIdentifier")?.Value;

             Console.WriteLine($"All claims: {string.Join(", ", Context.User?.Claims.Select(c => $"{c.Type}={c.Value}") ?? new string[0])}");
            Console.WriteLine($"Extracted userId: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
                Console.WriteLine($"Fallback userId from query: {userId}");
            }

            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections[userId] = Context.ConnectionId;
                Console.WriteLine($"User {userId} connected with connection ID: {Context.ConnectionId}");
            }
            else
            {
                Console.WriteLine("No userId found - connection will not be tracked");
            }

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userToRemove = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (!userToRemove.Equals(default(KeyValuePair<string, string>)))
            {
                UserConnections.TryRemove(userToRemove.Key, out _);
                Console.WriteLine($"User {userToRemove.Key} disconnected");
            }
            await base.OnDisconnectedAsync(exception);
        }
        public static bool TryGetConnectionId(string userId, out string connectionId)
        {
            return UserConnections.TryGetValue(userId, out connectionId);
        }
        public static Dictionary<string, string> GetAllConnections()
        {
            return new Dictionary<string, string>(UserConnections);
        }

        public async Task SendNotificationToUser(string userId, string message)
        {
            if (UserConnections.TryGetValue(userId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }
        public async Task SendNotificationToUserHandyman(string userId, string message)
        {
            if (UserConnections.TryGetValue(userId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotificationhandyman", message);
            }
        }

        public async Task SendNotificationToAdmin(string userId, string message)
        {
            if (UserConnections.TryGetValue(userId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotificationAdmin", message);
            }
        }

    }
}