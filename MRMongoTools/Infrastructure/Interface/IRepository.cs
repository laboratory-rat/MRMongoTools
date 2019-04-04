using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MRMongoTools.Infrastructure.Interface
{
    public interface IRepository<T>
        where T : class, IEntity, new()
    {
        #region create

        Task<T> Insert(T entity);
        Task<IEnumerable<T>> Insert(IEnumerable<T> entities);

        #endregion

        #region get

        Task<T> Get(string id);
        Task<IEnumerable<T>> Get(IEnumerable<string> ids);
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> search);
        Task<T> Get<F>(Expression<Func<T, F>> field, F value);

        Task<IEnumerable<T>> GetSorted(Expression<Func<T, object>> sort, bool desc);
        Task<IEnumerable<T>> GetSorted(Expression<Func<T, object>> sort, bool desc, int skip, int limit);
        Task<IEnumerable<T>> GetSorted(Expression<Func<T, bool>> search, Expression<Func<T, object>> sort, bool desc);
        Task<IEnumerable<T>> GetSorted(Expression<Func<T, bool>> search, Expression<Func<T, object>> sort, bool desc, int skip, int limit);

        Task<IEnumerable<T>> GetIn<F>(Expression<Func<T, F>> field, IEnumerable<F> values);
        Task<T> GetInFirst<F>(Expression<Func<T, F>> field, IEnumerable<F> values);
        Task<IEnumerable<T>> GetInSorted<F>(Expression<Func<T, F>> field, IEnumerable<F> values, Expression<Func<T, object>> sort, bool desc);
        Task<IEnumerable<T>> GetInSorted<F>(Expression<Func<T, F>> field, IEnumerable<F> values, Expression<Func<T, object>> sort, bool desc, int skip, int limit);

        Task<T> GetFirst(Expression<Func<T, bool>> search);
        Task<T> GetFirstSorted(Expression<Func<T, bool>> search, Expression<Func<T, object>> sort, bool desc);

        #endregion

        #region count

        Task<long> Count(Expression<Func<T, bool>> search);
        Task<long> Count<F>(Expression<Func<T, F>> field, F value);
        Task<bool> Any(Expression<Func<T, bool>> search);
        Task<bool> Any<F>(Expression<Func<T, F>> field, F value);
        Task<bool> ExistsOne<F>(Expression<Func<T, F>> field, F value);
        Task<bool> ExistsOne(Expression<Func<T, bool>> search);
        Task<bool> ExistsOne(string id);

        #endregion

        #region update

        Task<T> Replace(T entity);
        Task<long> Replace(IEnumerable<T> entities);

        #endregion

        #region delete

        Task DeleteSoft(T entity);
        Task DeleteSoft(string id);
        Task DeleteSoftFirst(Expression<Func<T, bool>> search);
        Task DeleteSoftAll(Expression<Func<T, bool>> search);

        Task DeleteHard(T entity);
        Task DeleteHard(string id);
        Task DeleteHardFirst(Expression<Func<T, bool>> search);
        Task DeleteHardAll(Expression<Func<T, bool>> search);

        #endregion
    }
}
