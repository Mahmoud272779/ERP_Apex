using App.Domain.Models.Request.POS;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.POS
{
    public interface IPOSInvSuspensionService
    {
        Task<ResponseResult> AddSuspensionInvoice(InvoiceSuspensionRequest parameter); //InvoiceSuspensionRequest
        Task<ResponseResult> GetSuspensionInvoice(int? PageNumber, int? PageSize);
        Task<ResponseResult> GetSuspensionInvoiceById(int Id);
    }
}
