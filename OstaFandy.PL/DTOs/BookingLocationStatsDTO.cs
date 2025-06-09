namespace OstaFandy.PL.DTOs
{
    // location based booking statistics
    public class BookingLocationStatsDTO
    {
        public string City { get; set; }
        public int BookingCount { get; set; }
        public List<AddressBookingStatsDTO> Addresses { get; set; } = new List<AddressBookingStatsDTO>();
    }

    // address details with booking count
    public class AddressBookingStatsDTO
    {
        public string Address { get; set; }
        public int BookingCount { get; set; }
    }
}
