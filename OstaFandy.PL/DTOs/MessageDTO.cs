namespace OstaFandy.PL.DTOs
{
    public class MessageDTO
    {
        public int ChatId { get; set; }
        public int? SenderId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public string? SenderName { get; set; }     // Useful for frontend display
        public string? SenderRole { get; set; }     // "Client" or "Handyman" (for role-based UI styling)

    }


    public class ChatThreadDTO
    {
        public int ChatId { get; set; }
        public int BookingId { get; set; }
        public string ClientName { get; set; }
        public string? HandymanName { get; set; } // Optional for clients
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }

        public DateTime BookingDate { get; set; }       
        public string? CategoryName { get; set; }

    }


    public class ChatThreadFilterDTO
    {
        public string? Name { get; set; }      // Client or Handyman name
        public string? Sort { get; set; }      // "newest" or "oldest"
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


}
