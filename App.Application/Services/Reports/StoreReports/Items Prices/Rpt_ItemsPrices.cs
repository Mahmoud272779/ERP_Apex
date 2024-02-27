using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.Items_Prices
{
   public class Rpt_ItemsPrices : BaseClass, IRpt_ItemsPrices
    {


      
         private readonly IRepositoryQuery<InvStpItemCardUnit> ItemCardUnitRepositoryQuery;
    
        private readonly IHttpContextAccessor httpContext;

        public Rpt_ItemsPrices(IRepositoryQuery<InvStpItemCardUnit> _ItemCardUnitRepositoryQuery,
                                   IHttpContextAccessor _httpContext) : base(_httpContext)
        {
           
            ItemCardUnitRepositoryQuery = _ItemCardUnitRepositoryQuery;
            httpContext = _httpContext;
        }




        public async Task<ResponseResult> GetByFiltersWithPagenation(ItemsPricesRequest parameters)
        {
          

            var data = await ItemCardUnitRepositoryQuery.GetAllIncludingAsync(parameters.PageNumber, parameters.PageSize
                                             , a => ( 1==1 && a.Item.ReportUnit == a.UnitId &&
                                             (string.IsNullOrEmpty(parameters.ItemCode)|| a.Item.ItemCode.Contains(parameters.ItemCode)) &&
                                             (string.IsNullOrEmpty(parameters.ItemName) || a.Item.ArabicName.Contains(parameters.ItemName) || a.Item.LatinName.Contains(parameters.ItemName)) &&
                                             (parameters.Status == 0 || a.Item.Status == parameters.Status) &&
                                             (parameters.TypeId == 0 || a.Item.TypeId == parameters.TypeId) &&
                                             (parameters.CategoryId == 0 || a.Item.GroupId == parameters.CategoryId))
                                              , e => e.Item, X => X.Unit);


             return new ResponseResult() { Data = data, Id = null, Result = data.Any() ? Result.Success : Result.Failed };

        }

        
    }
}
