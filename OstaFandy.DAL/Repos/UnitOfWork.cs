using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
        }
        //add repos here
        private IUserRepo _userRepo;

        private IUserTypeRepo _userTypeRepo;
        private IHandyManRepo _handymanRepo;
        private IAddressRepo _addressRepo;
        private IBookingRepo _bookingRepo;

        private IAnalyticsRepo _analyticsRepo;

        private IBookingRepo _bookingRepo;




        // This propertyes initializes the repos if it hasn't been initialized yet
        public IUserRepo UserRepo => _userRepo??=new UserRepo(_db);
        public IUserTypeRepo UserTypeRepo => _userTypeRepo ??= new UserTypeRepo(_db);

        public IHandyManRepo HandyManRepo => _handymanRepo ??= new HandyManRepo(_db);

        public IAddressRepo AddressRepo => _addressRepo ?? new AddressRepo(_db);
        public IAnalyticsRepo AnalyticsRepo => _analyticsRepo ??= new AnalyticsRepo(_db);

        public IBookingRepo BookingRepo => _bookingRepo ??= new BookingRepo(_db);

        public IBookingRepo BookingRepo => _bookingRepo ??= new BookingRepo(_db);

        public int Save()
        {
            return _db.SaveChanges();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _db.Database.BeginTransaction();
        }


    }
}
