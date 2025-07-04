using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    class HandyManRepo : GeneralRepo<Handyman>, IHandyManRepo
    {
        private readonly AppDbContext _db;
        public HandyManRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public bool checkUniqueNationalId(string nationalid)
        {
            return _db.Handymen.FirstOrDefault(a => a.NationalId == nationalid) == null;
        }

        public Handyman GetHandymanByJobId(int JobId)
        {
            return _db.JobAssignments
                .Include(ja => ja.Handyman)
                .Where(ja => ja.Id == JobId)
                .Select(ja => ja.Handyman)
                .FirstOrDefault();
        }

        public Handyman GetHandymanByNotificationId(int NotificationId)
        {
            return _db.Notifications
                .Where(n => n.Id == NotificationId )
                .Join(_db.JobAssignments,
                    n => n.RelatedEntityId,
                    ja => ja.Id,
                    (n, ja) => ja.Handyman)
                .FirstOrDefault();
        }

    }
}
