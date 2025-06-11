namespace OstaFandy.PL.DTOs
{
    public class OrderFeedbackDto
    {
        public int BookingId { get; set; }
        public string HandymanName { get; set; }
        public string HandymanSpecialty { get; set; }
        public string ClientName { get; set; }
        public string ServiceName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime ReviewCreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
    }

    //public class FeedbackStatisticsDto
    //{
    //    public int TotalReviews { get; set; }
    //    public double AverageRating { get; set; }
    //    public List<RatingDistributionDto> RatingDistribution { get; set; }
    //}

    //public class RatingDistributionDto
    //{
    //    public int Rating { get; set; }
    //    public int Count { get; set; }
//}
}
