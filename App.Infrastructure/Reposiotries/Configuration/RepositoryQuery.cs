using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.settings;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace App.Infrastructure.Reposiotries.Configuration
{
    public class RepositoryQuery<T> : IRepositoryQuery<T> where T : class
    {
        private DbSet<T> _entites;

        private ClientSqlDbContext _context = null;
        public virtual IQueryable<T> Table => Entities;
        private readonly IConfiguration _configuration;
        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entites == null)
                    _entites = _context.Set<T>();
                return _entites;
            }

        }

        public IQueryable<T> TableNoTracking => Entities.AsNoTracking();

        //public IQueryable<T>  TableNoTracking1 => Entities.AsNoTracking().ToQueryString();


        public RepositoryQuery(ClientSqlDbContext _context, IConfiguration configuration)
        {
            this._context = _context;
            _entites = Entities;
            _configuration = configuration;
            //table = _context.Set<T>();
        }
        public virtual async Task<List<TType>> Get<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _entites;

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);
             
            return await query.AsNoTracking().Select(select).ToListAsync();
        }
        public virtual async Task<List<T>> Get(Expression<Func<T, T>> select, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = _entites.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);
             

            return await query.AsNoTracking().Select(select).ToListAsync();
        }
        public virtual async Task<List<T>> Get(int? pageIndex, int? pageSize, Expression<Func<T, T>> select, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = _entites.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (pageIndex != null)
                query = query.Skip((pageIndex ?? 0 - 1) * pageSize ?? 0).Take(pageSize ?? 0);
             

            return await query.AsNoTracking().Select(select).ToListAsync();
        }
        public virtual async Task<List<T>> Get(Expression<Func<T, T>> select, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _entites;

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);
             

            return await query.AsNoTracking().Select(select).ToListAsync();
        }
        public virtual async Task<List<TType>> Get<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = _entites;


            if (filter != null)
                query = query.Where(filter);
             

            return await query.AsNoTracking().Select(select).ToListAsync();
        }
        public virtual async Task<List<T>> Get()
        {
            //IQueryable<T> query = _entites;


             

            return await Entities.ToListAsync();
        }
        public virtual async Task<List<T>> GetWithPaging(int? pageIndex, int? pageSize)
        {
            //IQueryable<T> query = _entites;
            if (pageSize == null)
                return await Entities.ToListAsync();
            if (pageSize == 0)
                return await Entities.ToListAsync();
             

            return await Entities.Skip((pageIndex ?? 0 - 1) * pageSize ?? 0).Take(pageSize ?? 0).ToListAsync();
        }

        public virtual async Task<List<T>> GetWithPaging(int? pageIndex, int? pageSize, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _entites;
            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);
            //IQueryable<T> query = _entites;
            if (pageSize == 0)
                return await query.ToListAsync();
             

            return await query.Skip((pageIndex ?? 0 - 1) * pageSize ?? 0).Take(pageSize ?? 0).ToListAsync();
        }
        public virtual async Task<List<T>> GetWithPaging(int? pageIndex, int? pageSize, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _entites;

            if (filter != null)
                query = query.Where(filter);

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);
            //IQueryable<T> query = _entites;
            if (pageSize == 0)
                return await query.ToListAsync();
             

            return await query.Skip((pageIndex ?? 0 - 1) * pageSize ?? 0).Take(pageSize ?? 0).ToListAsync();
        }


        public virtual async Task<TType> GetFirstOrDefault<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _entites;

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);
            if (filter != null)
                query = query.Where(filter);
             

            return await query.Select(select).FirstOrDefaultAsync();
        }
        //public virtual async Task<TType> GetFirstOrDefault<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> filter = null)
        //{
        //    IQueryable<T> query = _entites;

        //    if (filter != null)
        //        query = query.Where(filter);
        //    return await query.Select(select).FirstOrDefaultAsync();
        //}



        public virtual async Task<TType> GetFirstOrDefault<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _entites;


            if (filter != null)
                query = query.Where(filter);

            return await query.Select(select).FirstOrDefaultAsync();
        }

        public virtual async Task<List<TType>> Get<TType>(Expression<Func<T, TType>> select)
        {
            IQueryable<T> query = _entites;

            return await query.AsNoTracking().Select(select).ToListAsync();
        }

        public virtual async Task<List<T>> Get(Expression<Func<T, T>> select, Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _entites;
            if (filter != null)
                query = query.Where(filter);

            return await query.AsNoTracking().Select(select).ToListAsync();
        }

        public virtual async Task<List<TType>> Get<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _entites;

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);
            if (filter != null)
                query = query.Where(filter);

            return await query.Select(select).ToListAsync();
        }

        public async Task<T> GetByIdAsync(object ID)
        {
             

            return await Entities.FindAsync(ID);
        }

        public int Count()
        {
             

            return TableNoTracking.Count();
        }
        public int Count(Expression<Func<T, bool>> predicate)
        {
             

            return TableNoTracking.Where(predicate).Count();
        }
        public async Task<int> CountAsync()
        {
             

            return await TableNoTracking.CountAsync();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                this.disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T Find(Expression<Func<T, bool>> match)
        {
            return TableNoTracking.SingleOrDefault(match);
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> match)
        {
            return TableNoTracking.Where(match).ToList();
        }
        public IQueryable<TResult> FindSelectorQueryable<TResult>(IQueryable<T> myQueryable,
                                 Expression<Func<T, TResult>> selector)
        {
            return myQueryable.Select(selector);
        }

        public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await TableNoTracking.Where(match).ToListAsync();
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await TableNoTracking.SingleOrDefaultAsync(match);
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return TableNoTracking.Where(predicate);
        }

        public async Task<ICollection<T>> FindByAsyn(Expression<Func<T, bool>> predicate)
        {
            return await Entities.Where(predicate).ToListAsync();

        }

        public async Task<ICollection<T>> FindByAsyn(int? pageIndex, int? pageSize, Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            IQueryable<T> query = _entites;

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            if (pageSize == 0)
                return await query.ToListAsync();
            // return await Entities.Where(predicate).ToListAsync();
            //return await Entities.Where(predicate).Skip((pageIndex ?? 0 - 1) * pageSize ?? 0).Take(pageSize ?? 0).ToListAsync();
            return await query.Skip((pageIndex ?? 0 - 1) * pageSize ?? 0).Take(pageSize ?? 0).ToListAsync();

        }

        public async Task<ICollection<T>> FindByAsyn(int? pageIndex, int? pageSize ,Expression<Func<T, bool>> predicate)
        {
            return await FindByAsyn(pageIndex, pageSize, predicate , null);
        }
        public T Get(int id)
        {
            return Entities.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return TableNoTracking;
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return TableNoTracking.Where(predicate);
        }

        public async Task<ICollection<T>> GetAllAsyn()
        {
            return await TableNoTracking.ToListAsync();
        }
        public async Task<ICollection<T>> GetAllAsyn(Expression<Func<T, bool>> predicate)
        {
            return await TableNoTracking.Where(predicate).ToListAsync();
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = GetAll();
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {

                queryable = queryable.Include<T, object>(includeProperty);
            }

            return queryable;
        }
     
        public  async Task<IList<T>> GetAllIncludingAsync(int? pageIndex, int? pageSize, Expression<Func<T, bool>> predicate , Func<IQueryable<T>, IOrderedQueryable<T>> orderBy , params Expression<Func<T, object>>[] includeProperties )
        {
            IQueryable<T> query = TableNoTracking;
            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            if (pageSize != 0)
                query = query.Skip((pageIndex ?? 0 - 1) * pageSize ?? 0).Take(pageSize ?? 0);

            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {
                query = query.Include<T, object>(includeProperty);
            }
            



            return await query.ToListAsync();
        }

        public  async Task<IList<T>> GetAllIncludingAsync(int? pageIndex, int? pageSize, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return await GetAllIncludingAsync(pageIndex, pageSize, predicate, null, includeProperties);

        }

        public async Task<T> GetAsync(int? id)
        {
            return await Entities.FindAsync(id);
        }

        public async Task<T> GetByAsync(Expression<Func<T, bool>> predicate)
        {
            return await Entities.Where(predicate).FirstOrDefaultAsync();
        }
        public async Task<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = Entities.OfType<T>();
            query = includes.Aggregate(query, (current, property) => current.Include(property));
            return await query.Where(predicate).FirstOrDefaultAsync();
        }
        public async Task<IList<T>> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = Entities.OfType<T>();
            query = includes.Aggregate(query, (current, property) => current.Include(property));
            return await query.Where(predicate).ToListAsync();
        }
        public async Task<T> SingleOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var result = await Find(predicate, includes);
            return result.SingleOrDefault();
        }
        public IQueryable<T> FindQueryable(Expression<Func<T, bool>> predicate)
        {
            
            return Entities.Where(predicate).AsQueryable();
        }
        public TType GetMaxCode<TType>(Expression<Func<T, TType>> select)
        {
           return GetMaxCode<TType>(select , null);
        }

        public TType GetMaxCode<TType>(Expression<Func<T, TType>> select , Expression<Func<T, bool>> predicate)
        {

            try
            {
                if (predicate == null)
                    return Entities.Select(select).Max();
                else
                    return Entities.Where(predicate).Select(select).Max();
            }
            catch (Exception)
            {

                return default(TType);
            }

        }


        public TType GetMinCode<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> predicate)
        {

            try
            {
                if (predicate == null)
                    return Entities.Select(select).Min();
                else
                    return Entities.Where(predicate).Select(select).Min();
            }
            catch (Exception)
            {

                return default(TType);
            }

        }


        public IQueryable<T> FindThenInclude(
                                 Expression<Func<T, bool>> predicate = null,
                                 Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                 Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                                 bool disableTracking = true)
        {
            IQueryable<T> query = Entities;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.AsQueryable();
        }

        public async Task<IEnumerable<T>> QuerySrting(string Query)
        {
            SqlConnection _sqlConnection = new SqlConnection(defultData.ERPConnectionString(_configuration, _context.Connection.Database));
            await _sqlConnection.OpenAsync();
            var Data = await _sqlConnection.QueryAsync<T>(Query);
            await _sqlConnection.CloseAsync();
            return Data;
        }
        public void ClearTracking()
        {
            _context.ChangeTracker.Clear();
        }
        public string databaseName()
        {
            return _context.Database.ProviderName;
        }

        public string connectionString()
        {
            return _context.Database.GetConnectionString();
        }

    }

}
