using System.ComponentModel.DataAnnotations;

namespace OstaFandy.PL.DTOs
{
    public class UpdateClientDefaultAddressDTO
    {
        [Required(ErrorMessage = "Address ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Address ID must be greater than 0")]
        public int AddressId { get; set; }
    }
}
