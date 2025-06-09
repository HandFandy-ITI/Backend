using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OstaFandy.DAL.Repos.IRepos
{
    public interface IAnalyticsRepo
    {
        // groups services by category and service name, counts usage
        IQueryable<object> GetServiceUsageStats();

        // groups bookings by city and address
        IQueryable<object> GetBookingLocationStats();
    }
}
