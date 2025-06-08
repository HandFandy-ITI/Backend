using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    public class ServiceRepo : GeneralRepo<Service>, IServiceRepo
    {
        private readonly AppDbContext _context;

        public ServiceRepo(AppDbContext db) : base(db)
        {
            _context = db;
        }

        public async Task<IEnumerable<Service>> GetActiveAsync()
        {
            return await _context.Services
                .Include(s => s.Category)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Services
                .Where(s => s.CategoryId == categoryId && s.IsActive)
                .ToListAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return false;

            service.IsActive = false;
            service.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
