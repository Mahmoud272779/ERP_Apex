using App.Domain.Models.Request.POS;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface IGetPOSInvoicesService
    {
        Task<ResponseResult> GetAllPOSInvoices(POSReturnInvoiceSearchDTO request);
        Task<ResponseResult> GetPOSInvoiceById(int? InvoiceId, string? InvoiceCode,bool? ForIOS);
        Task<ResponseResult> GetPOSReturnInvoice(string InvoiceCode);
        Task<ResponseResult> GetPOSReturnInvoice(string InvoiceCode, bool? ForIOS);
        Task<ResponseResult> Navegation(int invoiceTypeId);
        Task<ResponseResult> POSNavigationStep(int invoiceTypeId, int invoiceId, int stepType, int branchId);
        Task<ResponseResult> POSNavigationStepIndex(int invoiceTypeId, int IndexId, int branchId);
    }
}
