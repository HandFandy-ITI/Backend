namespace OstaFandy.PL.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }

    public class PaymentDetailsDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? Receipt { get; set; }
        public DateTime Date { get; set; }
    }

    public class PaymentFilterDto
    {
        public string? Status { get; set; }
        public string? Method { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PagedPaymentResponseDto
    {
        public IEnumerable<PaymentDto> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}