using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;

namespace OstaFandy.DAL.Repos.IRepos
{
    public interface IUserRepo : IGeneralRepo<User>
    {
        public bool CheckUniqueOfEmailPhone(string phone, string email);
        public bool softDelete(int id);
    }
}
