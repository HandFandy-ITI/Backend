using AutoMapper;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.DAL.Repos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.DAL.Entities;
using Stripe;

namespace OstaFandy.PL.BL
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<PagedPaymentResponseDto> GetAllPaymentsAsync(PaymentFilterDto filter)
        {
            var payments = await _unitOfWork.PaymentRepo.GetAllAsync(
                filter.Status,
                filter.Method,
                filter.SearchTerm,
                filter.PageNumber,
                filter.PageSize);

            var totalCount = await _unitOfWork.PaymentRepo.GetTotalCountAsync(
                filter.Status,
                filter.Method,
                filter.SearchTerm);

            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);

            return new PagedPaymentResponseDto
            {
                Data = paymentDtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                HasNextPage = filter.PageNumber < totalPages,
                HasPreviousPage = filter.PageNumber > 1
            };
        }

        public async Task<PaymentDetailsDto?> GetPaymentByIdAsync(int id)
        {
            var payment = await _unitOfWork.PaymentRepo.GetByIdAsync(id);

            if (payment == null)
                return null;

            return _mapper.Map<PaymentDetailsDto>(payment);
        }

        public async Task<string> CreatePaymentIntent(decimal amount)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe uses cents
                Currency = "egp",
                PaymentMethodTypes = new List<string> { "card" },
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return intent.ClientSecret;
        }
    }
}