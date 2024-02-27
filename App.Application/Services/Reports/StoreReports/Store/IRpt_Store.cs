using App.Domain.Enums;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Response.Store.Reports.Stores;
using App.Domain.Models.Response.General;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastReport.Web;
using App.Domain.Models.Response.Store.Reports.Store;

namespace App.Application.Services.Reports.Items_Prices
{
    public interface IRpt_Store
    {
        public Task<ResponseResult> DetailedMovementOfanItem(DetailedMovementOfanItemReqest param,bool isPrint =false);
        public Task<ResponseResult> InventoryValuation(InventoryValuationRequest param, bool isPrint = false);
        public Task<InventoryValuationResponse> getInventoryValuation(InventoryValuationRequest param, bool isPrint = false);

        public Task<TotalTransactionOfItemsResponse> getTotalTransactionOfItems(TotalTransactionOfItemsRequestDTO parm, bool isPrint = false);
        public Task<ResponseResult> TotalTransactionOfItems(TotalTransactionOfItemsRequestDTO parm);
        public Task<List<ExpiredItemsReportResponseDTO>> GetDetailsOfExpiredItems(ExpiredItemsReportRequestDTO parm, bool isPrint = false);
        public Task<ResponseResult> DetailsOfExpiredItems(ExpiredItemsReportRequestDTO parm);

        
        
        public Task<DemandLimitResponse> getDemandLimit(DemandLimitRequestDTO parm, bool isPrint = false);
        public Task<ResponseResult> DemandLimit(DemandLimitRequestDTO parm);

        public Task<ReviewWarehouseTransfersResponse> GetReviewWarehouseTransfers(ReviewWarehouseTransfersRequest parm,bool isPrint = false);
        public Task<ResponseResult> ReviewWarehouseTransfers(ReviewWarehouseTransfersRequest parm);


        public Task<DetailedTransferReportResponse> GetDetailedTransferReport(DetailedTransferReportRequest parm, bool isPrint = false);
        public Task<ResponseResult> DetailedTransferReport(DetailedTransferReportRequest parm);

        Task<WebReport> DetailedMovementOfanItemReport(DetailedMovementOfanItemReqest param, exportType exportType, bool isArabic, int fileId = 0);

        Task<WebReport> InventoryValuationReport(InventoryValuationRequest param, exportType exportType, bool isArabic, int fileId = 0);

        Task<WebReport> ReviewWarehouseTransfersReport(ReviewWarehouseTransfersRequest parm, exportType exportType, bool isArabic, int fileId = 0);
        Task<WebReport> DetailedTransferPrint(DetailedTransferReportRequest parm, exportType exportType, bool isArabic, int fileId = 0);

        Task<WebReport> DemandLimitReport(DemandLimitRequestDTO parm, exportType exportType, bool isArabic, int fileId = 0);

        Task<WebReport> DetailsOfExpiredItemsReport(ExpiredItemsReportRequestDTO parm, exportType exportType, bool isArabic, int fileId = 0);


    }
}
