using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Invoices.General_Process
{
    public interface IAddInvoice
    {
        Task<ResponseResult> SaveInvoice(InvoiceMasterRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice, int invoiceTypeId, string InvoiceTypeName, int MainInvoiceId, string fileDirectory, bool rejectedTransfer, int? Transferstore ,int? inCommingTransferCode, int transStatusOfSerial);

        Task<ResponseResult> SaveInvoice(InvoiceMasterRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice,  int invoiceType, string InvoiceTypeName, int MainInvoiceId, string fileDirectory,bool recjectedTransfer);
        Task<ResponseResult> SaveInvoice(InvoiceMasterRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice,  int invoiceType, string InvoiceTypeName, int MainInvoiceId, string fileDirectory,int transStatusOfSerial);
        Task<ResponseResult> SaveInvoice(InvoiceMasterRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice,  int invoiceType, string InvoiceTypeName, int MainInvoiceId, string fileDirectory);
        InvoiceDetails ItemDetails(InvoiceMaster invoice, InvoiceDetailsRequest item );//, InvoiceDetails invoiceDetails);
        List<InvoiceDetailsRequest> setCompositItem(InvoiceMaster invoice, int itemId, int unitId, int indexOfItem, double qty);
    }
}
