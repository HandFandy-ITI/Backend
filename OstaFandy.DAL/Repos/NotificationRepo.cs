using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    public class NotificationRepo : GeneralRepo<Notification>, INotificationRepo
    {
        private readonly AppDbContext _db;
        public NotificationRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public Notification GetByUserId(int userId)
        {
            return _db.Notifications.FirstOrDefault(n => n.UserId == userId);
        }
    }
    
}
