using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface  IAddSalesService
    {
        Task<ResponseResult> AddSales(InvoiceMasterRequest parameter);
        Task<ResponseResult> AddInvoiceForPOS(InvoiceMasterRequest parameter);
        Task<ResponseResult> AddOfferPrice(InvoiceMasterRequest parameter);


    }
}
