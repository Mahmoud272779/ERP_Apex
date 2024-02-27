using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Infrastructure.Interfaces.Repository
{
    public interface IRepositoryCommand<T> where T : class
    {
        /// <summary>
        /// Insert entity to db
        /// </summary>
        /// <param name="entity"></param>
        /// 
        /// 
        Task<int> SaveChanges();
        void StartTransaction();
        void CommitTransaction();
        void Rollback();
        void Dispose();
        object GetKeyValue(T t);
        bool Update(T entity);
        void Update(T entity, object key);

        void Update(T originalEntity, T newEntity);
        ///////////////
        Task<bool> AddAsync(T entity);
        T AddWithoutSaveChanges(T newEntity);
        bool Add(T entity);
        Task<bool> AddAsync(IEnumerable<T> entities);
        void AddRange(IEnumerable<T> entities);

        void AddRangeAsync(IEnumerable<T> entities);
        /// <summary>
        /// Update entity in db
        /// </summary>
        /// <param name="entity"></param>
        Task<bool> UpdateAsyn(T entity);
        Task<bool> UpdateAsyn(IEnumerable<T> entities);
        Task<bool> UpdateAsynWithOutSave(IEnumerable<T> entities);
        void UpdateFieldsSave(T entity, params Expression<Func<T, object>>[] includeProperties);
        /// <summary>
        /// Delete entity from db by primary key
        /// </summary>
        /// <param name="id"></param>
        Task<bool> DeleteAsync(object id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> DeleteAsync(Expression<Func<T, bool>> filter = null);
        Task<bool> SaveAsync();
        IQueryable<T> Table { get; }
        Task<T> GetFirstOrDefault<TType>(Expression<Func<T, bool>> filter = null);
        Task<List<T>> Get(Expression<Func<T, bool>> filter = null);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        public Task UnSavedDeleteAsync(Expression<Func<T, bool>> filter = null);
        public void UnSavedDelete(object Id);
        public Task<bool> CommitUnSaved();
        public void ClearTracking();

        public Task<bool> ExcuteQuery(string Query);
    }

}
