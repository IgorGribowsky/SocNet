using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Core.Interfaces
{
    public interface IRepository
    {
        public IQueryable<TEntity> Query<TEntity>() where TEntity : class;

        public TEntity GetById<TEntity>(int id) where TEntity : class;

        public IList<TEntity> GetAll<TEntity>() where TEntity : class;

        public void Create<TEntity>(TEntity entity) where TEntity : class;

        public void Update<TEntity>(TEntity entity) where TEntity : class;

        public void Delete<TEntity>(int id) where TEntity : class;
    }
}
