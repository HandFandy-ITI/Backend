using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.General;
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






        public PaginatedResult<ChatThreadDTO> GetHandymanThreads(int handymanUserId, ChatThreadFilterDTO filters)
        {
            var assignments = _unit.JobAssignmentRepo.GetAll(
                j => j.HandymanId == handymanUserId &&
                     (j.Status == JobAssignmentsStatus.Assigned || j.Status == JobAssignmentsStatus.InProgress) &&
                     j.IsActive &&
                     j.Booking.IsActive,
                includeProperties: "Booking.Client.User,Booking.Chats.Messages,Booking.BookingServices.Service.Category,Handyman.User"
            );

            var result = new List<ChatThreadDTO>();

            foreach (var job in assignments)
            {
                var booking = job.Booking;
                var client = booking.Client?.User;
                var handyman = job.Handyman?.User;

                var chat = booking.Chats.FirstOrDefault();
                if (chat == null)
                {
                    int chatId = EnsureChatExists(booking.Id);
                    chat = _unit.ChatRepo.GetById(chatId); // ✅ Load from DB with messages
                }

                var lastMessage = chat.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault();
                var bookingService = booking.BookingServices.FirstOrDefault();

                var serviceName = bookingService?.Service?.Name ?? "N/A";

                if (client != null && handyman != null)
                {
                    result.Add(new ChatThreadDTO
                    {
                        ChatId = chat.Id,
                        BookingId = booking.Id,
                        ClientName = $"{client.FirstName} {client.LastName}",
                        HandymanName = $"{handyman.FirstName} {handyman.LastName}",
                        LastMessage = lastMessage?.Content ?? "",
                        LastMessageTime = lastMessage?.SentAt,
                        BookingDate = booking.PreferredDate,
                        ServiceName = serviceName
                    });
                }
            }

            // 🔍 Apply name filter
            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                result = result.Where(t =>
                    t.ClientName.ToLower().Contains(filters.Name.ToLower())).ToList();
            }

            // 🔃 Sort
            result = filters.Sort == "oldest"
                ? result.OrderBy(t => t.LastMessageTime).ToList()
                : result.OrderByDescending(t => t.LastMessageTime).ToList();

            // 📄 Pagination
            var total = result.Count;
            var items = result
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            return new PaginatedResult<ChatThreadDTO>
            {
                PageNumber = filters.PageNumber,
                PageSize = filters.PageSize,
                TotalItems = total,
                Items = items
            };
        }

        public PaginatedResult<ChatThreadDTO> GetClientThreads(int clientId, ChatThreadFilterDTO filters)
        {
            // Step 1: Load bookings with active job assignments
            var bookings = _unit.BookingRepo.GetAll(
                b => b.ClientId == clientId &&
                     b.IsActive &&
                     b.JobAssignment != null &&
                     (b.JobAssignment.Status == JobAssignmentsStatus.Assigned ||
                      b.JobAssignment.Status == JobAssignmentsStatus.InProgress) &&
                     b.JobAssignment.IsActive,
                includeProperties: "Client.User,JobAssignment.Handyman.User,Chats.Messages,BookingServices.Service.Category"
            );

            // Step 2: Filter by handyman name (if provided)
            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                var lower = filters.Name.ToLower();
                bookings = bookings
                    .Where(b => b.JobAssignment.Handyman.User.FirstName.ToLower().Contains(lower)
                             || b.JobAssignment.Handyman.User.LastName.ToLower().Contains(lower))
                    .ToList();
            }

            var threadDtos = new List<ChatThreadDTO>();

            foreach (var booking in bookings)
            {
                var handyman = booking.JobAssignment?.Handyman?.User;
                if (handyman == null) continue;

                var chat = booking.Chats.FirstOrDefault();
                if (chat == null)
                {
                    var chatId = EnsureChatExists(booking.Id);
                    chat = _unit.ChatRepo.GetById(chatId); // 🔄 Reload with messages
                }

                var lastMessage = chat?.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault();
                var bookingService = booking.BookingServices.FirstOrDefault();
                var serviceName = bookingService?.Service?.Name ?? "N/A";

                threadDtos.Add(new ChatThreadDTO
                {
                    ChatId = chat.Id,
                    BookingId = booking.Id,
                    ClientName = $"{booking.Client.User.FirstName} {booking.Client.User.LastName}",
                    HandymanName = $"{handyman.FirstName} {handyman.LastName}",
                    LastMessage = lastMessage?.Content,
                    LastMessageTime = lastMessage?.SentAt,
                    BookingDate = booking.PreferredDate,
                    ServiceName = serviceName
                });
            }

            // Step 3: Sort
            threadDtos = filters.Sort == "oldest"
                ? threadDtos.OrderBy(t => t.LastMessageTime ?? DateTime.MinValue).ToList()
                : threadDtos.OrderByDescending(t => t.LastMessageTime ?? DateTime.MinValue).ToList();

            // Step 4: Paginate
            var total = threadDtos.Count;
            var paginatedItems = threadDtos
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            return new PaginatedResult<ChatThreadDTO>
            {
                PageNumber = filters.PageNumber,
                PageSize = filters.PageSize,
                TotalItems = total,
                Items = paginatedItems
            };
        }




    }
}
