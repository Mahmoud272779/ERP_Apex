
using App.Application.Handlers.Reports.SalesReports.ItemsProfitServices;
using App.Application.Helpers.LinqExtensions;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Infrastructure.UserManagementDB;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Models.Security.Authentication.Response.Totals;
using DocumentType = App.Domain.Enums.Enums.DocumentType;

namespace App.Application.Handlers.Reports.SalesReports.SalesOfSalesMan
{
    public class SalesOfSalesManHandeler : IRequestHandler<SalesOfSalesManRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceQuery;
        private readonly IRoundNumbers roundNumber;
        private readonly iUserInformation _iUserInformation;

        public SalesOfSalesManHandeler(IRepositoryQuery<InvoiceMaster> invoiceQuery, IRoundNumbers RoundNumber, iUserInformation iUserInformation)
        {
            InvoiceQuery = invoiceQuery;
            roundNumber = RoundNumber;
            _iUserInformation = iUserInformation;
        }

        public async Task<ResponseResult> Handle(SalesOfSalesManRequest Parameter, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(Parameter.branches))
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "branches is required" };
            var branches = Parameter.branches.Split(',').Select(c => int.Parse(c)).ToArray();


            var totalDataCount = InvoiceQuery.TableNoTracking
                             .Where(h => h.InvoiceDate >= Parameter.dateFrom && h.InvoiceDate <= Parameter.dateTo && h.IsDeleted == false && branches.Contains(h.BranchId) &&
                             Lists.SalesWithOutDeleteInvoicesList.Contains(h.InvoiceTypeId)//(h.InvoiceTypeId == (int)DocumentType.POS || h.InvoiceTypeId == (int)DocumentType.Sales)
                             && (Parameter.PaymentType == 0 ? 1 == 1 : h.PaymentType == Parameter.PaymentType) && h.SalesManId == Parameter.SalesManID).Count();
            var paymentType = Lists.paymentTypes;
            var userinfo = await _iUserInformation.GetUserInformation();
            var listSalesManSalesData = InvoiceQuery
                .TableNoTracking
                .Include(c=> c.Person)
                .Where(getFilter(Parameter, branches))
                .Where(x=> !userinfo.otherSettings.salesShowOtherPersonsInv ? x.EmployeeId == userinfo.employeeId : true)
                            .Select(h => new SalesOfSalesManResponse()
                            {
                                FullInvoiceDate = h.InvoiceDate,
                                InvoiceDate = h.InvoiceDate.ToString("yyyy-MM-dd"),
                                InvoiceCode = h.InvoiceType,
                                CustomerNameAr = h.Person.ArabicName,
                                CustomerNameEn = h.Person.LatinName,
                                Net = roundNumber.GetRoundNumber(h.PriceWithVat ? (h.TotalAfterDiscount - h.TotalVat) * (h.InvoicesDetails.Select(a => a.Signal).SingleOrDefault() * -1):  (h.TotalAfterDiscount) * (h.InvoicesDetails.Select(a => a.Signal).SingleOrDefault() * -1)),
                                Paid = h.Paid,
                                paymentType = h.PaymentType,
                                //PaymentTypeNameAr = paymentType.Where(c => c.id == h.PaymentType).First().arabicName,
                                //PaymentTypeNameEn = paymentType.Where(c => c.id == h.PaymentType).First().latinName,
                                //PaymentTypeNameAr = h.InvoicePaymentsMethods.Select(a => a.PaymentMethod).Count() > 1 ? " اخرى " : h.InvoicePaymentsMethods.Select(a => a.PaymentMethod).Count() == 1 ? h.InvoicePaymentsMethods.Select(a => a.PaymentMethod).Select(a => a.ArabicName).FirstOrDefault() : "اجل",
                                //PaymentTypeNameEn = h.InvoicePaymentsMethods.Select(a => a.PaymentMethod).Count() > 1 ? " Other " : h.InvoicePaymentsMethods.Select(a => a.PaymentMethod).Count() == 1 ? h.InvoicePaymentsMethods.Select(a => a.PaymentMethod).Select(a => a.LatinName).FirstOrDefault() : "Defree",
                                Cost = roundNumber.GetRoundNumber(h.InvoicesDetails.Where(h => h.parentItemId == null || h.parentItemId == 0).Sum(a => a.Cost * a.Quantity * a.ConversionFactor)) * (h.InvoicesDetails.Select(a => a.Signal).SingleOrDefault() * -1),
                                rowClassName = h.InvoiceTypeId == (int)DocumentType.ReturnPOS || h.InvoiceTypeId == (int)DocumentType.ReturnSales ? "text-danger" : "",
                                discount = roundNumber.GetRoundNumber(h.TotalDiscountValue * (h.InvoicesDetails.Select(a => a.Signal).SingleOrDefault() * -1)),
                                DocumenTypeID = h.InvoiceTypeId,
                                TotalPrice = roundNumber.GetRoundNumber(h.PriceWithVat? (h.TotalPrice -h.TotalVat) * (h.InvoicesDetails.Select(a => a.Signal).SingleOrDefault() * -1): (h.TotalPrice) * (h.InvoicesDetails.Select(a => a.Signal).SingleOrDefault() * -1)),
                                Vat = roundNumber.GetRoundNumber(h.TotalVat * (h.InvoicesDetails.Select(a => a.Signal).SingleOrDefault() * -1)),
                                InvoiceId= h.InvoiceId,
                                InvoiceTypeId = h.InvoiceTypeId,
                                DocTypeId=-1,
                                PageId = DocHelpers.GetPageIdOfDoc(h.InvoiceTypeId),
                                PageEnum = DocHelpers.GetPageEnum(h.InvoiceTypeId),
                                isCustomer = h.Person.IsCustomer
                            }).ToList();//.Skip(Parameter.isPrint ? 0 : (Parameter.PageNumber - 1) * Parameter.PageSize).TakeIfNotNull(Parameter.isPrint ? null : Parameter.PageSize).ToList();


            listSalesManSalesData.Select(a => a.Profit =  roundNumber.GetRoundNumber(a.Net - a.Cost)).ToList();
            //listSalesManSalesData.Select(a => a.Cost = a.DocumenTypeID == (int)DocumentType.ReturnPOS || a.DocumenTypeID == (int)DocumentType.ReturnSales ? a.Cost * -1 : a.Cost).ToList();
            listSalesManSalesData.Select(a => a.PaymentTypeNameAr = paymentType.Where(c => c.id == a.paymentType).First().arabicName).ToList();
            listSalesManSalesData.Select(a => a.PaymentTypeNameEn = paymentType.Where(c => c.id == a.paymentType).First().latinName).ToList();
                                
            var results = new TotalsOfSalesManData()
            {
               
                TotalDiscount = roundNumber.GetRoundNumber(listSalesManSalesData.Sum(a => a.discount)),
                TotalNet = roundNumber.GetRoundNumber(listSalesManSalesData.Sum(a => a.Net)),
                TotalOfTotalPrice = roundNumber.GetRoundNumber(listSalesManSalesData.Sum(a => a.TotalPrice)),
                TotalPaid = roundNumber.GetRoundNumber(listSalesManSalesData.Sum(a => a.Paid)),
                TotalProfit = roundNumber.GetRoundNumber(listSalesManSalesData.Sum((a) => a.Profit)),
                TotalVat = roundNumber.GetRoundNumber(listSalesManSalesData.Sum(a => a.Vat)),
                _SalesOfSalesManResponseList =Parameter.isPrint?listSalesManSalesData: Pagenation<SalesOfSalesManResponse>.pagenationList( Parameter.PageSize , Parameter.PageNumber, listSalesManSalesData),
               
            };
            return new ResponseResult() { Data = results, Result = Result.Success, DataCount = totalDataCount };

        }
        public Expression<Func<InvoiceMaster, bool>> getFilter(SalesOfSalesManRequest Parameter, int[] branches)
        {
            Expression<Func<InvoiceMaster, bool>> filter = h =>
                                        h.InvoiceDate >= Parameter.dateFrom
                                          && h.InvoiceDate <= Parameter.dateTo
                                          && h.IsDeleted == false
                                          && branches.Contains(h.BranchId)
                                          && Lists.SalesWithOutDeleteInvoicesList.Contains(h.InvoiceTypeId)
                                          && (Parameter.PaymentType == 0 ? 1 == 1 : h.PaymentType == Parameter.PaymentType)
                                          && h.SalesManId == Parameter.SalesManID;


            return filter;
        }
    }
}
