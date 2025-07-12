using System.ComponentModel.DataAnnotations;

namespace OstaFandy.PL.DTOs
{
    public class ClientProfileDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ClientDefaultAddressDTO? DefaultAddress { get; set; }
    
    }

    public class ClientDefaultAddressDTO
    {
        public int? Id { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? AddressType { get; set; }
    }

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

    public class ClientOrderAddressDTO
    {
        public string FullAddress { get; set; }
        public string City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class ClientOrderServiceDTO
    {
        public string ServiceName { get; set; }
        public string CategoryName { get; set; }
        public decimal FixedPrice { get; set; }
        public int EstimatedMinutes { get; set; }
    }

    public class ClientPaymentDTO
    {
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string? Status { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class ClientReviewDTO
    {
        public byte Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }

    public class ClientQuoteDTO
    {
        public int QuoteId { get; set; }
        public int BookingId { get; set; }
        public string HandymanName { get; set; }
        public int handymanId { get; set; }
        public int addressId { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime BookingDate { get; set; }
        public List<string> Services { get; set; } = new List<string>();
        public string CategoryName { get; set; }
    }

    public class ClientAddressDTO
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string AddressType { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateClientProfileDTO
    {
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }

        // Address-related properties
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address1 { get; set; }

        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string? City { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }

    }

    public class UpdateClientAddressDTO
    {
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address1 { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }

        [StringLength(50, ErrorMessage = "Address type cannot exceed 50 characters")]
        public string? AddressType { get; set; }
    }




}
