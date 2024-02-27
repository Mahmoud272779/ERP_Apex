using App.Application.Helpers.Service_helper.FileHandler;
using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.Process.Restaurants;
using App.Domain.Entities.Setup;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Threading;

namespace App.Application.Handlers.Categories
{
    public class UpdateCategoriesHandler : IRequestHandler<UpdateCategoriesRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvCategories> CategoriesRepositoryQuery;
        private readonly IRepositoryCommand<InvStpItemCardMaster> _InvStpItemCardMasterCommand;
        private readonly IRepositoryCommand<CategoriesPrinters> categoriesPrintersCommand;
        private readonly IRepositoryQuery<CategoriesPrinters> categoriesPrintersQuery;
        private readonly IFileHandler fileHandler;
        private readonly IRepositoryCommand<InvCategories> CategoriesRepositoryCommand;
        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<InvCategoriesHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public UpdateCategoriesHandler(ISystemHistoryLogsService systemHistoryLogsService,
            IHistory<InvCategoriesHistory> history,
            IHttpContextAccessor httpContext,
            IRepositoryCommand<InvCategories> categoriesRepositoryCommand,
            IFileHandler fileHandler,
            IRepositoryQuery<InvCategories> categoriesRepositoryQuery,
            IRepositoryCommand<InvStpItemCardMaster> invStpItemCardMasterCommand,
            IRepositoryCommand<CategoriesPrinters> categoriesPrintersCommand,
            IRepositoryQuery<CategoriesPrinters> categoriesPrintersQuery)
        {
            _systemHistoryLogsService = systemHistoryLogsService;
            this.history = history;
            this.httpContext = httpContext;
            CategoriesRepositoryCommand = categoriesRepositoryCommand;
            this.fileHandler = fileHandler;
            CategoriesRepositoryQuery = categoriesRepositoryQuery;
            _InvStpItemCardMasterCommand = invStpItemCardMasterCommand;
            this.categoriesPrintersCommand = categoriesPrintersCommand;
            this.categoriesPrintersQuery = categoriesPrintersQuery;
        }

        public async Task<ResponseResult> Handle(UpdateCategoriesRequest parameters, CancellationToken cancellationToken)
        {
            if (parameters.Id == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            parameters.LatinName = Helpers.Helpers.IsNullString(parameters.LatinName);
            parameters.ArabicName = Helpers.Helpers.IsNullString(parameters.ArabicName);
            if (string.IsNullOrEmpty(parameters.LatinName))
                parameters.LatinName = parameters.ArabicName;

            if (string.IsNullOrEmpty(parameters.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }

            var ArabicCategoriesExist = await CategoriesRepositoryQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName && a.Id != parameters.Id);
            if (ArabicCategoriesExist != null)
                return new ResponseResult() { Data = null, Id = ArabicCategoriesExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };

            var LatinCategoriesExist = await CategoriesRepositoryQuery.GetByAsync(a => a.LatinName == parameters.LatinName && a.Id != parameters.Id);

            if (LatinCategoriesExist != null)
                return new ResponseResult() { Data = null, Id = LatinCategoriesExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };

            var data = await CategoriesRepositoryQuery.GetByAsync(a => a.Id == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };


            data.ArabicName = parameters.ArabicName;
            data.LatinName = parameters.LatinName;
            data.VatValue = parameters.VatValue;
            data.Color = parameters.Color;
            data.Code = data.Code;
            data.CategoryType = parameters.categoryType;
            if (data.Id == 1)
                data.Status = (int)Status.Active;
            else
                data.Status = parameters.Status;
            data.Notes = parameters.Notes;
            data.UsedInSales = parameters.UsedInSales;

            if (parameters.ChangeImage == true)
            {
                data.ImagePath = fileHandler.DeleteImage(data.ImagePath);

                var img = parameters.Image;
                if (img != null)
                {
                    data.ImagePath = fileHandler.SaveImage(img, "Categories", true);

                }
            }
            else
            {
                data.ImagePath = data.ImagePath;
            }
            //Set Time
            data.UTime = DateTime.Now;

            //update CategoriesPrinter Table
            if (data.CategoryType == (int)CategoryType.kitchen)
            {

                if (parameters.kitchenId != null)
                    data.kitchenId = data.kitchenId;
                else
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = "Please Select Kitchen" };

                if (parameters.printers != null)
                {
                    await CategoriesRepositoryCommand.UpdateAsyn(data);

                    //Remove all Printer from CategoriesPrinter Table.

                    var cp = await categoriesPrintersQuery.TableNoTracking.Where(i => i.CategoryId == parameters.Id).ToListAsync();

                    categoriesPrintersCommand.RemoveRange(cp);
                    await categoriesPrintersCommand.SaveAsync();

                    //Add New list printers to CategoriesPrinter Table
                    var printersArray = parameters.printers.Split(',').Select(c => int.Parse(c)).ToArray();
                    foreach (var item in printersArray)
                    {
                        await categoriesPrintersCommand.AddAsync(new CategoriesPrinters() { PrinterId = item, CategoryId = parameters.Id });
                    }
                }
                else
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = "Please Select Printer" };
            }
            else
                await CategoriesRepositoryCommand.UpdateAsyn(data);


                await _InvStpItemCardMasterCommand.ExcuteQuery($"update InvStpItemMaster set VAT = {data.VatValue} where GroupId = {data.Id}");
            


            string browserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
            history.AddHistory(data.Id, data.LatinName, data.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editItemCategories);
            return new ResponseResult() { Data = null, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };

        }
    }
}
