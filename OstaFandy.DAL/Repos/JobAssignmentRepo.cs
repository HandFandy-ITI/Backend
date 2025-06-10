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
    }
}
