using App.Application.Handlers.Restaurants;
using App.Application.Handlers.Restaurants.Kitchens;
using App.Application.Handlers.Restaurants.Kitchens.GetListOfKitchens;
using App.Application.Handlers.Restaurants.Kitchens.UpdateKitchens;

namespace App.Application.Services.Process.RestaurantsServices.KitchensServices
{
    public interface iKitchensServices
    {
        Task<ResponseResult> AddKitchen(AddKitchensRequest parameter);
        Task<ResponseResult> GetListOfKitchens(GetListOfKitchensRequest parameters);
        Task<ResponseResult> UpdateKitchens(UpdateKitchensRequest parameters);
        Task<ResponseResult> UpdateStatus(UpdateStatusRequest parameters);
        Task<ResponseResult> DeleteKitchens(DeleteKitchensRequest parameter);
        Task<ResponseResult> GetKitchensHistory(int Code);
        Task<ResponseResult> GetKitchensDropDown();
        //Task<ResponseResult> GetKitchensByDate(GetUnitsByDateRequest parameter);
    }
}
