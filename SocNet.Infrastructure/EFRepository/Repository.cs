using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocNet.Infrastructure.Interfaces;

namespace SocNet.Infrastructure.EFRepository
{
    public class Repository : IRepository
    {
        private ApplicationContext _context;

        public Repository(ApplicationContext context)
        {
            _context = context;
        }

        public TEntity Create<TEntity>(TEntity entity) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            var entityEntry = _dbSet.Add(entity);
            _context.SaveChanges();

            return entityEntry.Entity;
        }

        public async Task<TEntity> CreateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            var entityEntry = _dbSet.Add(entity);
            await _context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async Task DeleteAsync<TEntity>(TEntity item) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            _dbSet.Remove(item);
            await _context.SaveChangesAsync();
        }

        public void DeleteById<TEntity>(int id) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            var item = _dbSet.Find(id);

            _dbSet.Remove(item);
            _context.SaveChanges();
        }

        public async Task DeleteByIdAsync<TEntity>(int id) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            var item = _dbSet.Find(id);

            _dbSet.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteManyAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            foreach (var entity in entities)
            {
                _dbSet.Remove(entity);
            }
            
            await _context.SaveChangesAsync();
        }

        public IList<TEntity> GetAll<TEntity>() where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            return _dbSet.ToList();
        }

        public async Task<IList<TEntity>> GetAllAsync<TEntity>() where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            return await _dbSet.ToListAsync();
        }

        public TEntity GetById<TEntity>(int id) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            return _dbSet.Find(id);
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(int id) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            return await _dbSet.FindAsync(id);
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

        public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            var _dbSet = _context.Set<TEntity>();

            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
