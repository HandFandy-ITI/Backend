using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using Microsoft.EntityFrameworkCore;

namespace OstaFandy.DAL.Repos
{
    class ClientRepo : GeneralRepo<Client>, IClientRepo
    {
        private readonly AppDbContext _db;
        public ClientRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Client?> GetClientWithProfileData(int clientId)
        {
            return await _db.Clients
                .Include(c => c.User)
                .Include(c => c.DefaultAddress)
                .FirstOrDefaultAsync(c => c.UserId == clientId);
        }

        public async Task<Client?> GetClientWithBookingHistory(int clientId)
        {
            return await _db.Clients
                .Include(c => c.User)
                .Include(c => c.Bookings)
                    .ThenInclude(b => b.Address)
                .Include(c => c.Bookings)
                    .ThenInclude(b => b.BookingServices)
                        .ThenInclude(bs => bs.Service)
                            .ThenInclude(s => s.Category)
                .Include(c => c.Bookings)
                    .ThenInclude(b => b.JobAssignment)
                        .ThenInclude(ja => ja.Handyman)
                            .ThenInclude(h => h.User)
                .Include(c => c.Bookings)
                    .ThenInclude(b => b.JobAssignment)
                        .ThenInclude(ja => ja.Quotes)
                .Include(c => c.Bookings)
                    .ThenInclude(b => b.Payments)
                .Include(c => c.Bookings)
                    .ThenInclude(b => b.Reviews)
                .FirstOrDefaultAsync(c => c.UserId == clientId);
        }

        public async Task<Client?> GetClientByUserId(int clientId)
        {
            return await _db.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == clientId);
        }

        

        public async Task<Client?> GetById(int id)
        {
            return await _db.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }
        public Client? GetByIdSync(int id)
        {
            return _db.Clients
                .Include(c => c.User)
                .FirstOrDefault(c => c.UserId == id);
        }
        public async Task<IEnumerable<Client>> GetAll()
        {
            return await _db.Clients
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<Client> Add(Client client)
        {
            await _db.Clients.AddAsync(client);
            return client;
        }

        public void Update(Client client)
        {
            _db.Clients.Update(client);
        }

        public void Delete(Client client)
        {
            _db.Clients.Remove(client);
        }

        public IQueryable<Client> GetAll(Expression<Func<Client, bool>>? predicate = null)
        {
            var query = _db.Clients.Include(c => c.User).AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }

        public async Task<List<AvailableTimeSlotForHandyman>> GetAvailableTimeSlotsForHandymanAsync(int HandymanId, DateTime Day, int estimatedMinutes)
        {
            return await _db.AvailableTimeSlotForHandyman
                .FromSqlInterpolated($@"
                    EXEC GetHandymanFreeTime 
                        @HandymanId = {HandymanId},
                        @Day = {Day},
                        @EstimatedMinutes = {estimatedMinutes}
                ")
                .ToListAsync();

        }
    }
}