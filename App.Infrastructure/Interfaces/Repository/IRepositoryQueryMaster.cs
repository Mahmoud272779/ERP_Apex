using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Infrastructure.Interfaces.Repository
{
    public interface IRepositoryQueryMaster<T> where T : class
    {
        Task<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<List<T>> Get();
        Task<List<T>> GetWithPaging(int? pageIndex, int? pageSize);
        Task<List<T>> GetWithPaging(int? pageIndex, int? pageSize, params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetWithPaging(int? pageIndex, int? pageSize, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);

        Task<List<TType>> Get<TType>(Expression<Func<T, TType>> select);
        Task<List<TType>> Get<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> filter);
        Task<List<TType>> Get<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);
        Task<List<TType>> Get<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes);

        #region Methods
        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        Task<T> GetByIdAsync(object ID);
        //Task<List<T>> Get(Expression<Func<T, T>> select, Expression<Func<T, bool>> filter = null);
        //Task<List<T>> Get(Expression<Func<T, T>> select , Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        // Task<List<T>> Get();
        /// <summary>
        /// Get single entity by primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        /// <summary>
        /// Get first or default entity by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<TType> GetFirstOrDefault<TType>(Expression<Func<T, TType>> select,
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] includes);

        Task<TType> GetFirstOrDefault<TType>(Expression<Func<T, TType>> select,
           Expression<Func<T, bool>> filter = null);


        int Count();
        int Count(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync();
        void Dispose();
        T Find(Expression<Func<T, bool>> match);
        ICollection<T> FindAll(Expression<Func<T, bool>> match);
        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<ICollection<T>> FindByAsyn(Expression<Func<T, bool>> predicate);
        Task<ICollection<T>> FindByAsyn(int? pageIndex, int? pageSize, Expression<Func<T, bool>> predicate);
        Task<ICollection<T>> FindByAsyn(int? pageIndex, int? pageSize, Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);

        T Get(int id);
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
        // IQueryable<T> GetAll2();
        Task<ICollection<T>> GetAllAsyn();
        Task<ICollection<T>> GetAllAsyn(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);
        Task<IList<T>> GetAllIncludingAsync(int? pageIndex, int? pageSize, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IList<T>> GetAllIncludingAsync(int? pageIndex, int? pageSize, Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, params Expression<Func<T, object>>[] includeProperties);

        Task<T> GetAsync(int? id);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
        Task<IList<T>> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T> SingleOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        IQueryable<TResult> FindSelectorQueryable<TResult>(IQueryable<T> myQueryable,
                             Expression<Func<T, TResult>> selector);
        IQueryable<T> FindQueryable(Expression<Func<T, bool>> predicate);
        #endregion
        IQueryable<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }
        TType GetMaxCode<TType>(Expression<Func<T, TType>> select);
        TType GetMaxCode<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> predicate);
        IQueryable<T> FindThenInclude(
                                 Expression<Func<T, bool>> predicate = null,
                                 Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                 Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                                 bool disableTracking = true);
    }

}
