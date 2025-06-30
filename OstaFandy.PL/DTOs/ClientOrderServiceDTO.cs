namespace OstaFandy.PL.DTOs
{
    public class ClientOrderServiceDTO
    {
        public string ServiceName { get; set; }
        public string CategoryName { get; set; }
        public decimal FixedPrice { get; set; }
        public int EstimatedMinutes { get; set; }
    }
}
