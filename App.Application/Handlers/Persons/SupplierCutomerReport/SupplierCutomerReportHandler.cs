using App.Domain.Models.Request.print;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Persons
{
    public class SupplierCutomerReportHandler : IRequestHandler<SupplierCutomerReportRequest, WebReport>
    {
        private readonly IMediator _mediator;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;
        public SupplierCutomerReportHandler(IMediator mediator, iUserInformation iUserInformation, IGeneralPrint iGeneralPrint)
        {
            _mediator = mediator;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
        }



        public async Task<WebReport> Handle(SupplierCutomerReportRequest parameters, CancellationToken cancellationToken)
        {
            var data = await _mediator.Send(new GetListOfPersonsRequest
            {
                ids = parameters.ids,
                isSearchData = parameters.isSearchData,
                isPrint = true,
                IsSupplier = parameters.IsSupplier,
                Name = parameters.Name,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                Status = parameters.Status,
                TypeArr = parameters.TypeArr,
                SalesManId=parameters.SalesManId,
            });

            var userInfo = await _iUserInformation.GetUserInformation();


            var mainData = (List<PersonsReponseDto>)data.Data;

            var otherdata = new ReportOtherData()
            {
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

            };
            



            int screenId = 0;
            if (parameters.IsSupplier == true)
            {
                screenId = (int)SubFormsIds.Suppliers_Purchases;

            }
            else
            {
                screenId = (int)SubFormsIds.Customers_Sales;

            }
            var tablesNames = new TablesNames()
            {
                FirstListName = "Persons"
            };
            var report = await _iGeneralPrint.PrintReport<object, PersonsReponseDto, object>(null, mainData, null, tablesNames, otherdata
                , screenId, parameters.exportType, parameters.isArabic,parameters.fileId);
            return report;
        }
    }
}
