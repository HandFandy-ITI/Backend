using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;


namespace OstaFandy.DAL.Repos
{
    public class GeneralRepo<T> : IGeneralRepo<T> where T : class
    {
        private readonly AppDbContext _db;
        public  DbSet<T> dbSet;

        public GeneralRepo(AppDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        //ex for use var productsWithCategory = _handyman.GetAll( p => p.id > 100,"Catagory");
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? properties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (properties != null)
            {
                foreach (var prop in properties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop);
                }
            }
            return query.ToList();
        }

        public void Remove(int id)
        {
            T? entity = dbSet.Find(id);
            if (entity != null)
            {
                dbSet.Remove(entity);
            }
        }

        public void RemoveRange(IEnumerable<T> entites)
        {
            dbSet.RemoveRange(entites);

        }
        public T GetById(int id)
        {
           
            T? entity = dbSet.Find(id);
            return entity ;
        }

        public void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var prop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop);
                }
            }
            return query.FirstOrDefault();
        }
        public void Update(T entity)
        {
            dbSet.Update(entity);
        }
        public void AddRange(IEnumerable<T> entites)
        {
            dbSet.AddRange(entites);
        }
        public T? GetByIdOrDefault(int id)
        {
            return dbSet.Find(id);
        }

        //public T GetById(int id, string? includeProperties = null)
        //{
        //    IQueryable<T> query = dbSet;

        //    if (includeProperties != null)
        //    {
        //        foreach (var prop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            query = query.Include(prop);
        //        }
        //    }

        //    T? entity = query.FirstOrDefault(e => EF.Property<int>(e, "UserId") == id || EF.Property<int>(e, "Id") == id);
        //    return entity ?? throw new InvalidOperationException($"Entity of type {typeof(T).Name} with ID {id} not found.");
        //}
    }
}


