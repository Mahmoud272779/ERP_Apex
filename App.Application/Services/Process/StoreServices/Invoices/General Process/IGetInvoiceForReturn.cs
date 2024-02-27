using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Invoices.General_Process
{
    public interface IGetInvoiceForReturn
    {
        Task<ResponseResult> GetMainInvoiceForReturn(string InvoiceCode, int InvoiceTypeId);
        Task<ResponseResult> GetMainInvoiceForReturn(string InvoiceCode, int InvoiceTypeId, bool? ForIOS);
    }
}
