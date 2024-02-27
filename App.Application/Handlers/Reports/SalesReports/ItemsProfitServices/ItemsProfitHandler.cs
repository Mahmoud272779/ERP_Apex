using App.Application.Handlers.Reports.SalesReports.SalesOfSalesMan;
using App.Application.Helpers.LinqExtensions;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DocumentType = App.Domain.Enums.Enums.DocumentType;

namespace App.Application.Handlers.Reports.SalesReports.ItemsProfitServices
{
    public class ItemsProfitHandler : IRequestHandler<ItemsProfitRequest, ResponseResult>
    {

        private readonly IRepositoryQuery<InvoiceDetails> InvoiceQuery;
        private readonly IRoundNumbers roundNumber;
        private readonly iUserInformation _userInformaiotn;

        public ItemsProfitHandler(IRepositoryQuery<InvoiceDetails> invoiceQuery, IRoundNumbers RoundNumber,iUserInformation userInformaiotn )
        {
            InvoiceQuery = invoiceQuery;
            roundNumber = RoundNumber;
            _userInformaiotn = userInformaiotn;
        }
        private Expression<Func<InvoiceDetails, bool>> GetFilter(ItemsProfitRequest Parameter, int[] branches,UserInformationModel userinfo)
        {
            //var uInfo = await _userInformaiotn.GetUserInformation();
            var salesShowOtherPersons = userinfo.otherSettings.salesShowOtherPersonsInv;
            var posShowOtherPersons = userinfo.otherSettings.posShowOtherPersonsInv;
            var salesType = new List<int> { (int)DocumentType.Sales, (int)DocumentType.ReturnSales };
            var posType = new List<int> { (int)DocumentType.POS, (int)DocumentType.ReturnPOS };
            List<int> invoices = new List<int>();
            Expression<Func<InvoiceDetails, bool>> filter = h =>
                                             h.InvoicesMaster.InvoiceDate >= Parameter.dateFrom
                                          && h.InvoicesMaster.InvoiceDate <= Parameter.dateTo
                                          && h.InvoicesMaster.IsDeleted == false
                                          && branches.Contains(h.InvoicesMaster.BranchId)

                                          && (!salesShowOtherPersons && salesType.Contains(h.InvoicesMaster.InvoiceTypeId) ? h.InvoicesMaster.EmployeeId == userinfo.employeeId : 1 == 1)
                                          && (!posShowOtherPersons && posType.Contains(h.InvoicesMaster.InvoiceTypeId) ? h.InvoicesMaster.EmployeeId == userinfo.employeeId : 1 == 1)

                                          && Lists.SalesWithOutDeleteInvoicesList.Contains(h.InvoicesMaster.InvoiceTypeId)
                                          && (Parameter.itemId == 0 ? 1 == 1 : h.ItemId == Parameter.itemId)
                                          && (h.parentItemId == null || h.parentItemId == 0)
                                          && (Parameter.categoryId == 0 ? 1 == 1 : h.Items.GroupId == Parameter.categoryId);


            return filter;
        } 



        public async Task<ResponseResult> Handle(ItemsProfitRequest Parameter, CancellationToken cancellationToken)
        {    
            var uInfo = await _userInformaiotn.GetUserInformation();

            if (string.IsNullOrEmpty(Parameter.branches))
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "branches is required" };

            // Filter branches before joining tables
            var branches = Parameter.branches.Split(',').Select(c => int.Parse(c)).ToArray();

            // Use deferred execution
            var Fresult = InvoiceQuery.TableNoTracking
           .Where(GetFilter(Parameter, branches, uInfo))
           .GroupBy(a => a.ItemId)
           .Select(h => new ItemsProfitResponse(roundNumber)
           {
               itemNameAr = h.FirstOrDefault().Items.ArabicName,
               itemNameEn = h.FirstOrDefault().Items.LatinName,
               ItemCode = h.FirstOrDefault().Items.ItemCode,
               QTY = roundNumber.GetRoundNumber(h.Sum(a => a.Quantity * a.ConversionFactor * a.Signal * -1)),
               Cost = roundNumber.GetRoundNumber(h.Where(a => a.parentItemId == null || a.parentItemId == 0)
                       .Sum(s => s.Cost * s.Quantity * s.ConversionFactor * s.Signal * -1)),
               Net = roundNumber.GetRoundNumber(h.Sum(a => (a.InvoicesMaster.PriceWithVat ? (a.TotalWithSplitedDiscount - a.VatValue) : a.TotalWithSplitedDiscount) * a.Signal * -1)),
               
           }).AsEnumerable().Where(a => a.QTY !=0).ToList();

            //var Fresult = result.Where(a=>a.QTY != 0).ToList();

            // Use pagination if needed
            var finalResult = new TotalsItemsProfitResponse()
            {
                TotalCost = roundNumber.GetRoundNumber(Fresult.Sum(a => a.Cost)),
                TotalNet = roundNumber.GetRoundNumber(Fresult.Sum(a => a.Net)),
                TotalProfit = roundNumber.GetRoundNumber(Fresult.Sum(a => a.Profit)),
                TotalQTY= roundNumber.GetRoundNumber(Fresult.Sum(a => a.QTY)),
                Data =Parameter.isPrint ? Fresult: Pagenation<ItemsProfitResponse>.pagenationList(Parameter.PageSize, Parameter.PageNumber, Fresult),

            };

            return new ResponseResult() { Data = finalResult, Result = Result.Success, DataCount = Fresult.Count() };
        }
    }
}
