namespace OstaFandy.PL.DTOs
{
    public class ClientDefaultAddressDTO
    {
        public int? Id { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? AddressType { get; set; }
    }
}
