using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;

namespace OstaFandy.DAL.Repos.IRepos
{
    public interface IBookingRepo:IGeneralRepo<Booking>
    {
        public int GetAllCompletedJobCount();
        public decimal GetTotalPrice();

         Task<List<AvailableTimeSlot>> GetAvailableTimeSlotsAsync(
           int categoryId, DateTime day, decimal userLat, decimal userLong, int estimatedMinutes);
    }
}
