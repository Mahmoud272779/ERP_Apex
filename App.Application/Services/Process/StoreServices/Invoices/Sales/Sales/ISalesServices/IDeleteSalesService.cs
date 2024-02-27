using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices
{
    public interface IDeleteSalesService
    {
        Task<ResponseResult> DeleteSales(SharedRequestDTOs.Delete parameter);
        Task<ResponseResult> DeleteInvoiceForPOS(SharedRequestDTOs.Delete parameter);
    }
}
