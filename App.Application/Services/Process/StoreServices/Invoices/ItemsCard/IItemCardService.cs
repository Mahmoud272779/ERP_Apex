using App.Domain.Models.Setup.ItemCard.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Models.Security.Authentication.Response.Totals;

namespace App.Application.Services.Process.StoreServices.Invoices.ItemsCard
{
    public interface IItemCardService
    {
        Task<WebReport> ItemCardPrint(GetAllItemCardRequest parameter, exportType exportType, bool isArabic,int fileId=0);
        Task<ResponseResult> GetItemsByDate(DateTime date, int PageNumber, int PageSize);

    }
}
