using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
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
        public async Task<(List<AvailableTimeSlot> Slots, bool AreaNotSupported)> GetAvailableTimeSlotsAsync(
   int categoryId, DateTime day, decimal userLat, decimal userLong, int estimatedMinutes)
        {
            using var cmd = _db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "GetAvailableTimeSlots";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@CategoryId", categoryId));
            cmd.Parameters.Add(new SqlParameter("@Day", day));
            cmd.Parameters.Add(new SqlParameter("@UserLatitude", userLat));
            cmd.Parameters.Add(new SqlParameter("@UserLongitude", userLong));
            cmd.Parameters.Add(new SqlParameter("@EstimatedMinutes", estimatedMinutes));

            var areaNotSupportedParam = new SqlParameter("@AreaNotSupported", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(areaNotSupportedParam);

            await _db.Database.OpenConnectionAsync();

            var slots = new List<AvailableTimeSlot>();

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                slots.Add(new AvailableTimeSlot
                {
                    UserId = reader.GetInt32(0),
                    StartTime = reader.GetDateTime(1),
                    EndTime = reader.GetDateTime(2),
                    SlotLength = reader.GetInt32(3)
                });
            }

            await reader.CloseAsync();
            await _db.Database.CloseConnectionAsync();

            bool areaNotSupported = (bool)areaNotSupportedParam.Value;

            return (slots, areaNotSupported);
        }
    }

}
