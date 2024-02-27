using App.Application.Handlers.Categories;
using MediatR;

namespace App.Application.Services.Process.Category
{

    public class CategoryService : BaseClass, ICategoryService
    {

        private readonly IMediator _mediator;

        public CategoryService(IHttpContextAccessor _httpContext,IMediator mediator) : base(_httpContext)
        {
            _mediator = mediator;
        }
        public async Task<ResponseResult> AddCategory(AddCategoryRequest parameter)
        {
            return await _mediator.Send(parameter);
        }
        public async Task<ResponseResult> GetListOfCategories(GetListOfCategoriesRequest parameters)
        {
            return await _mediator.Send(parameters);

        }
        public async Task<ResponseResult> GetCategoriesById(int Id)
        {
            return await _mediator.Send(new GetCategoriesByIdRequest { Id = Id });
        }
        public async Task<ResponseResult> UpdateCategories(UpdateCategoriesRequest parameters)
        {
            return await _mediator.Send(parameters);
        }
        public async Task<ResponseResult> UpdateStatus(UpdateStatusRequest parameters)
        {
            return await _mediator.Send(parameters);

        }
        public async Task<ResponseResult> DeleteCategories(DeleteCategoriesRequest ListCode)
        {
            return await _mediator.Send(ListCode);
        }
        public async Task<ResponseResult> GetCategoryHistory(int CategoryId)
        {
            return await _mediator.Send(new GetCategoryHistoryRequest { CategoryId =  CategoryId });    
        }
        public async Task<ResponseResult> GetCategoriesDropDown(GetCategoriesDropDownRequest request)
        {
            return await _mediator.Send(request);

        }
        public async Task<ResponseResult> GetAllCategoriesDropDown()
        {
            return await _mediator.Send(new GetAllCategoriesDropDownRequest());

        }
        public async Task<ResponseResult> GetCategoriesByDate(GetCategoriesByDateRequest parameter)
        {
            return await _mediator.Send(parameter);

        }
    }
}

