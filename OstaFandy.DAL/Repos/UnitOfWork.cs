using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.Repos;
using OstaFandy.PL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
        }
        #region d
        //add repos here
        private IUserRepo _userRepo;

        private IUserTypeRepo _userTypeRepo;
        private IPaymentRepo _paymentRepo;

        private IHandyManRepo _handymanRepo;
        private IAddressRepo _addressRepo;
        private IBookingRepo _bookingRepo;
        private IReviewRepo _reviewRepo;
        private IAnalyticsRepo _analyticsRepo;
        private IJobAssignmentRepo _JobAssignmentRepo;

        private IClientRepo _ClientRepo;
        private ICategoryRepo _categoryRepo;
        private IServiceRepo _serviceRepo;
        private INotificationRepo _notificationRepo;
        private IQuoteRepo _quoteRepo;
        private IBlockDateRepo _blockDataRepo;
 

        private IChatRepo _chatRepo;
        private IMessageRepo _messageRepo;

        private IBookingServiceRepo _bookingServiceRepo;
        
        private IPasswordResetTokenRepo _passwordResetTokenRepo;    



        // This propertyes initializes the repos if it hasn't been initialized yet
        public IUserRepo UserRepo => _userRepo ??= new UserRepo(_db);
        public IUserTypeRepo UserTypeRepo => _userTypeRepo ??= new UserTypeRepo(_db);
        public IPaymentRepo PaymentRepo => _paymentRepo ??= new PaymentRepo(_db);
        public INotificationRepo NotificationRepo=> _notificationRepo ??= new NotificationRepo(_db);
        public IQuoteRepo QuoteRepo=> _quoteRepo ??= new QuoteRepo(_db);
        public IBlockDateRepo BlockDateRepo=> _blockDataRepo ??= new BlockDateRepo(_db);


        public IHandyManRepo HandyManRepo => _handymanRepo ??= new HandyManRepo(_db);

        public IAddressRepo AddressRepo => _addressRepo ?? new AddressRepo(_db);
        public IAnalyticsRepo AnalyticsRepo => _analyticsRepo ??= new AnalyticsRepo(_db);
        public IReviewRepo ReviewRepo => _reviewRepo ??= new ReviewRepo(_db);
        public IBookingRepo BookingRepo => _bookingRepo ??= new BookingRepo(_db);
        public IJobAssignmentRepo JobAssignmentRepo => _JobAssignmentRepo ??= new JobAssignmentRepo(_db);


        public ICategoryRepo CategoryRepo => _categoryRepo ??= new CategoryRepo(_db);
        public IServiceRepo ServiceRepo => _serviceRepo ??= new ServiceRepo(_db);

        public IChatRepo ChatRepo => _chatRepo ??= new ChatRepo(_db);
        public IMessageRepo MessageRepo => _messageRepo ??= new MessageRepo(_db);

        public IClientRepo ClientRepo => _ClientRepo ??= new ClientRepo(_db);

        public IBookingServiceRepo BookingServiceRepo => _bookingServiceRepo ??= new BookingServiceRepo(_db);

        public IPasswordResetTokenRepo PasswordResetTokenRepo=> _passwordResetTokenRepo ??= new PasswordResetTokenRepo(_db);



        public int Save()
        {
            return _db.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _db.Database.BeginTransaction();
        }

        public async Task<IDbContextTransaction> BeginTransactionasync()
        {
            return await _db.Database.BeginTransactionAsync();
        }
        #endregion
   
    }
}
