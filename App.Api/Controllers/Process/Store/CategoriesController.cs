using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers.Categories;
using App.Application.Handlers.Units;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Category;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class CategoriesController : ApiStoreControllerBase
    {
        private readonly ICategoryService CategoriesService;
        private readonly iAuthorizationService _iAuthorizationService;

        public CategoriesController(ICategoryService _CategoriesService,iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            CategoriesService = _CategoriesService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpPost(nameof(AddCategory))]
        public async Task<ResponseResult> AddCategory([FromForm] AddCategoryRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Categories_MainUnits, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await CategoriesService.AddCategory(parameter);
            return result;
        }
        [HttpGet("GetListOfCategories")]
        public async Task<ResponseResult> GetListOfCategories(int PageNumber,int PageSize,string? Name,int Status,int CategoryId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Categories_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            GetListOfCategoriesRequest paramters = new GetListOfCategoriesRequest()
            {

                PageNumber = PageNumber,
                PageSize = PageSize,
                Name=Name,
                Status=Status,
                Id= CategoryId

            };
            var result = await CategoriesService.GetListOfCategories(paramters);
            
            return result;
        }

        [HttpGet("Categories/{id}")]
        public async Task<ResponseResult> Categories(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Categories_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //return Ok(accountTree.GetById(id).Result);
            var result = await CategoriesService.GetCategoriesById(id);
            return result;
        }

        [HttpPut(nameof(UpdateCategory))]
        public async Task<ResponseResult> UpdateCategory([FromForm] UpdateCategoriesRequest parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Categories_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await CategoriesService.UpdateCategories(parameters);
            return result;
        }

        [HttpPut(nameof(UpdateActiveCategory))]
        public async Task<ResponseResult> UpdateActiveCategory(Application.Handlers.Categories.UpdateStatusRequest parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Categories_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await CategoriesService.UpdateStatus(parameters);
            return result;
        }
        [HttpDelete(nameof(DeleteCategories))]
        public async Task<ResponseResult> DeleteCategories([FromBody]int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Categories_MainUnits, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            DeleteCategoriesRequest parameter = new DeleteCategoriesRequest()
            {
                Ids = Id
            };
            var result = await CategoriesService.DeleteCategories(parameter);
            return result;

        }

        [HttpGet("GetCategoryHistory")]
        public async Task<ResponseResult> GetCategoryHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Categories_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await CategoriesService.GetCategoryHistory(Id);
            return result;
        }       
        [HttpGet("GetCategoriesDropDown")]
        public async Task<ResponseResult> GetCategoriesDropDown(string? Name , int PageSize , int PageNumber)
        {
            GetCategoriesDropDownRequest request = new GetCategoriesDropDownRequest()
            {
                SearchCriteria = Name,
                PageNumber = PageNumber,
                PageSize = PageSize
            };
            var result = await CategoriesService.GetCategoriesDropDown(request);
            return result;
        }        
        [HttpGet("GetAllCategoriesDropDown")]
        public async Task<ResponseResult> GetAllCategoriesDropDown()
        {
            var result = await CategoriesService.GetAllCategoriesDropDown();
            return result;
        }
        [HttpGet("GetCategoriesByDate")]
        public async Task<ResponseResult> GetCategoriesByDate(DateTime date, int PageNumber, int PageSize = 10)
        {
            GetCategoriesByDateRequest parameters = new GetCategoriesByDateRequest()
            {
                date = date,
                PageNumber = PageNumber,
                PageSize = PageSize

            };

            var result = await CategoriesService.GetCategoriesByDate(parameters);
            return result;
        }
    }
}
