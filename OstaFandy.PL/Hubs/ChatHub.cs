using Microsoft.AspNetCore.SignalR;
using OstaFandy.PL.DTOs;
using System.Threading.Tasks;

namespace OstaFandy.PL.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinChat(string chatId)
        {
            Console.WriteLine($"✅ JOIN: {Context.ConnectionId} joined chat-{chatId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }


        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }

        public async Task SendMessage(MessageDTO message)
        {
            Console.WriteLine($"📨 Broadcasting message: {message.Content} to chat-{message.ChatId}");
            await Clients.Group($"chat-{message.ChatId}")
                .SendAsync("ReceiveMessage", message);
        }
    }
}
