namespace OstaFandy.PL.DTOs
{
    public class ClientReviewDTO
    {
        public byte Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
