//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using OstaFandy.DAL.Entities;
//using OstaFandy.DAL.Repos.IRepos;
//using Microsoft.EntityFrameworkCore;
//using OstaFandy.DAL.Repos;
//using OstaFandy.PL.Repos.IRepos;


//namespace OstaFandy.DAL.Repos
//{
//    public class ReviewRepo : GeneralRepo<Review>, IReviewRepo
//    {
//        private readonly AppDbContext _db;

//        public ReviewRepo(AppDbContext db) : base(db)
//        {
//            _db = db;
//        }

//        public decimal GetAverageRating()
//        {
//            return _db.Reviews.Where(r => r.Rating > 0).Average(r => (decimal)r.Rating);
//        }

//        //client side 
//        public async Task<bool> IsBookingExistsAsync(int bookingId)
//        {
//            return await _db.Set<Booking>().AnyAsync(b => b.Id == bookingId);
//        }

//        public async Task<bool> HasUserAlreadyReviewedAsync(int bookingId)
//        {
//            return await _db.Set<Review>().AnyAsync(r => r.BookingId == bookingId);
//        }

//        public async Task<Review> GetReviewByBookingIdAsync(int bookingId)
//        {
//            return await _db.Set<Review>()
//                .FirstOrDefaultAsync(r => r.BookingId == bookingId);
//        }

//    }
//}


using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos;
using OstaFandy.PL.Repos.IRepos;

namespace OstaFandy.PL.Repos
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


        public async Task<int?> GetBookingIdByJobAssignmentIdAsync(int jobAssignmentId)
        {
            var jobAssignment = await _context.Set<JobAssignment>()
                .FirstOrDefaultAsync(ja => ja.Id == jobAssignmentId);

            return jobAssignment?.BookingId;
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