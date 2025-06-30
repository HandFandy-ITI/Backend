using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;

namespace OstaFandy.DAL.Repos.IRepos
{
    public interface IClientRepo : IGeneralRepo<Client>
    {

        Task<Client?> GetClientWithProfileData(int clientId);
        Task<Client?> GetClientWithBookingHistory(int clientId);
        Task<Client?> GetClientByUserId(int clientId);
        Task<Client?> GetById(int id);
        Task<IEnumerable<Client>> GetAll();
        Task<Client> Add(Client client);
        void Update(Client client);
        void Delete(Client client);
        IQueryable<Client> GetAll(System.Linq.Expressions.Expression<Func<Client, bool>>? predicate = null);
    }
}
