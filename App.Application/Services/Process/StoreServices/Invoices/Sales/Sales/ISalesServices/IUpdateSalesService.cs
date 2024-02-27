using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices
{
    public  interface IUpdateSalesService
    {
        Task<ResponseResult> UpdateSales(UpdateInvoiceMasterRequest parameter);
        Task<ResponseResult> UpdateInvoiceForPOS(UpdateInvoiceMasterRequest parameter);
    }
}
