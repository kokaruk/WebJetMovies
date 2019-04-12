using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebJetMoviesAPI.Core.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(string methodUrl);
        Task<TEntity> GetAsync(string methodUrl, string id);
        Task<TEntity> FindAsync(string methodUrl, Func<TEntity, bool> filter);
    }
}