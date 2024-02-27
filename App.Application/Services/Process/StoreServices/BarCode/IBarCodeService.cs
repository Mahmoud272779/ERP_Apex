using System.Threading.Tasks;
using App.Domain.Models.Request.Barcode;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;

namespace App.Application.Services.Process.BarCode
{
    public interface IBarCodeService
    {
        Task<ResponseResult> AddBarCode(AddBarCodeRequest parameter);

        Task<ResponseResult> FillItemCardBarcode(string? itemCode, int? unitId, int? itemId, int? itemType, int? categoryId);
        Task<ResponseResult> updateBarCode(UpdateBarCodeRequest parameter);
        Task<ResponseResult> GetAllBarCode(BarcodeSearch parameter);
        Task<ResponseResult> GetBarCodeById(int BarCodeId);
        Task<ResponseResult> DeleteBarCode(SharedRequestDTOs.Delete parameter);
        Task<ResponseResult> GetAllBarCodeHistory(int BarCodeId);
        Task<ResponseResult> UpdateDefaultBarcode(DefaultBarCodeRequest barcodeId);
        Task<ResponseResult> GetBarcodeDropDown();
        Task<ResponseResult> InstalledPrinters();
        Task<WebReport> BarcodeReport(PrintItemsBarcodeRequest parameters);
    }
}
