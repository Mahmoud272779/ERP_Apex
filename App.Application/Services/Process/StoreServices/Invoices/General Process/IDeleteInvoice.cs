using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Invoices.General_Process
{
    public interface IDeleteInvoice
    {
        Task<ResponseResult> DeleteInvoices(int Ids, int invoiceTypeId, string InvoiceTypeName, int MaininvoiceTytpe);
       // Task<ResponseResult> DeleteInvoices(int Ids, int invoiceType, string InvoiceTypeName);
    }
}
