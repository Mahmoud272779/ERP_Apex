using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.POS
{
    public interface IGetInvSuspensionService
    {
        Task<ResponseResult> GetSuspensionInvoices(int? PageNumber, int? PageSize);
        Task<ResponseResult> GetSuspensionInvoicesById(int Id);
    }
}
