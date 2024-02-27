using System.Data;
using System.Threading.Tasks;

namespace App.Infrastructure.Interfaces
{
    public interface IApplicationWriteDbConnection : IApplicationReadDbConnection
    {
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null);
    }
}
