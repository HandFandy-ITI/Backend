using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Identity.Client;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    internal class UserRepo : GeneralRepo<User>, IUserRepo
    {
        private readonly AppDbContext db;

        public UserRepo(AppDbContext db) : base(db) { this.db = db; }

        public bool CheckUniqueOfEmailPhone(string phone, string email)
        {
            var userExists = db.Users.FirstOrDefault(u =>
                u.Email == email ||
                u.Phone == phone);
            return userExists == null;
        }

        public bool softDelete(int id)
        {
            var x = db.Users.Find(id);
            if(x == null)
            {
                return false;
            }
            
                x.IsActive = false;
                db.Entry(x).State = EntityState.Modified;
                int affected = db.SaveChanges(); //  number of row effected
                return affected > 0;
        }
    }
}
