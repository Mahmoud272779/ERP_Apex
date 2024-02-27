using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Query
{
    public class CheckStoreQuery : IRequestHandler<CheckStoreRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpStores> storesRepositoryQuery;

        public CheckStoreQuery(IRepositoryQuery<InvStpStores> StoresRepositoryQuery)
        {
            storesRepositoryQuery = StoresRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(CheckStoreRequest request, CancellationToken cancellationToken)
        {
            var store = await storesRepositoryQuery.FindAsync(e => e.Code == request.Code);
            if (store==null)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };
                
            }
            else if (store.Status == 2)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.InActive };
            }
            else
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
            }
        }
    }
}
