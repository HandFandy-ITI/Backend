using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace OstaFandy.DAL.Repos.IRepos
{
    public interface IUnitOfWork
    {
        public IUserRepo UserRepo { get; }
        IUserTypeRepo UserTypeRepo { get; }
        public IHandyManRepo HandyManRepo { get; }
        public IAddressRepo AddressRepo { get; }
        public IReviewRepo ReviewRepo { get; }

        public IAnalyticsRepo AnalyticsRepo { get; }

        public IBookingRepo BookingRepo { get; }

        public IBookingRepo BookingRepo { get; }




        public int Save();
        public IDbContextTransaction BeginTransaction();
    }
}
