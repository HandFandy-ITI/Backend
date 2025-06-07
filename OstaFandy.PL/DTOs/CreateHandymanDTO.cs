namespace OstaFandy.PL.DTOs
{
    public class CreateHandymanDTO
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        public int SpecializationId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string NationalId { get; set; }
        public string NationalIdImg { get; set; }
        public string Img { get; set; }
        public int ExperienceYears { get; set; }
        public string Status { get; set; } = "Pending";

        public string? DefaultAddressPlace { get; set; }
        public string AddressType { get; set; }
        public string? DefaultAddressCity { get; set; }
        public decimal? DefaultAddressLatitude { get; set; }
        public decimal? DefaultAddressLongitude { get; set; }

    }
}
