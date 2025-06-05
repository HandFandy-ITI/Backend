using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IPaymentService
    {
        Task<PagedPaymentResponseDto> GetAllPaymentsAsync(PaymentFilterDto filter);
        Task<PaymentDetailsDto?> GetPaymentByIdAsync(int id);
    }
}