using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService
{
    public interface IGetAllReturnPurchaseService
    {
          Task<ResponseResult> GetAllReturnPurchase(InvoiceSearchPagination Reequest);
    }
}
