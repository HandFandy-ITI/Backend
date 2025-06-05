using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

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
        public async Task<ActionResult<PagedPaymentResponseDto>> GetAllPayments([FromQuery] PaymentFilterDto filter)
        {
            try
            {
                var result = await _paymentService.GetAllPaymentsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDetailsDto>> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);

                if (payment == null)
                    return NotFound(new { message = "Payment not found" });

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}