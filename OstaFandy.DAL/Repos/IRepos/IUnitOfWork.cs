using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.DAL.Repos;
using OstaFandy.PL.Repos.IRepos;

namespace OstaFandy.DAL.Repos.IRepos
{
    public interface IUnitOfWork
    {
        public IUserRepo UserRepo { get; }
        public IUserTypeRepo UserTypeRepo { get; }
        public IPaymentRepo PaymentRepo { get; }
        public INotificationRepo NotificationRepo { get; }
        public IHandyManRepo HandyManRepo { get; }
        public IClientRepo ClientRepo { get; }
        public IAddressRepo AddressRepo { get; }
        public IAnalyticsRepo AnalyticsRepo { get; }
        public IQuoteRepo QuoteRepo{ get; }
        public IBlockDateRepo BlockDateRepo{ get; }
        public IBookingRepo BookingRepo { get; }

        //public IReviewRepo ReviewRepo { get; }

        public IReviewRepo ReviewRepo { get; }

        public IJobAssignmentRepo JobAssignmentRepo { get; }
        public ICategoryRepo CategoryRepo { get; }
        public IServiceRepo ServiceRepo { get; }
        public IChatRepo ChatRepo { get; }
        public IMessageRepo MessageRepo { get; }
        public IBookingServiceRepo BookingServiceRepo { get; } 
        public IPasswordResetTokenRepo PasswordResetTokenRepo { get; }

        public int Save();
        public Task<int> SaveAsync();
        public IDbContextTransaction BeginTransaction();
        Task<IDbContextTransaction> BeginTransactionasync();
    }
}
