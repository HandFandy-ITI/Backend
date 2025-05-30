using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    internal class UserRepo : GeneralRepo<User>,IUserRepo
    {
        public UserRepo(AppDbContext db) : base(db)
        {
        }
    }
}
