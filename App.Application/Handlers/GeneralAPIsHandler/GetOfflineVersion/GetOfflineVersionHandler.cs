using App.Application.Handlers.GeneralAPIsHandler.SetDeletedRecords;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetOfflineVersion
{
    internal class GetOfflineVersionHandler : IRequestHandler<GetOfflineVersionRequest, ResponseResult>
    {
        public async Task<ResponseResult> Handle(GetOfflineVersionRequest request, CancellationToken cancellationToken)
        {
            try 
            {
                var versionNumber = Defults.GetOfflineVersion();
                return new ResponseResult() { Data = versionNumber, Id = null, Result = Result.Success }; ;
            }
            catch (Exception ex) 
            {
                return new ResponseResult() { Result = Result.NotFound };
            }
        }
    }
}
