using App.Application.Handlers.Setup.ItemCard.Query.FillItemCardQuery;
using App.Domain.Models.Request.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface IPOSTouchService
    {
        Task<ResponseResult> getCategoriesOfPOS();
        Task<ResponseResult> getItemsOfPOS(int categoryID, int PageNumber, int PageSize ,string? itemName, int? PersonId, int? PriceListId);
        Task<ResponseResult> GetPOSTouchSettings();
        Task<ResponseResult> UpdatePOSTouchSettings(POSTouchRequest request);
        Task<ResponseResult> getItemsOfPOSIOS(int categoryID, string itemName, int PageNumber, int lastId, int PageSize, int? PriceListId, int? PersonId);
        Task<ResponseResult> FillItemForPOSIOS(FillItemCardQueryRequest request);
    }
}
