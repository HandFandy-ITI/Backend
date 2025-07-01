using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using Microsoft.EntityFrameworkCore;

namespace OstaFandy.DAL.Repos
{
    class ClientRepo : GeneralRepo<Client>, IClientRepo
    {
        private readonly AppDbContext _db;
        public ClientRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<List<AvailableTimeSlotForHandyman>> GetAvailableTimeSlotsForHandymanAsync(int HandymanId, DateTime Day, int estimatedMinutes)
        {
            return await _db.AvailableTimeSlotForHandyman
                .FromSqlInterpolated($@"
                    EXEC GetHandymanFreeTime 
                        @HandymanId = {HandymanId},
                        @Day = {Day},
                        @EstimatedMinutes = {estimatedMinutes}
                ")
                .ToListAsync();
        }
    }
}
