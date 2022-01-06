using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocNet.Infrastructure.Interfaces;

public interface IRepository
{
    public IQueryable<TEntity> Query<TEntity>() where TEntity : class;

    public TEntity GetById<TEntity>(int id) where TEntity : class;
    public Task<TEntity> GetByIdAsync<TEntity>(int id) where TEntity : class;

    public IList<TEntity> GetAll<TEntity>() where TEntity : class;
    public Task<IList<TEntity>> GetAllAsync<TEntity>() where TEntity : class;

    public TEntity Create<TEntity>(TEntity entity) where TEntity : class;
    public Task<TEntity> CreateAsync<TEntity>(TEntity entity) where TEntity : class;

    public void Update<TEntity>(TEntity entity) where TEntity : class;
    public Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;

    public void DeleteById<TEntity>(int id) where TEntity : class;
    public Task DeleteByIdAsync<TEntity>(int id) where TEntity : class;

    public Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;

    public Task DeleteManyAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

}
