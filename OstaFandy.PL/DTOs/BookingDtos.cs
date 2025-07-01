using System.ComponentModel.DataAnnotations.Schema;

namespace OstaFandy.PL.DTOs
{
    public class BookingViewDto
    {
        public int Id { get; set; }

        public string? ClientName { get; set; }

        public string? HandymanName { get; set; }

        public string? CategoryName { get; set; }

        public List<string>? ServiceNames { get; set; }

        public decimal? TotalPrice { get; set; }
        public string? Note { get; set; }

        public DateTime PreferredDate { get; set; }

        public int EstimatedMinutes { get; set; }

        public string? Status { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }


    }

    public class AvailableTimeSlotsRequestDto
    {
        public int CategoryId { get; set; }
        public DateTime Day { get; set; }
        public decimal UserLatitude { get; set; }
        public decimal UserLongitude { get; set; }
        public int EstimatedMinutes { get; set; }
    }

    public class CreateBookingDTO
    {
        //booking part
        public int ClientId { get; set; }
        public int AddressId { get; set; }
        public DateTime PreferredDate { get; set; }
        public int EstimatedMinutes { get; set; }
        public decimal TotalPrice { get; set; }
        public string Note { get; set; }
        public List<BookingServiceDTO> serviceDto { get; set; }

        //job assign part
        public int HandymanId { get; set; }

        //applay payment 
        public decimal Amount { get; set; }

        public string Method { get; set; }

        public string PaymentStatus { get; set; }

        public string? PaymentIntentId { get; set; }

        

    }

    public class BookingServiceDTO
    {
        public int ServiceId { get; set; }
        public int Quantity { get; set; }
    }


    public class BookingChatResponse
    {
        public int BookingId { get; set; }
        public int ChatId { get; set; }
    }
}
