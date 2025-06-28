using OstaFandy.PL.DTOs;
using Stripe;

namespace OstaFandy.PL.BL.IBL
{
    public interface IPaymentService
    {
        Task<PagedPaymentResponseDto> GetAllPaymentsAsync(PaymentFilterDto filter);
        Task<PaymentDetailsDto?> GetPaymentByIdAsync(int id);

        Task<string> CreatePaymentIntent(decimal amount);
    }
}