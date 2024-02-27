using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.Items_Prices
{
    public interface IRpt_ItemsPrices
    {
        Task<ResponseResult> GetByFiltersWithPagenation(ItemsPricesRequest parameters);
    }
}
