using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    class BlockDateRepo : GeneralRepo<BlockDate>, IBlockDateRepo
    {
        public BlockDateRepo(AppDbContext db) : base(db)
        {

        }
    }

}
