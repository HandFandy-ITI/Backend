using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;

namespace OstaFandy.DAL.Repos.IRepos
{
    public interface IUserRepo : IGeneralRepo<User>
    {
        bool CheckUniqueOfEmailPhone(string email, string phone);
        bool SoftDelete(int id);

        int GetCountOfActiveClient();

        Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate, string includeProperties = "");
        Task<User> GetByIdAsync(int clientId);
    }
}