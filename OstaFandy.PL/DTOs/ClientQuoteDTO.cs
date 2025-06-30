namespace OstaFandy.PL.DTOs
{
    public class ClientQuoteDTO
    {
        public int QuoteId { get; set; }
        public int BookingId { get; set; }
        public string HandymanName { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime BookingDate { get; set; }
        public List<string> Services { get; set; } = new List<string>();
        public string CategoryName { get; set; }
    }
}
