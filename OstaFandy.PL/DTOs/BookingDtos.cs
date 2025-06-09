namespace OstaFandy.PL.DTOs
{
    public class BookingViewDto
    {
        public int Id { get; set; }

        public string? ClientName { get; set; }

        public string? HandymanName { get; set; }

        public string? CategoryName { get; set; } 

        public List<string>? ServiceNames { get; set; } 
        public string? Note { get; set; }

        public DateTime PreferredDate { get; set; }

        public int EstimatedMinutes { get; set; }

        public string? Status { get; set; }
    }
}
