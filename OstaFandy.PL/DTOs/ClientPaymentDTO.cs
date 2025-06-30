namespace OstaFandy.PL.DTOs
{
    public class ClientPaymentDTO
    {
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string? Status { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
