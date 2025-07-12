namespace OstaFandy.PL.DTOs
{
    public class AddressDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string AddressType { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAddressDTO
    {
        public int UserId { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string AddressType { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DeleteAddressDTO
    {
        public int UserId { get; set; }
    }

    public class SetDefaultAddressDTO
    {
        public int UserId { get; set; }
    }
}
