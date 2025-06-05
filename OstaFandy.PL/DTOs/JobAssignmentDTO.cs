namespace OstaFandy.PL.DTOs
{
    public class JobAssignmentDTO
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int HandymanId { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
