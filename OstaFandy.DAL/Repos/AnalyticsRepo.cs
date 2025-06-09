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
    public class AnalyticsRepo: IAnalyticsRepo
    {
        private readonly AppDbContext _db;

        public AnalyticsRepo(AppDbContext db)
        {
            _db = db;
        }

        public IQueryable<object> GetServiceUsageStats()
        {
            try
            {
                var serviceUsageStats = _db.BookingServices
                    .Include(bs => bs.Service)
                        .ThenInclude(s => s.Category)
                    .GroupBy(bs => new { bs.Service.Id, bs.Service.Name, Category_Name = bs.Service.Category.Name })
                    .Select(g => new
                    {
                        ServiceId = g.Key.Id,
                        ServiceName = g.Key.Name,
                        CategoryName = g.Key.Category_Name, 
                        UsageCount = g.Count()
                    })
                    .OrderByDescending(s => s.UsageCount);

                return serviceUsageStats;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting service usage stats: {ex.Message}", ex);
            }
        }

        public IQueryable<object> GetBookingLocationStats()
        {
            try
            {
                var locationStats = _db.Bookings 
                    .Include(b => b.Address)
                    .Where(b => b.IsActive == true)
                    .GroupBy(b => b.Address.City)
                    .Select(g => new
                    {
                        City = g.Key,
                        BookingCount = g.Count(),
                        Addresses = g.GroupBy(b => b.Address.Address1)
                                   .Select(ag => new
                                   {
                                       Address = ag.Key,
                                       BookingCount = ag.Count()
                                   })
                                   .OrderByDescending(a => a.BookingCount)
                                   .ToList()
                    })
                    .OrderByDescending(l => l.BookingCount);

                return locationStats;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting booking location stats: {ex.Message}", ex);
            }
        }
    }
}
