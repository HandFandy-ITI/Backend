namespace OstaFandy.PL.DTOs
{
    public class AdminBlockDateDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
