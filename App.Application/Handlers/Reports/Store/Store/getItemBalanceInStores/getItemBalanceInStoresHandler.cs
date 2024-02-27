using App.Application.Helpers.Service_helper.Item_unit;
using App.Domain.Models.Security.Authentication.Response.Store.Reports.Stores;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.Store.Store.getItemBalanceInStores
{
    public class getItemBalanceInStoresHandler : IRequestHandler<getItemBalanceInStoresRequest, ResponseResult>
    {
        public IitemUnitHelperServices itemUnitHelperServices;
        readonly private IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery;

        public getItemBalanceInStoresHandler(IitemUnitHelperServices itemUnitHelperServices, IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery)
        {
            this.itemUnitHelperServices = itemUnitHelperServices;
            this.invoiceDetailsQuery = invoiceDetailsQuery;
        }
        public async Task<ResponseResult> Handle(getItemBalanceInStoresRequest request, CancellationToken cancellationToken)
        {
            var unitOfReport = await itemUnitHelperServices.getRptUnitData(request.itemId);

            var resData = invoiceDetailsQuery.TableNoTracking.Include(a => a.Units).Include(a => a.InvoicesMaster.store)
                .Where(a => a.ItemId == request.itemId).ToList().OrderBy(a => a.InvoicesMaster.StoreId)
                .GroupBy(a => a.InvoicesMaster.StoreId).Select(a => new itemBalanceInStoresResponse()
                {
                    storeId = a.First().InvoicesMaster.StoreId,
                    storeName = a.First().InvoicesMaster.store.ArabicName,
                    storeNameEn = a.First().InvoicesMaster.store.LatinName,
                    unitName = unitOfReport.rptUnitAR,
                    unitNameEn = unitOfReport.rptUnitEn,
                    balance = a.Sum(a => (a.Quantity * a.ConversionFactor * a.Signal) / unitOfReport.rptFactor)
                });





            return new ResponseResult { Data = resData, DataCount = resData.Count(), Result = resData.Count() > 0 ? Result.Success : Result.NoDataFound };
        }
    }
}
