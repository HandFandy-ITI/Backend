using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;


namespace OstaFandy.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedPaymentResponseDto>>> GetAllPayments([FromQuery] PaymentFilterDto filter)
        {
            try
            {
                var result = await _paymentService.GetAllPaymentsAsync(filter);
                return Ok(ApiResponse<PagedPaymentResponseDto>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PagedPaymentResponseDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PaymentDetailsDto>>> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);

                if (payment == null)
                    return NotFound(ApiResponse<PaymentDetailsDto>.ErrorResult("Payment not found"));

                return Ok(ApiResponse<PaymentDetailsDto>.SuccessResult(payment));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PaymentDetailsDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentIntentRequest request)
        {
            var ClientSecret = await _paymentService.CreatePaymentIntent(request.Amount);
            return Ok(new { clientSecret = ClientSecret });
        }
    }
}