using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    public class JobAssignmentRepo : GeneralRepo<JobAssignment>, IJobAssignmentRepo
    {
        private readonly AppDbContext _db;
        public JobAssignmentRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public int gethandymanbyjobid(int id)
        {
            int handymanid = _db.JobAssignments.Where(a => a.Id == id).Select(a => a.HandymanId).FirstOrDefault();
            return handymanid;
        }
        public int GetJobIdByNotificationId(int notificationId)
        {
            return (int) _db.Notifications
               .Where(n => n.Id == notificationId)
               .Select(n => n.RelatedEntityId)
               .FirstOrDefault();
        }
    }
}
