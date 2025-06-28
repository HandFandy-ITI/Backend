using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OstaFandy.PL.BL
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unit;

        public ChatService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public int EnsureChatExists(int bookingId)
        {
            var chat = _unit.ChatRepo.GetByBookingId(bookingId);
            if (chat != null) return chat.Id;

            var newChat = new Chat
            {
                BookingId = bookingId,
                StartedAt = DateTime.UtcNow
            };

            _unit.ChatRepo.Insert(newChat);
            _unit.Save();
            return newChat.Id;
        }

        public void SendMessage(MessageDTO dto)
        {
            if (!dto.SenderId.HasValue)
                throw new ArgumentException("SenderId is required");

            var message = new Message
            {
                ChatId = dto.ChatId,
                SenderId = dto.SenderId.Value,
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _unit.MessageRepo.Insert(message);
            _unit.Save();
        }


        public IEnumerable<MessageDTO> GetMessages(int chatId)
        {
            return _unit.MessageRepo.GetByChatId(chatId)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDTO
                {
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    Content = m.Content
                });
        }
    }
}
