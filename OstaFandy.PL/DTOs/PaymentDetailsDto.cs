namespace OstaFandy.PL.DTOs
{
    public class PaymentDetailsDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? Receipt { get; set; }
        public DateTime Date { get; set; }
    }
}