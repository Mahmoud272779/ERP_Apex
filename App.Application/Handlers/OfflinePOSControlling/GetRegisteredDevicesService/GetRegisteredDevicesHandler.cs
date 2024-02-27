using App.Domain.Entities.POS;
using MediatR;
using System.Linq;
using System.Threading;

namespace App.Application.Handlers.OfflinePOSControlling.GetRegisteredDevicesService
{
    public class GetRegisteredDevicesHandler : IRequestHandler<GetRegisteredDevicesRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<POSDevices> _POSDevicesQuery;
        private readonly IRepositoryQuery<POS_OfflineDevices> _POS_OfflineDevicesQuery;
        private readonly IRepositoryCommand<POS_OfflineDevices> _POS_OfflineDevicesCommand;
        public GetRegisteredDevicesHandler(IRepositoryQuery<POS_OfflineDevices> pOS_OfflineDevicesQuery, IRepositoryCommand<POS_OfflineDevices> pOS_OfflineDevicesCommand, IRepositoryQuery<POSDevices> pOSDevicesQuery)
        {
            _POS_OfflineDevicesQuery = pOS_OfflineDevicesQuery;
            _POS_OfflineDevicesCommand = pOS_OfflineDevicesCommand;
            _POSDevicesQuery = pOSDevicesQuery;
        }
        public async Task<ResponseResult> Handle(GetRegisteredDevicesRequest request, CancellationToken cancellationToken)
        {
            //check if server id exist 
            var isServerIdExist = await _POSDevicesQuery.GetByIdAsync(request.serverId);
            if (isServerIdExist == null)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "هذا الخادم غير موجود",
                    ErrorMessageEn = "This server is not exist"
                };
            //table data
            var table = _POS_OfflineDevicesQuery.TableNoTracking.Where(c => c.ServiceId == request.serverId);

            //Remove Binding Delete eles
            var bindingRemoveData = table.Where(c => c.DeleteWaiting);
            if (bindingRemoveData.Any())
            {
                var DeletedEles = await _POS_OfflineDevicesCommand.DeleteAsync(bindingRemoveData);
                var resData = table.Where(c => !c.DeleteWaiting).ToHashSet();
            }
            return new ResponseResult
            {
                Result = Result.Success,
                Data = table.Where(c => !c.DeleteWaiting).Select(c => new { c.Id, c.ServiceId, c.DeviceSerial })
            };
        }
    }
}
