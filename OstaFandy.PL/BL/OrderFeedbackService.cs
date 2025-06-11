using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.BL
{
    public class OrderFeedbackService : IOrderFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderFeedbackService> _logger;

        public OrderFeedbackService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderFeedbackService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public PaginationHelper<OrderFeedbackDto> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                var reviews = _unitOfWork.ReviewRepo.GetAll(includeProperties: "Booking.JobAssignment.Handyman.User,Booking.JobAssignment.Handyman.Specialization,Booking.Client.User").ToList();

                if (reviews == null)
                {
                    return new PaginationHelper<OrderFeedbackDto>
                    {
                        Data = new List<OrderFeedbackDto>(),
                        CurrentPage = pageNumber,
                        TotalPages = 0,
                        TotalCount = 0,
                        SearchString = searchString
                    };
                }
                var orderFeedbackDtos = _mapper.Map<List<OrderFeedbackDto>>(reviews);

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchString))
                {
                    reviews = reviews.Where(h =>
                        h.Booking.JobAssignment.Handyman.User.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        h.Booking.JobAssignment.Handyman.User.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        h.Booking.JobAssignment.Handyman.Specialization.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    //h.User.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    //h.User.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    //h.Specialization.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }
                return PaginationHelper<OrderFeedbackDto>.Create(orderFeedbackDtos, pageNumber, pageSize, searchString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all handymen");
                return new PaginationHelper<OrderFeedbackDto>
                {
                    Data = new List<OrderFeedbackDto>(),
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0,
                    SearchString = searchString
                };
            }
        }
    }
}