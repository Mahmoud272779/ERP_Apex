using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Domain.Entities.POS;
using MediatR;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.OfflinePOSControlling.DeviceRegistrationService
{
    public class DeviceRegistrationHandler : IRequestHandler<DeviceRegistrationRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<POS_OfflineDevices> _POS_OfflineDevicesQuery;
        private readonly IRepositoryCommand<POS_OfflineDevices> _POS_OfflineDevicesCommand;
        private readonly IRepositoryQuery<POSDevices> _POSDevicesQuery;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        public DeviceRegistrationHandler(IRepositoryQuery<POS_OfflineDevices> pOS_OfflineDevicesQuery, IRepositoryCommand<POS_OfflineDevices> pOS_OfflineDevicesCommand, IRepositoryQuery<POSDevices> pOSDevicesQuery, ISecurityIntegrationService securityIntegrationService)
        {
            _POS_OfflineDevicesQuery = pOS_OfflineDevicesQuery;
            _POS_OfflineDevicesCommand = pOS_OfflineDevicesCommand;
            _POSDevicesQuery = pOSDevicesQuery;
            _securityIntegrationService = securityIntegrationService;
        }
        public async Task<ResponseResult> Handle(DeviceRegistrationRequest request, CancellationToken cancellationToken)
        {
            var companyInfo = await _securityIntegrationService.getCompanyInformation();
            //check if server exist
            var isServerExist = _POSDevicesQuery.TableNoTracking.Where(x => x.Id == request.serverId).Any();
            if (!isServerExist)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "هذا الخادم غير موجود",
                    ErrorMessageEn = "This server is not exist"
                };
            //check if device registered 
            var isDeviceRegistered = _POS_OfflineDevicesQuery.TableNoTracking;
            if(isDeviceRegistered.Where(c => c.DeviceSerial == request.deviceSerial).Any())
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "هذه الجهاز مسجلة بالفعل",
                    ErrorMessageEn = "This Device is already registered"
                };
            //Check limit of POS 
            if(isDeviceRegistered.Count() >= companyInfo.AllowedNumberOfExtraOfflinePOS)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "لقد حصلت على الحد الأقصى من نقاط البيع غير المتصلة بالإنترنت",
                    ErrorMessageEn = "You got the maximum of offline POS"
                };
            var ele = new POS_OfflineDevices
            {
                DeleteWaiting = false,
                DeviceSerial = request.deviceSerial,
                ServiceId = request.serverId
            };
            var saveEle = await _POS_OfflineDevicesCommand.AddAsync(ele);
            return new ResponseResult
            {
                Result = saveEle ? Result.Success : Result.Failed,
                ErrorMessageAr = saveEle ? "" : "حدث خطأ أثناء الحفظ",
                ErrorMessageEn = saveEle ? "" : "Error while saving"
            };
        }
    }
}
