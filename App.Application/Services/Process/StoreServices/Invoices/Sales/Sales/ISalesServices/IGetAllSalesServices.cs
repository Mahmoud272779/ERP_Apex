using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface IGetAllSalesServices
    {
        Task<ResponseResult> GetAllSales(InvoiceSearchPagination parameter);
    }
}
