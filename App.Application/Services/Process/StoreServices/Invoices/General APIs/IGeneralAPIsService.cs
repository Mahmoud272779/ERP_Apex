using App.Application.Services.HelperService.EmailServices;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Process;
using App.Domain.Models;
using App.Domain.Models.Common;
using App.Domain.Models.Request.General;
using App.Domain.Models.Response.Store.Invoices;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Request.Store.Invoices;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials.SerialsService;
using static App.Application.Services.Reports.Items_Prices.Rpt_Store;

namespace App.Application.Services.Process.Invoices.General_APIs
{
    public interface IGeneralAPIsService
    {
         Task<QuantityInStoreAndInvoice> CalculateItemQuantity(int ItemId, int UnitId, int StoreId, string ParentInvoiceType, DateTime? ExpiryDate, bool IsExpiared,int invoiceId ,DateTime invoiceDate, int? invoiceTypeId, List<CalcQuantityRequest> items);

        Task<serialsReponse> CheckSerials(serialsRequest request);
        serialsReponse CheckSerial(serialsRequest request, bool fromInvoice);
        Task<ResponseResult> GetItemsDropDown(DropDownRequest request);

        Task<ResponseResult> UpdateCanDeleteInItemUnits(int itemId, int? UnitId, bool delete);
        Task<ResponseResult> GetItemUnitsDropDown(int itemId, string? barcode);

        Task<ResponseResult> GetItemsInPartsDropDown();
        QuantityInStoreAndInvoice CalcItemQuantity(int? invoiceId, int itemId, int? unitId , int StoreId , string ParentInvoiceType,DateTime? ExpiryDate, bool IsExpiared,int? invoiceTypeId, DateTime invoiceDate, List<CalcQuantityRequest> items, int itemTypeId);
        List<InvoiceDetailsRequest> setCompositItem(int itemId, int unitId, double qty);
        List<InvoiceDetailsRequest> setCompositItem(List<CompositeItemsRequest> compositeRequest, List<compositItem> compositItems);
        List<compositItem> GetComponentsOfCompositItem(List<CompositeItemsRequest> compositeRequest);
        Task<ResponseResult> mergeTotalItems(List<InvoiceDetailsRequest> list, int invoiceType);
        Task<List<InvoiceDetailsRequest>> MergeItems(List<InvoiceDetailsRequest> list , int invoiceType);


        double CreateSerializeOfInvoice(int invoiceType, int invoiceCode, int branchId );
        bool addSerialize(double serialize, int MainInvoiceId, int invoiceType, int branchId);
   //     Task<ResponseResult> FillItemCardQuery(FillItemCardRequest request);
        Task<ResponseResult> CheckInvoiceExistance(string invoiceType, int InvoiceTypeId);
        // use in return without invoice in automatic extract expiareDate mode
        Task<ResponseResult> SetQuantityForExpiaryDate(SetQuantityForExpiaryDateRequest request);
        Task<bool> generateEditedItems(List<InvoiceDetails> invoiceDetailsList, double serialize, bool isUpdate, int invoiceId, int branchId);
        Task<ResponseResult> GetTotalAmountOfPerson(int? personId, int? FinancialAccountId);
        int GetSignal(int invoiceTypeId);
        Task<ResponseResult> ValidationOfInvoices(InvoiceMasterRequest parameter, int invoiceTypeId, InvGeneralSettings setting ,int currentBranchId,bool rejectedTransfer, int[] userStors, DateTime invoiceDate, bool isUpdate);
        Task<ResponseResult> setPOSstartup(int invoiceTypeId);
        Task<ResponseResult> NavigationStep(int invoiceTypeId, int invoiceCode, bool NextCode);

        Task<Tuple<bool, string,string>> checkQuantityBeforeSaveInvoiceForExtract(List<InvoiceDetailsRequest> invoiceDetails, int storeId, DateTime invoiceDate, int invoiceId, InvGeneralSettings? setting, int invoiceTypeId, int signal, int oldStoreId, bool isExpired,string invoiceType);
        Task<Tuple<bool, string,string>> checkQuantityBeforeSaveInvoiceForAdd(List<InvoiceDetailsRequest> invoiceDetails, int storeId, DateTime invoiceDate, int invoiceId, InvGeneralSettings? setting, int invoiceTypeId, int signal, int newStoreId, bool RejectedTransfer);
        Task<ResponseResult> GetSystemHistoryLogs(int pageNumber, int pageSize);
        Task<ResponseResult> SendingEmail(emailRequest parm);
        Task<ResponseResult> getPersonEmail(int personId);
        Task<bool> checkDeleteOfInvoice(int InvoiceTypeId, bool IsAccredite, bool IsReturn, bool IsDeleted
                , string InvoiceType, int StoreId, DateTime InvoiceDate, int InvoiceId );

        public DateTime serverDate(DateTime Dt);

        public Task<ResponseResult> GetItemsDropDownForReports(dropdownListOfItemsForReports parm);
        public Task<ResponseResult> GetStoresDropDownForReports(dropdownListOfItemsForReports parm);
        public Task<int> AddPrintFiles();
        public Task<int> UpdatePrintFiles();
        public bool checkitemsFromOutgoingTransfer(List<InvoiceDetailsRequest> invoiceDetails, ICollection<InvoiceDetails> outgoingDetails);
        public bool CompareSerialsWithMainInvoice(List<InvoiceDetailsRequest> requestInvoiceDetails,string parentInvoiceType, int invoiceTypeId);
       public  Task<List<string>> serialIsBinded(string itemCode, List<string>? serialsRequest, string invoiceType);
       public TransferDesc GetTransferDesc(int invoicesTypeID, int storeFrom, int? storeto, bool isRejected);
        public Tuple<bool, ResponseResult> userHasSession(int? sessionId);
        public Task<ResponseResult> Pagination<T>(List<T> resData, int pageNumber, int pageSize);

    }
}
