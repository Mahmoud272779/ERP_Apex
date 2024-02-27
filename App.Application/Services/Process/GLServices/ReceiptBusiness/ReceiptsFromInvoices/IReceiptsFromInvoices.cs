using App.Domain.Entities.Process;
using App.Domain.Models.Request.Store.Invoices;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Application.Services
{
    public interface IReceiptsFromInvoices
    {
        Task<ResponseResult> updateReceiptsFromInvoices(InvoiceMaster Invoice, List<InvoicePaymentsMethods> InvPaymentMethod);
        Task<ResponseResult> AddReceiptsFromInvoices(InvoiceMaster Invoice, List<InvoicePaymentsMethods> InvPaymentMethod);
        Task<ResponseResult> DeleteInvoicesReceipts(List<int> InvoiceIds);


    }
}
