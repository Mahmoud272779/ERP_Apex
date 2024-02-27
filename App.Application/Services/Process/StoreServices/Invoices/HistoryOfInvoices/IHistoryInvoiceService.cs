using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices
{
    public interface IHistoryInvoiceService
    {
        Task HistoryInvoiceMaster(int branchId, string notes, string browserName, string lastTransactionAction, string addTransactionUser, string BookIndex, int Code
              , DateTime InvoiceDate, int InvoiceId, string InvoiceType, int InvoiceTypeId, int SubType, bool IsDeleted, string ParentInvoiceCode, double Serialize, int StoreId, double TotalPrice, int collectionMainCode=0);
        Task<InvoiceMasterHistory> HistoryInvoiceMasterObjectBuilder(
            int _branchId,
            string _notes,
            string _browserName,
            string _lastTransactionAction,
            string _addTransactionUser,
            string _BookIndex,
            int _Code,
            DateTime _InvoiceDate,
            int _InvoiceId,
            string _InvoiceType,
            int _InvoiceTypeId,
            int _SubType,
            bool _IsDeleted,
            string _ParentInvoiceCode,
            double _Serialize,
            int _StoreId,
            double _TotalPrice,
            int _collectionMainCode = 0,
            int  MultiCollectionReceiptsId = 0
            );
        Task<ResponseResult> GetAllInvoiceMasterHistory(int InvoiceId , string? ParentInvoiceCode, int invoiceTypeId);
    }
}
