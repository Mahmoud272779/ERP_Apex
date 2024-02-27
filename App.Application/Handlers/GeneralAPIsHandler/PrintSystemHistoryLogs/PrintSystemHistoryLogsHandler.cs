using App.Application.Handlers.GeneralAPIsHandler.GetSystemHistoryLogs;
using App.Application.Helpers;
using App.Domain.Entities.Process.General;
using App.Domain.Models.Request.General;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.PrintSystemHistoryLogs
{
    public  class PrintSystemHistoryLogsHandler : IRequestHandler<PrintSystemHistoryLogsRequest, WebReport>
    {
        private readonly IMediator mediator;
        private readonly iUserInformation _userInfo;
        private readonly IGeneralPrint _iGeneralPrint;

        public PrintSystemHistoryLogsHandler(IMediator mediator, iUserInformation userInfo, IGeneralPrint iGeneralPrint)
        {
            this.mediator = mediator;
            this._userInfo = userInfo;
            _iGeneralPrint = iGeneralPrint;
        }
        public async Task<WebReport> Handle( PrintSystemHistoryLogsRequest request,CancellationToken cancellationToken)
        {
            var req = new GetSystemHistoryLogsRequest()
            { dateFrom = request.dateFrom, dateTo = request.dateTo ,empId=request.empId,isPrint=true};
            var data = await  mediator.Send(req);
            var userInfo =await  _userInfo.GetUserInformation();
            var systemHistoryData =  (IQueryable<HistoryMovement>) data.Data;
           
           
             var otherData = ArabicEnglishDate.OtherDataWithDatesArEn(request.isArabic,request.dateFrom,request.dateTo);



            otherData.EmployeeName = userInfo.employeeNameAr.ToString();

            otherData.EmployeeNameEn = userInfo.employeeNameEn.ToString();


            int screenId = (int)SubFormsIds.userActions;
            var tablesNames = new TablesNames()
            {

                FirstListName = "SystemHistoryLogs",
                

            };
            var report = await _iGeneralPrint.PrintReport< object, HistoryMovement, object>( null, systemHistoryData.ToList(), null, tablesNames, otherData
             , screenId,request.exportType,request.isArabic,request.fileId);
            return report;
        }
    }
}
