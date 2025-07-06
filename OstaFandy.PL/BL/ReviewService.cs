using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.Repos.IRepos;

namespace OstaFandy.PL.BL
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ReviewResponseDTO> CreateReviewAsync(CreateReviewDTO createReviewDTO)
        {
            // Get BookingId from JobAssignment
            var bookingId = await unitOfWork.ReviewRepo.GetBookingIdByJobAssignmentIdAsync(createReviewDTO.JobAssignmentId);
            if (bookingId == null)
            {
                throw new ArgumentException("Job assignment not found");
            }

            // Check if user already reviewed this booking
            var hasAlreadyReviewed = await unitOfWork.ReviewRepo.HasUserAlreadyReviewedAsync(bookingId.Value);
            if (hasAlreadyReviewed)
            {
                throw new InvalidOperationException("You have already reviewed this booking");
            }

            // Create new review
            var review = new Review
            {
                BookingId = bookingId.Value,
                Rating = createReviewDTO.Rating,
                Comment = createReviewDTO.Comment ?? string.Empty,
                CreatedAt = DateTime.Now
            };

            // Save to database using Unit of Work
            unitOfWork.ReviewRepo.Insert(review);
            await unitOfWork.SaveAsync(); // استخدام Unit of Work للحفظ

            // Map to response DTO
            return mapper.Map<ReviewResponseDTO>(review);
        }
    
}
}