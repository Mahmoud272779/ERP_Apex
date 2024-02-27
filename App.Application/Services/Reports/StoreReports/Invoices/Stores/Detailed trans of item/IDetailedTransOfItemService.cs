using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Stores;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.Invoices.Stores.Detailed_trans_of_item
{
    public interface IDetailedTransOfItemService
    {
        Task<ResponseResult> DetailedTransactoinsOfItem(DetailedTransOfItemRequest request);
    }
}
