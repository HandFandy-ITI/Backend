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
            // Check if booking exists
            var bookingExists = await unitOfWork.ReviewRepo.IsBookingExistsAsync(createReviewDTO.BookingId);
            if (!bookingExists)
            {
                throw new ArgumentException("Booking not found");
            }

            // Check if user already reviewed this booking
            var hasAlreadyReviewed = await unitOfWork.ReviewRepo.HasUserAlreadyReviewedAsync(createReviewDTO.BookingId);
            if (hasAlreadyReviewed)
            {
                throw new InvalidOperationException("You have already reviewed this booking");
            }

            // Create new review
            var review = new Review
            {
                BookingId = createReviewDTO.BookingId,
                Rating = createReviewDTO.Rating,
                Comment = createReviewDTO.Comment ?? string.Empty,
                CreatedAt = DateTime.Now
            };

            // Save to database using Unit of Work
            unitOfWork.ReviewRepo.Insert(review);
            await unitOfWork.SaveAsync(); 

            // Map to response DTO
            return mapper.Map<ReviewResponseDTO>(review);
        }
    }




}