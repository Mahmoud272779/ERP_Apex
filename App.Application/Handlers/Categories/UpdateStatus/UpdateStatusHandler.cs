using App.Application.Helpers.Service_helper.History;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Categories
{
    public class UpdateStatusHandler : IRequestHandler<UpdateStatusRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvCategories> CategoriesRepositoryQuery;
        private readonly IRepositoryCommand<InvCategories> CategoriesRepositoryCommand;
        private readonly IHistory<InvCategoriesHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public UpdateStatusHandler(ISystemHistoryLogsService systemHistoryLogsService, IHistory<InvCategoriesHistory> history, IRepositoryCommand<InvCategories> categoriesRepositoryCommand, IRepositoryQuery<InvCategories> categoriesRepositoryQuery)
        {
            _systemHistoryLogsService = systemHistoryLogsService;
            this.history = history;
            CategoriesRepositoryCommand = categoriesRepositoryCommand;
            CategoriesRepositoryQuery = categoriesRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(UpdateStatusRequest parameters, CancellationToken cancellationToken)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            var categories = CategoriesRepositoryQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.Id));
            var CategoryList = categories.ToList();

            CategoryList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            if (parameters.Id.Contains(1))
                CategoryList.Where(q => q.Id == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();


            var result = await CategoriesRepositoryCommand.UpdateAsyn(CategoryList);

            foreach (var Category in CategoryList)
            {
                history.AddHistory(Category.Id, Category.LatinName, Category.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            }
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editItemCategories);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }
    }
}
