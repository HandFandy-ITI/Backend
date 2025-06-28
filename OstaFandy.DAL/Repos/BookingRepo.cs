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
    class BookingRepo : GeneralRepo<Booking>, IBookingRepo
    {
        private readonly AppDbContext _db;
        public BookingRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public int GetAllCompletedJobCount()
        {
            return _db.Bookings.Where(b => b.Status == "Completed").Count();
        }

        public decimal GetTotalPrice()
        {
            return _db.Bookings.Where(b => b.Status == "Completed").Sum(b => b.TotalPrice ?? 0);
        }

        //get free slot
        public async Task<List<AvailableTimeSlot>> GetAvailableTimeSlotsAsync(
           int categoryId, DateTime day, decimal userLat, decimal userLong, int estimatedMinutes)
        {
            return await _db.AvailableTimeSlots
                .FromSqlInterpolated($@"
                    EXEC GetAvailableTimeSlots 
                        @CategoryId = {categoryId},
                        @Day = {day},
                        @UserLatitude = {userLat},
                        @UserLongitude = {userLong},
                        @EstimatedMinutes = {estimatedMinutes}
                ")
                .ToListAsync();
        }
    }

}
