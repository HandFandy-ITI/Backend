namespace OstaFandy.PL.DTOs
{
    public class EmailDto
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSSl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string From { get; set; }

    }

    public class EmailContentDto
    {
        public string to { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}
