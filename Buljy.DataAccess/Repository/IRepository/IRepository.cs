using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();

        Task<T> Get(Expression<Func<T, bool>> filter);

        Task Add(T entity);

        Task Delete(T entity);

        Task DeleteRange(T entity);
    }
}
