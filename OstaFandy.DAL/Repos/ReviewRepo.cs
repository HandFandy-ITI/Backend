using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    public class ReviewRepo : GeneralRepo<Review>, IReviewRepo
    {
        private readonly AppDbContext _db;

        public ReviewRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public decimal GetAverageRating()
        {
            return _db.Reviews.Where(r => r.Rating > 0).Average(r => (decimal)r.Rating);
        }
    }
}