using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    public class ReviewRepo : GeneralRepo<Review>, IReviewRepo
    {
        private readonly AppDbContext _context;

        public ReviewRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public decimal GetAverageRating()
        {
            return _context.Reviews.Where(r => r.Rating > 0).Average(r => (decimal)r.Rating);
        }


        public async Task<bool> IsBookingExistsAsync(int bookingId)
        {
            return await _context.Set<Booking>().AnyAsync(b => b.Id == bookingId);
        }

        public async Task<bool> HasUserAlreadyReviewedAsync(int bookingId)
        {
            return await _context.Set<Review>().AnyAsync(r => r.BookingId == bookingId);
        }

        public async Task<Review> GetReviewByBookingIdAsync(int bookingId)
        {
            return await _context.Set<Review>()
                .FirstOrDefaultAsync(r => r.BookingId == bookingId);
        }


    }
}