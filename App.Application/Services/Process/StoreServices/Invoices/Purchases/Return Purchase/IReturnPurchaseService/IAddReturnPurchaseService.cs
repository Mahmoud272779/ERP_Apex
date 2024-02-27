using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService
{
    public interface IAddReturnPurchaseService
    {
        Task<ResponseResult> AddReturnPurchase(InvoiceMasterRequest parameter,int invoiceTypeId);
    }
}
