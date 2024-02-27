using App.Infrastructure.Persistence.Context;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.ActivePOSSession
{
    public class ActivePOSSessionRequest : IRequest<ResponseResult>
    {
        public int employeeId { get; set; }
        public string databaseName { get; set; }
        public ClientSqlDbContext dbContext { get; set; }
    }
}
