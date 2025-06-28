using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    class QuoteRepo : GeneralRepo<Quote>, IQuoteRepo
    {
        public AppDbContext _db{ get; set; }
        public QuoteRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public Quote GetQuoteByJobId(int jobId)
        {
            return _db.Quotes.FirstOrDefault(a => a.JobAssignmentId == jobId);
        }
    }
}
