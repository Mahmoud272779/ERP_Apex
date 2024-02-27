using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace App.Infrastructure.Interfaces
{
    public interface IApplicationReadDbConnection
    {
        Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null);
        Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null);
    }
}
