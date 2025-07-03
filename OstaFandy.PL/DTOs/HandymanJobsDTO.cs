namespace OstaFandy.PL.DTOs
{

    public class HandymanJobsDTO
    {
        public int JobAssignmentId { get; set; }
        public string ClientName { get; set; }
        public String ClientNumber { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
    }
    public class AddQuoteDTO
    {
        public int JobId { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public int EstimatedMinutes { get; set; }

    }
    public class AllQuotes
    {
        public int JobAssignmentId { get; set; }
        public decimal price { get; set; }
        public int? estimatedMinutes { get; set; }
        public string notes { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }
    public class QuoteDetailsDTO
    {
        public int QuoteId { get; set; }
        public int JobId { get; set; }
        public int HandymanId { get; set; }
        public string HandymanName { get; set; }
        public decimal Price { get; set; }
        public int EstimatedMinutes { get; set; }
        public string Notes { get; set; }
        public int OriginalBookingId { get; set; }
        public int ClientId { get; set; }
        public int AddressId { get; set; }
        public DateTime OriginalPreferredDate { get; set; }
        public List<BookingServiceDTO> OriginalServices { get; set; } = new List<BookingServiceDTO>();
    }
    public class QuoteResponseDTO
    {
        public int QuoteId { get; set; }
        public string Action { get; set; }  
        public int ClientUserId { get; set; }
        public CreateBookingDTO BookingData { get; set; }  
    }
}
