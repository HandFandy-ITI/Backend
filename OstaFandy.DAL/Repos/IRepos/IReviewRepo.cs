using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos.IRepos
{
    public interface IReviewRepo : IGeneralRepo<Review>
    {
        public decimal GetAverageRating();

        Task<bool> IsBookingExistsAsync(int bookingId);
        Task<bool> HasUserAlreadyReviewedAsync(int bookingId);
        Task<Review> GetReviewByBookingIdAsync(int bookingId);
    }
}