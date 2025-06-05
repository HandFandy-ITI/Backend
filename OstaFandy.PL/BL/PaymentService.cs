using AutoMapper;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.DAL.Repos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepo _paymentRepo;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepo paymentRepo, IMapper mapper)
        {
            _paymentRepo = paymentRepo;
            _mapper = mapper;
        }

        public async Task<PagedPaymentResponseDto> GetAllPaymentsAsync(PaymentFilterDto filter)
        {
            var payments = await _paymentRepo.GetAllAsync(
                filter.Status,
                filter.Method,
                filter.SearchTerm,
                filter.PageNumber,
                filter.PageSize);

            var totalCount = await _paymentRepo.GetTotalCountAsync(
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
            var payment = await _paymentRepo.GetByIdAsync(id);

            if (payment == null)
                return null;

            return _mapper.Map<PaymentDetailsDto>(payment);
        }
    }
}