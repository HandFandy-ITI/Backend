using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    public class PaymentRepo : GeneralRepo<Payment>, IPaymentRepo
    {
        private readonly AppDbContext _context;

        public PaymentRepo(AppDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync(
            string? status = null,
            string? method = null,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Client)
                        .ThenInclude(c => c.User)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all status")
            {
                query = query.Where(p => p.Status.ToLower() == status.ToLower());
            }

            if (!string.IsNullOrEmpty(method) && method.ToLower() != "all methods")
            {
                query = query.Where(p => p.Method.ToLower() == method.ToLower());
            }

            // Apply search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>
                    (p.Booking.Client.User.FirstName + " " + p.Booking.Client.User.LastName)
                        .ToLower().Contains(searchTerm.ToLower()) ||
                    p.BookingId.ToString().Contains(searchTerm));
            }

            // Apply pagination
            query = query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(
            string? status = null,
            string? method = null,
            string? searchTerm = null)
        {
            var query = _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Client)
                        .ThenInclude(c => c.User)
                .AsQueryable();

            // Apply same filters as GetAllAsync
            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all status")
            {
                query = query.Where(p => p.Status.ToLower() == status.ToLower());
            }

            if (!string.IsNullOrEmpty(method) && method.ToLower() != "all methods")
            {
                query = query.Where(p => p.Method.ToLower() == method.ToLower());
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>
                    (p.Booking.Client.User.FirstName + " " + p.Booking.Client.User.LastName)
                        .ToLower().Contains(searchTerm.ToLower()) ||
                    p.BookingId.ToString().Contains(searchTerm));
            }

            return await query.CountAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Client)
                        .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}