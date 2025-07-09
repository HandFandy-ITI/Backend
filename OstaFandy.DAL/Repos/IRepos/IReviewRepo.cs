//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using OstaFandy.DAL.Entities;
//using OstaFandy.DAL.Repos.IRepos;

//namespace OstaFandy.DAL.Repos.IRepos
//{
//    public interface IReviewRepo : IGeneralRepo<Review>
//    {
//        public decimal GetAverageRating();
//        void Insert(Review review);
//        Task<bool> IsBookingExistsAsync(int bookingId);
//        Task<bool> HasUserAlreadyReviewedAsync(int bookingId);
//        Task<Review> GetReviewByBookingIdAsync(int bookingId);

//    }
//}

using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.PL.Repos.IRepos
{
    public interface IReviewRepo : IGeneralRepo<Review>
    {
        public decimal GetAverageRating();

        Task<bool> IsBookingExistsAsync(int bookingId);
        Task<bool> HasUserAlreadyReviewedAsync(int bookingId);
        Task<Review> GetReviewByBookingIdAsync(int bookingId);
    }
}