using Microsoft.AspNetCore.SignalR;
using OstaFandy.PL.DTOs;
using System.Threading.Tasks;

namespace OstaFandy.PL.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinChat(string chatId)
        {
            Console.WriteLine($" JOIN: {Context.ConnectionId} joined chat-{chatId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }


        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }

        public async Task SendMessage(MessageDTO message)
        {
            Console.WriteLine($" Broadcasting message: {message.Content} to chat-{message.ChatId}");
            await Clients.Group($"chat-{message.ChatId}")
                .SendAsync("ReceiveMessage", message);
        }


        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("NameIdentifier")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
                Console.WriteLine($"👤 Connected user-{userId} added to group");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("NameIdentifier")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
                Console.WriteLine($"👤 Disconnected user-{userId} removed from group");
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
