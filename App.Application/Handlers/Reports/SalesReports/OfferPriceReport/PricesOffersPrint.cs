using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.SalesReports.OfferPriceReport
{
    public class PricesOffersPrint : IPricesOffersPrint
    {
        private readonly IMediator _mediator;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly IRepositoryQuery<GLBranch> _GLBranchQuery;

        public PricesOffersPrint(IMediator mediator, iUserInformation iUserInformation, IGeneralPrint iGeneralPrint, IRepositoryQuery<GLBranch> gLBranchQuery)
        {
            _mediator = mediator;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _GLBranchQuery = gLBranchQuery;
        }

        public async Task<WebReport> PricesOffersReportPrint(offerpriceReportRequest parameter, exportType exportType, bool isArabic, int fileId)
        {
            if (parameter.priceTo == 0)
            {
                parameter.priceTo = null;
            }
            parameter.isPrint = true;
            var data = await _mediator.Send(parameter);

            //OfferPriceReportResponse
                var mainData =(IQueryable<OfferPriceReportResponse>) data.Data;

            var userInfo = await _iUserInformation.GetUserInformation();




            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, parameter.dateFrom, parameter.dateTo);



            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();



            if (parameter.branches != null) 
            {
                var branchesList = parameter.branches.Split(',').Select(c => int.Parse(c)).ToArray();

                if (branchesList.Count() > 0)
                {
                    if (branchesList[0] == 0)
                    {
                        otherdata.ArabicName = "الكل";
                        otherdata.LatinName = "all";
                    }
                    else
                    {
                        var branchesName = _GLBranchQuery.TableNoTracking.Where(a => branchesList.Contains(a.Id)).ToList();

                        otherdata.ArabicName = string.Join(" , ", branchesName.Select(a => a.ArabicName));
                        otherdata.LatinName = string.Join(" , ", branchesName.Select(a => a.LatinName));

                    }
                }

            }
            
            var tablesNames = new TablesNames()
            {
                FirstListName = "PricesOffersReport"
            };
            var report = await _iGeneralPrint.PrintReport<object, OfferPriceReportResponse, object>(null, mainData.ToList(), null, tablesNames, otherdata
                , (int)SubFormsIds.offerPriceReport, exportType, isArabic, fileId);
            return report;



            
        }
    }
}
