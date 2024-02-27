using App.Domain.Entities.Setup;
using App.Domain.Models.Response.Store.Reports.Store;
using App.Domain.Models.Security.Authentication.Response.Store.Reports.Stores;
using App.Infrastructure;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace App.Application.Handlers.Reports.Store.Store.ItemsBalanceInStores
{
    public class ItemsBalanceInStoresHandler : IRequestHandler<ItemsBalanceInStoresRequest, itemBalanceInStoreResponse>
    {
        private readonly IRepositoryQuery<InvStpUnits> _invStpUnitsQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> _InvStpItemCardUnitQuery;
        readonly private IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _invStpItemCardMasterQuery;
        private readonly IRoundNumbers _roundNumbers;

 


        public ItemsBalanceInStoresHandler(IRepositoryQuery<InvStpUnits> invStpUnitsQuery, IRepositoryQuery<InvStpItemCardUnit> invStpItemCardUnitQuery, IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery, IRepositoryQuery<InvStpItemCardMaster> invStpItemCardMasterQuery, IRoundNumbers roundNumbers)
        {
            _invStpUnitsQuery = invStpUnitsQuery;
            _InvStpItemCardUnitQuery = invStpItemCardUnitQuery;
            this.invoiceDetailsQuery = invoiceDetailsQuery;
            _invStpItemCardMasterQuery = invStpItemCardMasterQuery;
            _roundNumbers = roundNumbers;
           // _httpContext = httpContext; 
            //_configuration = configuration;
        }
        public async Task<itemBalanceInStoreResponse> Handle(ItemsBalanceInStoresRequest parm, CancellationToken cancellationToken)
        {
            var units = _invStpUnitsQuery.TableNoTracking.ToList();
            var itemsUnits = _InvStpItemCardUnitQuery.TableNoTracking.ToList();
            var _invoices = invoiceDetailsQuery.TableNoTracking
                                                .Include(x => x.InvoicesMaster)
                                                .Where(x => !x.InvoicesMaster.IsDeleted)
                                                .Where(x => x.InvoicesMaster.StoreId == parm.storeId)
                                                .Where(x => parm.itemId != null ? x.ItemId == parm.itemId : true)
                                                .ToList()
                                                .GroupBy(x => x.ItemId);
            var itemsIds = _invoices.Select(c => c.Key);
            var invoices = _invStpItemCardMasterQuery.TableNoTracking
                                                     .Include(x => x.Units).Include(x=>x.Category).Include(x=>x.StorePlace)
                                                     .Where(x => parm.itemId != null ? x.Id == parm.itemId : true)
                                                     .Where(x => parm.catId != 0 ? x.GroupId == parm.catId : true)
                                                     .Where(x => parm.itemTypes != 0 ? x.TypeId == (int)parm.itemTypes : true)
                                                     .Where(x => x.TypeId != (int)ItemTypes.Note && x.TypeId!= (int)ItemTypes.Service && x.TypeId!= (int)ItemTypes.Composite)
                                                     .Where(x => itemsIds.Contains(x.Id))
                                                     .ToList()
                                                     .Select(x => new itemBalanceInStoreResponseDTO
                                                     {
                                                         itemId = x.Id,
                                                         itemCode = x.ItemCode,
                                                         arabicName = x.ArabicName,
                                                         latinName = x.LatinName,
                                                         unitId = x.ReportUnit ?? 0,
                                                         CategoryNameAr = x.Category != null ? x.Category.ArabicName : "",
                                                         CatogeryNameEn = x.Category != null ? x.Category.LatinName : "",
                                                         Status = x.Status,
                                                         StorePlaceStatus = x.StorePlace != null ? x.StorePlace.Status : 0,
                                                         StoreNameAr = x.StorePlace != null ? x.StorePlace.ArabicName : "",
                                                         StoreNameEn = x.StorePlace != null ? x.StorePlace.LatinName : "",
                                                         Model = x.Model,
                                                         NationalBarcode = x.NationalBarcode,
                                                         VAT = x.VAT,
                                                         Description = x.Description,
                                                         TypeId = x.TypeId
                                                         //unitArabicName = units.Where(c => c.Id == x.ReportUnit).FirstOrDefault().ArabicName,
                                                         //unitLatinName = units.Where(c => c.Id == x.ReportUnit).FirstOrDefault().LatinName,
                                                         //balance = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Quantity(_invoices.Where(c => c.Key == x.Id).FirstOrDefault(), x.Units.Where(c => c.UnitId == x.ReportUnit).FirstOrDefault().ConversionFactor))
                                                     });

            double MaxPageNumber = parm.PageSize != 0 ? invoices.Count() / Convert.ToDouble(parm.PageSize) : invoices.Count();
            var countofFilter = _roundNumbers.GetRoundNumber(MaxPageNumber);
            var resData = !parm.isPrint ? invoices.Skip((parm.PageNumber - 1) * parm.PageSize).Take(parm.PageSize) : invoices;
            var responseData = resData.ToList();
            responseData.ForEach(x =>
            {
                x.unitArabicName = units.Where(c => c.Id == x.unitId).FirstOrDefault().ArabicName;
                x.unitLatinName = units.Where(c => c.Id == x.unitId).FirstOrDefault().LatinName;
                x.balance = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Quantity(_invoices.Where(c => c.Key == x.itemId).FirstOrDefault(), itemsUnits.Where(c => c.ItemId == x.itemId && c.UnitId == x.unitId).FirstOrDefault().ConversionFactor));
            });
            responseData.RemoveAll(a => a.balance == 0);
            return new itemBalanceInStoreResponse()
            {
                data = responseData,
                dataCount = resData.Count(),
                notes = (countofFilter == parm.PageNumber ? Actions.EndOfData : ""),
                Result = invoices.Any() ? Result.Success : Result.NoDataFound,
                totalCount = invoices.Count()
            };


        }
    }
}
