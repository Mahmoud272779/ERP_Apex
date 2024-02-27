using App.Domain.Entities.POS;
using App.Domain.Entities.Process;
using App.Domain.Models.Request.POS;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.POS
{
    public interface IAddSuspensionInvoice
    {
        Task<ResponseResult> SaveSuspensionInvoice(InvoiceSuspensionRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice, int invoiceType, string InvoiceTypeName, int MainInvoiceId);
        Task<POSInvSuspensionDetails> SuspensionInvItemDetails(POSInvoiceSuspension invoice, InvoiceDetailsRequest item, int setDecimal, int? ParentInvoiceId);
    }
}
