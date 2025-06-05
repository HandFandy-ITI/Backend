namespace OstaFandy.PL.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}