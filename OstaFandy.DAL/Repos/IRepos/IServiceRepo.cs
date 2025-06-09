using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;

namespace OstaFandy.DAL.Repos.IRepos
{
    public interface IServiceRepo : IGeneralRepo<Service>
    {
        //Task<IEnumerable<Service>> GetByCategoryIdAsync(int categoryId);
        //Task<IEnumerable<Service>> GetActiveAsync();
        //Task<bool> SoftDeleteAsync(int id);
    }
}
