using App.Application.Helpers.Service_helper.FileHandler;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Domain.Entities.Process.Restaurants;
using App.Domain.Entities.Process.Store;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Categories
{
    public class DeleteCategoriesHandler : IRequestHandler<DeleteCategoriesRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvCategories> CategoriesRepositoryQuery;
        private readonly IRepositoryCommand<DeletedRecords> _deletedRecordCommand;
        private readonly IDeletedRecordsServices _deletedRecords;
        private readonly IRepositoryCommand<CategoriesPrinters> categoriesPrintersCommand;
        private readonly IRepositoryQuery<CategoriesPrinters> categoriesPrintersQuery;
        private readonly IFileHandler fileHandler;
        private readonly IRepositoryCommand<InvCategories> CategoriesRepositoryCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public DeleteCategoriesHandler(ISystemHistoryLogsService systemHistoryLogsService,
            IRepositoryCommand<InvCategories> categoriesRepositoryCommand,
            IFileHandler fileHandler,
            IRepositoryQuery<InvCategories> categoriesRepositoryQuery,
            IRepositoryCommand<DeletedRecords> deletedRecordCommand,
            IDeletedRecordsServices deletedRecords,
            IRepositoryCommand<CategoriesPrinters> CategoriesPrintersCommand,
            IRepositoryQuery<CategoriesPrinters> CategoriesPrintersQuery)
        {
            _systemHistoryLogsService = systemHistoryLogsService;
            CategoriesRepositoryCommand = categoriesRepositoryCommand;
            this.fileHandler = fileHandler;
            CategoriesRepositoryQuery = categoriesRepositoryQuery;
            _deletedRecordCommand = deletedRecordCommand;
            _deletedRecords = deletedRecords;
            categoriesPrintersCommand = CategoriesPrintersCommand;
            categoriesPrintersQuery = CategoriesPrintersQuery;
        }

        public async Task<ResponseResult> Handle(DeleteCategoriesRequest request, CancellationToken cancellationToken)
        {
            var categories = CategoriesRepositoryQuery.FindAll(e => request.Ids.Contains(e.Id)
           && e.Id != 1 && !e.Items.Select(a => a.GroupId).Contains(e.Id)).ToList();
            // remove image from server
            var imgList = categories.Select(a => a.ImagePath).ToList();
            fileHandler.DeleteImage(imgList);
            CategoriesRepositoryCommand.RemoveRange(categories);
            await CategoriesRepositoryCommand.SaveAsync();
            var Ids = categories.Select(a => a.Id);
            var NumberOfDeleted = Ids.Count();
            if (Ids.ToList().Count > 0)
            {
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteItemCategories,NumberOfDeleted);
                //remove records CategoriesPrinters 
                var catPrinters = new List<CategoriesPrinters>();
                var cp = categoriesPrintersQuery.TableNoTracking;
                foreach (var id in Ids)
                {
                    if(cp.Count() > 0)
                    catPrinters.Add( cp.Where(c => c.CategoryId == id).FirstOrDefault());
                }
                try
                {
                    if (catPrinters.Count() > 0)
                    {
                        categoriesPrintersCommand.RemoveRange(catPrinters);
                        await categoriesPrintersCommand.SaveAsync();
                    }
                    
                }
                catch(Exception e)
                {
                    
                }
                
            }


            //Fill The DeletedRecordTable
            
            _deletedRecords.SetDeletedRecord(Ids.ToList(),2);

            return new ResponseResult() { Data = Ids, Id = null, Result = Ids.ToList().Count > 0 ? Result.Success : Result.Failed };
        }
    }
}
