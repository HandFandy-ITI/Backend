namespace OstaFandy.PL.DTOs
{
    public class ClientOrderHistoryDTO
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public List<ClientOrderDTO> Orders { get; set; } = new List<ClientOrderDTO>();
    }
}
