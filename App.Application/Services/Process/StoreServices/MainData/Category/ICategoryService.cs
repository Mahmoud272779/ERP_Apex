using App.Application.Handlers.Categories;

namespace App.Application.Services.Process.Category
{

    public interface ICategoryService
    {
        Task<ResponseResult> AddCategory(AddCategoryRequest parameter);
        Task<ResponseResult> GetListOfCategories(GetListOfCategoriesRequest parameters);
        Task<ResponseResult> GetCategoriesById(int Id);
        Task<ResponseResult> UpdateCategories(UpdateCategoriesRequest parameters);
        Task<ResponseResult> UpdateStatus(UpdateStatusRequest parameters);
        Task<ResponseResult> DeleteCategories(DeleteCategoriesRequest ListCode);
        Task<ResponseResult> GetCategoryHistory(int CategoryId);
        Task<ResponseResult> GetCategoriesDropDown(GetCategoriesDropDownRequest request);
        Task<ResponseResult> GetAllCategoriesDropDown();
        Task<ResponseResult> GetCategoriesByDate(GetCategoriesByDateRequest parameters);

    }
}
