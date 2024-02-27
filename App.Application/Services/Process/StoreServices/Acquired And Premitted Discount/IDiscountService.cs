using System.Threading.Tasks;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using FastReport.Web;

namespace App.Application.Services.Acquired_And_Premitted_Discount
{
    public interface IDiscountService
    {
        Task<ResponseResult> AddDiscount(DiscountRequest parameter);
        Task<ResponseResult> UpdateDiscount(UpdateDiscountRequest parameter);
        Task<ResponseResult> GetListOfDiscounts(DiscountSearch parameter);
        Task<ResponseResult> DeleteDiscount(SharedRequestDTOs.Delete ListCode);
        Task<ResponseResult> GetDiscountHistory(int DiscountId);
        Task<WebReport> DiscountsReport(int documentId, int documentType, exportType exportType, bool isArabic, int fileId = 0);
        Task<ResponseResult> GetDiscountById(int Id);
        Task UpdateJEntery(UpdateDiscountRequest parameter, UserInformationModel userInformation, int DocCode, int DocId);
    }
}
