using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Stores;
using App.Domain.Models.Security.Authentication.Response.Store.Reports.Stores;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.StoreReports.Invoices.Stores.Detailed_trans_of_item
{
    public class DetailedTransOfItemService : IDetailedTransOfItemService
    {
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> ItemUnitQuery;

        public DetailedTransOfItemService(IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
            IRepositoryQuery<InvStpItemCardUnit> ItemUnitQuery)
        {
            this.InvoiceDetailsQuery = InvoiceDetailsQuery;
            this.ItemUnitQuery = ItemUnitQuery;
        }

        public async Task<ResponseResult> DetailedTransactoinsOfItem(DetailedTransOfItemRequest request)
        {
            var selectedFactor_ = ItemUnitQuery.TableNoTracking.Where(a => a.ItemId == request.itemId && a.UnitId == request.unitId)
                              .Select(a => a.ConversionFactor);
            var selectedFactor = Convert.ToDouble(selectedFactor_.ToList()[0]);

            var prevData = InvoiceDetailsQuery.TableNoTracking.Include(a => a.InvoicesMaster)
                .Where(a => a.ItemId == request.itemId && a.UnitId == request.unitId
                      && a.InvoicesMaster.StoreId == request.storeId
                      && a.InvoicesMaster.InvoiceDate.Date < request.dateFrom.Date
                     ).ToList()
                      .GroupBy(a => new { a.InvoiceId })
                      .Select(a => new DetailedTransOfItemResponse()
                      {
                          outComingQuantity = (a.First().Signal < 0 ? a.Sum(a => a.Quantity * a.ConversionFactor / selectedFactor) : 0),
                          inComingQuantity = (a.First().Signal > 0 ? a.Sum(a => a.Quantity * a.ConversionFactor / selectedFactor) : 0)

                      }).ToList();

            double totalBalance = 0;
            var resDataList = new List<DetailedTransOfItemResponse>();

            if (prevData.Count() > 0)
            {
                var prev = new DetailedTransOfItemResponse();
                totalBalance += prevData.Sum(a => a.inComingQuantity - a.outComingQuantity);
                prev.totalBalance = totalBalance;
                resDataList.Add(prev);
            }

            var resData = InvoiceDetailsQuery.TableNoTracking.Include(a => a.InvoicesMaster)
                .Where(a => a.ItemId == request.itemId && a.UnitId == request.unitId
                      && a.InvoicesMaster.StoreId == request.storeId
                      && (a.InvoicesMaster.InvoiceDate.Date >= request.dateFrom.Date
                      && a.InvoicesMaster.InvoiceDate.Date <= request.dateTo.Date)).ToList()
                      .OrderBy(a => a.InvoicesMaster.InvoiceDate.Date).ThenBy(a => a.InvoicesMaster.Serialize)
                      .GroupBy(a => new { a.InvoiceId })
                      .Select(a => new DetailedTransOfItemResponse()
                      {
                          invoiceId = a.First().InvoiceId,
                          transDate = a.First().InvoicesMaster.InvoiceDate,
                          invoiceTypeId = a.First().InvoicesMaster.InvoiceTypeId,
                          documentId = a.First().InvoicesMaster.InvoiceType,
                          notes = a.First().InvoicesMaster.Notes,
                          outComingQuantity = (a.First().Signal < 0 ? a.Sum(a => a.Quantity * a.ConversionFactor / selectedFactor) : 0),
                          inComingQuantity = (a.First().Signal > 0 ? a.Sum(a => a.Quantity * a.ConversionFactor / selectedFactor) : 0)

                      }).ToList();


            for (int i = 0; i < resData.Count(); i++)
            {
                totalBalance += resData[i].inComingQuantity - resData[i].outComingQuantity;
                resData[i].totalBalance = totalBalance;
            }
            resDataList.AddRange(resData);
            return new ResponseResult { Data = resDataList, DataCount=resData.Count() 
                              , Result = resData.Count()>0?  Result.Success : Result.NoDataFound};
        }
    }
}
