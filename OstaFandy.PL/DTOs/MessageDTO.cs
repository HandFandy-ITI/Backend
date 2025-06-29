namespace OstaFandy.PL.DTOs
{
    public class MessageDTO
    {
        public int ChatId { get; set; }
        public int? SenderId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }  

    }
}
