using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    class ClientRepo : GeneralRepo<Client>, IClientRepo
    {
        public ClientRepo(AppDbContext db) : base(db)
        {

        }
    }
}
