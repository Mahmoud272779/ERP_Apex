using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials
{
    public interface ISerialsService
    {
        Task<string> DeleteSerialsForExtractInvoice(string ParentInvoice, string AddedInvoice, int storeId,int? invoiceTypeId);
        Task<bool> DeleteSerialsForAddedInvoice(string ParentInvoice, string ExtractedInvoice);
         Task<string> AddSerialsForAddedInvoice(string addedInvoice, List<InvoiceDetailsRequest> itemsWithSerialsFromRequest, string extractedInvoice, int storeId,int invoiceTypeId, int transStatusOfSerial);
        Task<Tuple<bool, string,string>> AddSerialsForExtractInvoice(bool isUpdateOrDelete, List<InvoiceDetailsRequest> itemsWithSerialsFromRequest, string extractedInvoice, int? transferStatus);
        bool SerialsExist(int invoiceTypeId, string invoiceType, List<InvoiceDetailsRequest> InvoicesDetails);
        Task<ResponseResult> checkSerialBeforeSave(bool isUpdate, string? invoiceType, List<InvoiceDetailsRequest>? invoiceDetails, int invoicceTypeId, int storeId, int oldStore);
    }
}
