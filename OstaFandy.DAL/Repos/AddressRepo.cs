using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    class AddressRepo : GeneralRepo<Address>, IAddressRepo
    {
        public AddressRepo(AppDbContext db) : base(db)
        {
            
        }
    }
}
