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
        IPaymentRepo PaymentRepo { get; }
        public INotificationRepo NotificationRepo { get; }
        public IHandyManRepo HandyManRepo { get; }
        public IClientRepo ClientRepo { get; }
        public IAddressRepo AddressRepo { get; }
        public IAnalyticsRepo AnalyticsRepo { get; }
        public IQuoteRepo QuoteRepo{ get; }
        public IBlockDateRepo BlockDateRepo{ get; }

        public IBookingRepo BookingRepo { get; }

        public IReviewRepo ReviewRepo { get; }
        public IJobAssignmentRepo JobAssignmentRepo { get; }
        ICategoryRepo CategoryRepo { get; }
        IServiceRepo ServiceRepo { get; }
        IChatRepo ChatRepo { get; }
        IMessageRepo MessageRepo { get; }

        IBookingServiceRepo BookingServiceRepo { get; } 
        public int Save();

        public Task<int> SaveAsync();
        public IDbContextTransaction BeginTransaction();


        Task<IDbContextTransaction> BeginTransactionasync();
    }
}
