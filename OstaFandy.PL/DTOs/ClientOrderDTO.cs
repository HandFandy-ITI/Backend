namespace OstaFandy.PL.DTOs
{
    public class ClientOrderDTO
    {
        public int BookingId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PreferredDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public int EstimatedMinutes { get; set; }
        public string? Note { get; set; }
        public string HandymanName { get; set; }
        public ClientOrderAddressDTO Address { get; set; }
        public List<ClientOrderServiceDTO> Services { get; set; } = new List<ClientOrderServiceDTO>();
        public ClientPaymentDTO? Payment { get; set; }
        public ClientReviewDTO? Review { get; set; }
    }
}
