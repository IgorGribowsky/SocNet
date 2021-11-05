using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocNet.Core.Interfaces;

namespace SocNet.Core.EFRepository
{
    public class Repository : IRepository
    {
        private ApplicationContext _context;

        public Repository(ApplicationContext context)
        {
            _context = context;
        }

        public void Create<TEntity>(TEntity entity) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Delete<TEntity>(int id) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            var item = _dbSet.Find(id);

            _dbSet.Remove(item);
            _context.SaveChanges();
        }

        public IList<TEntity> GetAll<TEntity>() where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            return _dbSet.ToList();
        }

        public TEntity GetById<TEntity>(int id) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            return _dbSet.Find(id);
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            return _dbSet.AsNoTracking();
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
