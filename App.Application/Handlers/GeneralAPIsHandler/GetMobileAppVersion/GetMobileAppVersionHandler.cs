using App.Application.Handlers.GeneralAPIsHandler.GetOfflineVersion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetMobileAppVersion
{
    internal class GetMobileAppVersionHandler : IRequestHandler<GetMobileAppVersionRequest, ResponseResult>
    {
        public async Task<ResponseResult> Handle(GetMobileAppVersionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var data = Defults.GetMobileAppVersion();
                return new ResponseResult() { Data = data, Id = null, Result = Result.Success }; ;
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Result = Result.NotFound };
            }
        }
    }
}
