using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    internal class UserRepo : GeneralRepo<User>, IUserRepo
    {
        private readonly AppDbContext _db;
        public UserRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public bool CheckUniqueOfEmailPhone(string email, string phone)
        {
            return !_db.Users.Any(u => u.Email == email || u.Phone == phone);
        }



        public int GetCountOfActiveClient()
        {
            return _db.Clients.Count(c => c.User.IsActive == true);
        }

        public bool SoftDelete(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
            {
                return false;
            }

            user.IsActive = false;
            _db.SaveChanges();
            return true;
        }

        public async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate, string includeProperties = "")
        {
            IQueryable<User> query = _db.Users;

            // Apply includes
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Users
                .Include(u => u.Client)
                .Include(u => u.Handyman)
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == id);
        }   
    }
}
