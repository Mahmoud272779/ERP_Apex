using App.Infrastructure.settings;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.SalesReports.OfferPriceReport
{
    public class offerpriceReportHandler : IRequestHandler<offerpriceReportRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<OfferPriceMaster> _OfferPriceMasterQuery;
        public offerpriceReportHandler(IRepositoryQuery<OfferPriceMaster> offerPriceMasterQuery)
        {
            _OfferPriceMasterQuery = offerPriceMasterQuery;
        }
        public async Task<ResponseResult> Handle(offerpriceReportRequest request, CancellationToken cancellationToken)
        {
            var branches = request.branches.Split(',').Select(c => int.Parse(c)).ToArray();
            var _offerPrices = _OfferPriceMasterQuery.TableNoTracking;
            var offerPrices = _offerPrices.Include(c => c.Person)
                                           .Include(c => c.salesMan)
                                           .Where(c => c.InvoiceTypeId == (int)Enums.DocumentType.OfferPrice)
                                           .Where(c => request.customerId != 0 ? c.PersonId == request.customerId : true)
                                           .Where(c => !string.IsNullOrEmpty(request.customerPhone) ? c.Person.Phone == request.customerPhone : true)
                                           .Where(c => request.priceFrom != null ? c.Net >= request.priceFrom  : true)
                                           .Where(c => request.priceTo != null ? c.Net <= request.priceTo : true)
                                           .Where(c => request.salesManId != 0 ? c.SalesManId == request.salesManId : true)
                                           .Where(c => request.branches != "0" ? branches.Contains(c.BranchId) : true)
                                           .Where(c => request.statues != OfferPriceStatues.all ? c.InvoiceSubTypesId == (int)request.statues : true)
                                           .Where(c => c.InvoiceDate.Date >= request.dateFrom.Date && c.InvoiceDate.Date <= request.dateTo.Date).OrderBy(o=>o.InvoiceDate);
            var data = request.isPrint == false || request.isPrint == null ? offerPrices.Skip((int)((request.PageNumber-1)*request.PageSize)).Take((int)request.PageSize):offerPrices;
            var res = data.Select(c => new OfferPriceReportResponse
            {
                offerPriceDate = request.isPrint ==true ? c.InvoiceDate.ToString("dd/MM/yyyy") : c.InvoiceDate.ToString(defultData.datetimeFormat),
                code = c.InvoiceType,
                arabicName = c.Person.ArabicName,
                latinName = c.Person.LatinName,
                Phone = c.Person.Phone,
                email = c.Person.Email,
                salesmanNameAr = c.salesMan.ArabicName,
                salesmanNameEn = c.salesMan.LatinName,
                statues = c.InvoiceSubTypesId,
                totalPrice = c.Net,
                InvoiceTypeId = c.InvoiceTypeId,
                InvoiceId = c.InvoiceId,
                PageId = DocHelpers.GetPageIdOfDoc(c.InvoiceTypeId),
                PageEnum = DocHelpers.GetPageEnum(c.InvoiceTypeId),
                DocTypeId = (int)PersonAccountDataDocTypeId.offerPrice,
                isCustomer = c.Person.IsCustomer

            });

            return new ResponseResult
            {
                Data = res,
                DataCount = offerPrices.Count(),
                TotalCount = _offerPrices.Count(),
                Result = Result.Success,
                Note= request.PageNumber * request.PageSize >= offerPrices.Count()? Actions.EndOfData:""



            };
        }
    }
}
