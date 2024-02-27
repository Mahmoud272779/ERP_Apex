using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.UnitOfWork;
using App.Infrastructure.settings;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
 
namespace App.Infrastructure.Reposiotries.Configuration
{
    public class RepositoryCommand<T> :UnitOfWork, IRepositoryCommand<T> where T : class
    {
        private readonly DbFactory _dbFactory;
        private DbSet<T> _dbSet;
        protected DbSet<T> _entites
        {
            get => _dbSet ?? (_dbSet = _dbFactory.DbContext.Set<T>());
        }


        //private DbSet<T> _entites;
        private readonly ClientSqlDbContext _context = null;
        private IDbContextTransaction _transaction;
        private readonly DbSet<T> table = null;
        private readonly IConfiguration _configuration;
        public virtual IQueryable<T> Table => Entities;
        protected virtual DbSet<T> Entities
        {
            get
            {
                //if (_entites == null)
                //    _entites = _context.Set<T>();
                return _entites;
            }


        }
        public RepositoryCommand(ClientSqlDbContext _context, DbFactory dbFactory, IConfiguration configuration) : base(dbFactory)
        {
            this._context = _context;
            table = _context.Set<T>();
            _dbFactory = dbFactory;
            _configuration = configuration;
            //_entites = _context.Set<T>();

        }
        /// <summary>
        /// Rollback of entity changes and return full error message
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Error message</returns>
        /// 



        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
            
        }

        public void StartTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
            _transaction.Dispose();
        }
        public void UpdateFieldsSave(T entity, params Expression<Func<T, object>>[] includeProperties)
        {
            var dbEntry = _context.Entry(entity);
            foreach (var includeProperty in includeProperties)
            {
                dbEntry.Property(includeProperty).IsModified = true;
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public object GetKeyValue(T t)
        {
            var key =
                typeof(T).GetProperties().FirstOrDefault(
                    p => p.GetCustomAttributes(typeof(KeyAttribute), true).Length != 0);
            return key?.GetValue(t, null);
        }
        public bool Update(T entity)
        {
            var key = GetKeyValue(entity);

            Update(entity, key);

            return true;
        }

        public void Update(T entity, object key)
        {
            var originalEntity = Entities.Find(key);

            Update(originalEntity, entity);
        }

        public void Update(T originalEntity, T newEntity)
        {
            _context.Entry(originalEntity).CurrentValues.SetValues(newEntity);
        }



        /// <summary>
        /// ////////////////////////////////
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (_context == null)
            {
                return;
            }

            _context.Dispose();
           // _context = null;
        }
        protected string GetFullErrorTextAndRollbackEntityChanges(DbUpdateException exception)
        {
            //rollback entity changes
            if (_context is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

                entries.ForEach(entry =>
                {
                    try
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    catch (InvalidOperationException)
                    {
                        // ignored
                    }
                });
            }

            try
            {
                _context.SaveChanges();
                return exception.ToString();
            }
            catch (Exception ex)
            {
                //if after the rollback of changes the context is still not saving,
                //return the full text of the exception that occurred when saving
                return ex.ToString();
            }
        }
        public async Task<bool> SaveAsync()
        {
            bool result = await _context.SaveChangesAsync() != 0;
            return result;
        }
        public virtual bool Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Add(entity);
                return (_context.SaveChanges() == 0) ? false : true;

            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }
        public void AddRange(IEnumerable<T> entities)
        {
            Entities.AddRange(entities);
           // _context.SaveChanges();
        }
        public void AddRangeAsync(IEnumerable<T> entities)
        {
            Entities.AddRangeAsync(entities);
             _context.SaveChanges();
        }
        public T AddWithoutSaveChanges(T newEntity)
        {
            return Entities.Add(newEntity).Entity;
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            try
            {
                _entites.Add(entity);

                return await CommitUnSaved();

                //return await SaveAsync();

            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }
        public async Task<bool> AddAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));


            try
            {
                Entities.AddRange(entities);
                return await SaveAsync();
            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }
        public virtual async Task<bool> UpdateAsyn(T entity)
        {
           
           // _dbFactory.DbContext.Database.Log = Console.Write;
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {             
                Entities.Update(entity);
                // throw new DbUpdateException();
                //StreamWriter sw = new StreamWriter(@"C:\\Users\\Administrator\hamadaLog.txt");
                //sw.Write(entity.ToString());
                //_context.Database.q =sw.Write;
                //sw.Flush();
                //sw.Close();
                //
                return await CommitUnSaved();
                
            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }
        public virtual async Task<bool> UpdateAsyn(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.UpdateRange(entities);
                return await SaveAsync();
            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }
        public virtual async Task<bool> UpdateAsynWithOutSave(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.UpdateRange(entities);
                return await CommitUnSaved();

            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }
        public virtual async Task<bool> DeleteAsync(object id)
        {
            T entityToDelete = _entites.Find(id);
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _context.Attach(entityToDelete);
            }
            _entites.Remove(entityToDelete);
            return await SaveAsync();
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _entites.RemoveRange(entities);
        }
        public void Remove(T entity)
        {
            _entites.Remove(entity);
        }
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _entites;
            if (filter != null)
                query = query.Where(filter);
            _entites.RemoveRange(await query.ToListAsync());
            return await SaveAsync();
        }

        public async Task<T> GetFirstOrDefault<TType>(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _entites;
            if (filter != null)
                query = query.Where(filter);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<List<T>> Get(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _entites;
            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }
        public async Task<T> GetByAsync(Expression<Func<T, bool>> predicate)
        {
            return await Entities.Where(predicate).FirstOrDefaultAsync();
        }
        public async Task<T> GetByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _entites;
            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);
            
            return await query.Where(predicate).FirstOrDefaultAsync();
        }
        public virtual void UnSavedDelete(object id)
        {
            T entityToDelete = _entites.Find(id);
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _context.Attach(entityToDelete);
            }
            _entites.Remove(entityToDelete);
            //return await SaveAsync();
        }
        public virtual async Task UnSavedDeleteAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _entites;
            if (filter != null)
                query = query.Where(filter);
            _entites.RemoveRange(await query.ToListAsync());
        }

        public async Task<bool> CommitUnSaved()
        {
           return await base.CommitAsync();
        }

        public void ClearTracking()
        {
            _context.ChangeTracker.Clear();
        }

        public async Task<bool> ExcuteQuery(string Query)
        {
            SqlConnection _sqlConnection = new SqlConnection(defultData.ERPConnectionString(_configuration, _context.Connection.Database));
            await _sqlConnection.OpenAsync();
            var Excuted = await _sqlConnection.ExecuteAsync(Query);
            await _sqlConnection.CloseAsync();
            return Excuted > 0 ? true : false;
        }
    }
   
}
