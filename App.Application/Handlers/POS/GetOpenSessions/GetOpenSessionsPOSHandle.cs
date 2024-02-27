using App.Domain.Entities.POS;
using App.Domain.Models.Response.POS;
using App.Domain.Models.Response.Store.Reports.MainData;
using App.Infrastructure.UserManagementDB;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.GetSessionsOpened
{
    internal class GetOpenSessionsPOSHandle : IRequestHandler<GetOpenSessionsPOSRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<POSSession> _pOSSessionQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRoundNumbers _roundNumbers;
        private readonly iUserInformation _iUserInformation;

        public GetOpenSessionsPOSHandle(IRepositoryQuery<POSSession> POSSessionQuery, iUserInformation iUserInformation, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRoundNumbers roundNumbers)
        {
            _pOSSessionQuery = POSSessionQuery;
            _iUserInformation = iUserInformation;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _roundNumbers = roundNumbers;
        }
        public async Task<ResponseResult> Handle(GetOpenSessionsPOSRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            if (userInfo.employeeId == 1)
                userInfo.otherSettings.canShowAllPOSSessions = true;
            var invoices = _InvoiceMasterQuery.TableNoTracking.Where(x=> x.InvoiceTypeId == (int)Enums.DocumentType.POS  || x.InvoiceTypeId == (int)Enums.DocumentType.ReturnPOS);


            var _openSessions = _pOSSessionQuery.TableNoTracking
                                .Include(x => x.employee)
                                .Where(x => !userInfo.otherSettings.canShowAllPOSSessions ? x.employeeId == userInfo.employeeId : true)
                                .Where(x => x.start.Date >= request.dateFrom.Date && x.start.Date <= request.dateTo.Date || x.sessionStatus != (int)POSSessionStatus.closed);
                                
            var openSessions = _openSessions
                                .Where(x => request.employeeId != 0 ? x.employeeId == request.employeeId : true)
                                .Where(x => request.sessionStatus != POSSessionStatus.Unknown ? x.sessionStatus == (int)request.sessionStatus : true)
                                .OrderBy(x => x.sessionStatus)
                                .ThenByDescending(x => x.start);


            if (openSessions.Count() <= request.PageSize)
                request.PageNumber = 1;
            var pagin = openSessions.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            double MaxPageNumber = openSessions.Count() / Convert.ToDouble(request.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);

            var res = pagin.Select(x => new GetOpenSessionsPOS
            {
                                    Id = x.Id,
                                    code = x.sessionCode,
                                    employeeId = x.employeeId,
                                    arabicName = x.employee.ArabicName,
                                    latinName = x.employee.LatinName,
                                    startDate = x.start,
                                    endDate = x.end,
                                    statusId = x.sessionStatus,
                                    statusAr = getSessionStatus(x.sessionStatus).Item1,
                                    statusEn = getSessionStatus(x.sessionStatus).Item2,
                                    financials = getSessionTotalSales(invoices,x.Id)
                                }).ToList();
            return new ResponseResult()
            {
                Data = res,
                Result = openSessions.Count() > 0 ? Result.Success : Result.Failed,
                TotalCount = _openSessions.Count(),
                DataCount = openSessions.Count(),
                Note = (countofFilter == request.PageNumber ? Actions.EndOfData : "")
            };
        }
        public Tuple<string,string> getSessionStatus(int sessionStatus)
        {
            if (sessionStatus == (int)POSSessionStatus.active)
                return new Tuple<string, string>("مفتوحه", "Active");
            else if(sessionStatus == (int)POSSessionStatus.bining)
                return new Tuple<string, string>("معلق", "Binding");
            else if (sessionStatus == (int)POSSessionStatus.closed)
                return new Tuple<string, string>("مغلق", "Closed");
            return null;
        }
        public GetOpenSessionsPOSTotalSales getSessionTotalSales(IQueryable<InvoiceMaster> invoices,int sessionid)
        {
            var inv = invoices.Where(x => x.POSSessionId == sessionid);
            var totalSales = inv.Where(x => x.InvoiceTypeId == (int)Enums.DocumentType.POS).Sum(x => x.Net);
            var totalReturn = inv.Where(x => x.InvoiceTypeId == (int)Enums.DocumentType.ReturnPOS).Sum(x => x.Net);
            var totlaVat = inv.Where(x => x.InvoiceTypeId == (int)Enums.DocumentType.POS && !x.IsReturn).Sum(x => x.TotalVat);
            var net = _roundNumbers.GetRoundNumber(totalSales - totalReturn);
            return new GetOpenSessionsPOSTotalSales
            {
                totalSales = _roundNumbers.GetRoundNumber(totalSales),
                totalReturn = _roundNumbers.GetRoundNumber(totalReturn),
                totlaVat = _roundNumbers.GetRoundNumber(totlaVat),
                net = net
            };
        }
    }
}
