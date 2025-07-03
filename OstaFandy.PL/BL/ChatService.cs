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

        //public void SendMessage(MessageDTO dto)
        //{
        //    if (!dto.SenderId.HasValue)
        //        throw new ArgumentException("SenderId is required");

        //    var message = new Message
        //    {
        //        ChatId = dto.ChatId,
        //        SenderId = dto.SenderId.Value,
        //        Content = dto.Content,
        //        SentAt = DateTime.UtcNow,
        //        IsRead = false
        //    };

        //    _unit.MessageRepo.Insert(message);
        //    _unit.Save();
        //}

        public void SendMessage(MessageDTO dto)
        {
            if (!dto.SenderId.HasValue)
                throw new ArgumentException("SenderId is required");

            // ✅ Ensure the sender is a valid user
            var sender = _unit.UserRepo.GetById(dto.SenderId.Value);
            if (sender == null)
                throw new Exception($"User with ID {dto.SenderId.Value} not found in Users table.");

            // ✅ Ensure the chat exists
            var chat = _unit.ChatRepo.GetById(dto.ChatId);
            if (chat == null)
                throw new Exception($"Chat with ID {dto.ChatId} not found.");

            // ✅ Create the message entity
            var message = new Message
            {
                ChatId = dto.ChatId,
                SenderId = dto.SenderId.Value,
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            // ✅ Save with safe error handling
            try
            {
                _unit.MessageRepo.Insert(message);
                _unit.Save();
            }
            catch (Exception ex)
            {
                // Log this internally or return a clean error message
                throw new Exception("Failed to send message. Details: " + ex.Message);
            }
        }


        //public IEnumerable<MessageDTO> GetMessages(int chatId)
        //{
        //    return _unit.MessageRepo.GetByChatId(chatId)
        //        .OrderBy(m => m.SentAt)
        //        .Select(m => new MessageDTO
        //        {
        //            ChatId = m.ChatId,
        //            SenderId = m.SenderId,
        //            Content = m.Content,
        //            SentAt = m.SentAt
        //        });
        //}


        public IEnumerable<MessageDTO> GetMessages(int chatId)
        {
            var messages = _unit.MessageRepo.GetByChatId(chatId);

            return messages.Select(m => new MessageDTO
            {
                ChatId = m.ChatId,
                SenderId = m.SenderId,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderName = m.Sender?.FirstName + " " + m.Sender?.LastName,
                SenderRole = m.Sender?.UserTypes.FirstOrDefault()?.TypeName ?? "Unknown"
            });
        }



        //public IEnumerable<ChatThreadDTO> GetHandymanThreads(int handymanUserId)
        //{
        //    var assignments = _unit.JobAssignmentRepo.GetAll(
        //        j => j.HandymanId == handymanUserId &&
        //             j.Status == "InProgress" &&
        //             j.IsActive,
        //        includeProperties: "Booking.Client.User,Booking.Chats.Messages"
        //    );

        //    var result = new List<ChatThreadDTO>();

        //    foreach (var job in assignments)
        //    {
        //        var booking = job.Booking;
        //        var clientUser = booking.Client.User;
        //        var chat = booking.Chats.FirstOrDefault();

        //        if (chat == null)
        //        {
        //            int chatId = EnsureChatExists(booking.Id);
        //            chat = _unit.ChatRepo.GetById(chatId);
        //        }

        //        var lastMessage = chat.Messages
        //            .OrderByDescending(m => m.SentAt)
        //            .FirstOrDefault();

        //        result.Add(new ChatThreadDTO
        //        {
        //            ChatId = chat.Id,
        //            BookingId = booking.Id,
        //            ClientName = clientUser.FirstName + " " + clientUser.LastName,
        //            LastMessage = lastMessage?.Content,
        //            LastMessageTime = lastMessage?.SentAt
        //        });
        //    }

        //    return result;
        //}
        public IEnumerable<ChatThreadDTO> GetHandymanThreads(int handymanUserId)
        {
            var assignments = _unit.JobAssignmentRepo.GetAll(
                j => j.HandymanId == handymanUserId &&
                     j.Status == "Assigned" &&
                     j.IsActive,
                includeProperties: "Booking.Client.User,Booking.Chats.Messages"
            );

            var result = new List<ChatThreadDTO>();

            foreach (var job in assignments)
            {
                var booking = job.Booking;
                var clientUser = booking.Client?.User;

                if (clientUser == null) continue; // Defensive check

                // Ensure chat exists
                var chat = booking.Chats.FirstOrDefault();
                if (chat == null)
                {
                    int chatId = EnsureChatExists(booking.Id);
                    chat = _unit.ChatRepo.GetById(chatId);
                }

                // Now safe to access messages
                var lastMessage = chat?.Messages?
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefault();

                result.Add(new ChatThreadDTO
                {
                    ChatId = chat.Id,
                    BookingId = booking.Id,
                    ClientName = $"{clientUser.FirstName} {clientUser.LastName}",
                    LastMessage = lastMessage?.Content,
                    LastMessageTime = lastMessage?.SentAt
                });
            }

            return result;
        }

    }
}
