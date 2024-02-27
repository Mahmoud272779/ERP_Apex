using App.Infrastructure.Persistence.Context;
using App.Infrastructure.UserManagementDB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.UnitOfWork
{
    public class DbFactory : IDisposable
    {
        private bool _disposed;
        private bool _UsersManagerContext_disposed;


        private Func<ClientSqlDbContext> _instanceFunc;
        private Func<ERP_UsersManagerContext> _UsersManagerContext_instanceFunc;


        private ClientSqlDbContext _dbContext;
        private ERP_UsersManagerContext _UsersManagerContext;
        private SqlConnection _con;


        public ClientSqlDbContext DbContext => _dbContext ?? (_dbContext = _instanceFunc.Invoke());
        //public ERP_UsersManagerContext _ERP_UsersManagerContextDbContext => _UsersManagerContext ?? (_UsersManagerContext = _UsersManagerContext_instanceFunc.Invoke());


        public DbFactory(ClientSqlDbContext dbContextFactory, ERP_UsersManagerContext ERP_UsersManagerContext, SqlConnection con)
        {
            _dbContext = dbContextFactory;
            _UsersManagerContext = ERP_UsersManagerContext;
            _con = con;
        }

        public void Dispose()
        {
            if (!_disposed && _dbContext != null)
            {
                _disposed = true;
                try
                {
                    _dbContext.Database.CloseConnection();
                }
                catch (Exception)
                {

                }
                try
                {
                    _dbContext.ClearConnectionPool();
                }
                catch (Exception)
                {

                }
                _dbContext.Dispose();
            }

            if(!_UsersManagerContext_disposed && _UsersManagerContext != null)
            {
                _UsersManagerContext_disposed = true;
                try
                {
                    _UsersManagerContext.Database.CloseConnection();
                }
                catch (Exception)
                {

                }

                _UsersManagerContext.Dispose();
            }

            if (_con.State == System.Data.ConnectionState.Open)
                _con.Close();
            _con.Dispose();

        }
    }
}
