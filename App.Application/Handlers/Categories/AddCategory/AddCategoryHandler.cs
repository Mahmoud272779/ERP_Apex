using App.Application.Helpers.Service_helper.FileHandler;
using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.Process.Restaurants;
using App.Domain.Entities.Process.Store;
using App.Domain.Enums;
using MediatR;
using Microsoft.Net.Http.Headers;
using System.Threading;

namespace App.Application.Handlers.Categories
{
    public class AddCategoryHandler : IRequestHandler<AddCategoryRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvCategories> CategoriesRepositoryQuery;
        private readonly IRepositoryCommand<CategoriesPrinters> categoriesPrinterCommand;
        private readonly IFileHandler fileHandler;
        private readonly IRepositoryCommand<InvCategories> CategoriesRepositoryCommand;
        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<InvCategoriesHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public AddCategoryHandler(ISystemHistoryLogsService systemHistoryLogsService
            , IHistory<InvCategoriesHistory> history
            , IHttpContextAccessor httpContext
            , IRepositoryCommand<InvCategories> categoriesRepositoryCommand
            , IFileHandler fileHandler
            , IRepositoryQuery<InvCategories> categoriesRepositoryQuery
            ,IRepositoryCommand<CategoriesPrinters> categoriesPrinterCommand)
        {
            _systemHistoryLogsService = systemHistoryLogsService;
            this.history = history;
            this.httpContext = httpContext;
            CategoriesRepositoryCommand = categoriesRepositoryCommand;
            this.fileHandler = fileHandler;
            CategoriesRepositoryQuery = categoriesRepositoryQuery;
            this.categoriesPrinterCommand = categoriesPrinterCommand;
        }

        public async Task<ResponseResult> Handle(AddCategoryRequest parameter, CancellationToken cancellationToken)
        {
            parameter.latinName = Helpers.Helpers.IsNullString(parameter.latinName);
            parameter.arabicName = Helpers.Helpers.IsNullString(parameter.arabicName);
            if (string.IsNullOrEmpty(parameter.latinName))
                parameter.latinName = parameter.arabicName;
            if (string.IsNullOrEmpty(parameter.arabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };
            if (parameter.status < (int)Status.Active || parameter.status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            var ArabicCategoryExist = await CategoriesRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.arabicName);
            if (ArabicCategoryExist != null)
                return new ResponseResult() { Data = null, Id = ArabicCategoryExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };
            var LatinCategoryExist = await CategoriesRepositoryQuery.GetByAsync(a => a.LatinName == parameter.latinName);
            if (LatinCategoryExist != null)
                return new ResponseResult() { Data = null, Id = LatinCategoryExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };
            int NextCode = CategoriesRepositoryQuery.GetMaxCode(e => e.Code) + 1;
            var table = new InvCategories();
            table.ArabicName = parameter.arabicName;
            table.LatinName = parameter.latinName;
            table.VatValue = parameter.vatValue;
            table.Color = parameter.color;
            table.Status = parameter.status;
            table.Notes = parameter.notes;
            table.UsedInSales = parameter.usedInSales;
            table.Code = NextCode;
            table.UTime = DateTime.Now; // Set Time
            var img = parameter.image;
            if (img != null)
            {
                table.ImagePath = fileHandler.SaveImage(img, "Categories", true);
            }
            table.CategoryType = parameter.categoryType; 
            

            if(table.CategoryType == (int)CategoryType.kitchen)
            {   

                if(parameter.kitchenId != null)
                    table.kitchenId = parameter.kitchenId;
                else
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = "Please Select Kitchen" };

                if (parameter.printers != null)
                {
                    CategoriesRepositoryCommand.Add(table);

                    var id = (await CategoriesRepositoryQuery.FindByAsyn(e => e.Code == table.Code)).FirstOrDefault().Id;
                    var printersArray = parameter.printers.Split(',').Select(c => int.Parse(c)).ToArray();
                    foreach (var item in printersArray)
                    {
                        await categoriesPrinterCommand.AddAsync(new CategoriesPrinters() { PrinterId = item, CategoryId = id });
                    }
                }
                else
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = "Please Select Printer" };
            }
            else
                CategoriesRepositoryCommand.Add(table);

            string browserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addItemCategories);
            return new ResponseResult() { Data = null, Id = table.Id, Result = Result.Success };
        }
    }
}
