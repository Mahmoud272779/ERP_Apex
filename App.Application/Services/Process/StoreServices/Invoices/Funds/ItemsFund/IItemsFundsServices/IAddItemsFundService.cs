using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.ItemsFund.IItemsFundsServices
{
    public interface IAddItemsFundService
    {
        Task<ResponseResult> AddItemsFund(InvoiceMasterRequest parameter);
    }
}
