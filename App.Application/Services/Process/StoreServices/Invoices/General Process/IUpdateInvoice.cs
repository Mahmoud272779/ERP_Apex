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
    public interface IUpdateInvoice
    {
        Task<ResponseResult> UpdateInvoices(UpdateInvoiceMasterRequest parameter,  InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice,  int invoiceType, string InvoiceTypeName,string fileDirectory);
    }
}
