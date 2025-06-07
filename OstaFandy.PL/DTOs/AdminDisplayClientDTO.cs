using OstaFandy.DAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OstaFandy.PL.DTOs
{
    public class AdminDisplayClientDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Client Information
        public int ClientUserId { get; set; }
        public int? DefaultAddressId { get; set; }

        // Default Address Information 
        public AddressDTO DefaultAddress { get; set; }

        // All Addresses
        public List<AddressDTO> Addresses { get; set; } = new List<AddressDTO>();

        //   Statistics
        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
