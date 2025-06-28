using Microsoft.AspNetCore.SignalR;
using OstaFandy.PL.DTOs;
using System.Threading.Tasks;

namespace OstaFandy.PL.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task SendMessage(MessageDTO  message)
        {
            await Clients.Group(message.ChatId.ToString())
                .SendAsync("ReceiveMessage", message);
        }
    }
}
