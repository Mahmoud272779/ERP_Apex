using App.Domain.Entities.Process.General_Ledger;
using App.Domain.Entities.Process.Restaurants;
using App.Domain.Entities.Setup;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Categories
{
    public class GetListOfCategoriesHandler : IRequestHandler<GetListOfCategoriesRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvCategories> CategoriesRepositoryQuery;
        private readonly IRepositoryQuery<CategoriesPrinters> categoriesPrintersQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> InvStpItemCardMasterQuery;

        public GetListOfCategoriesHandler(IRepositoryQuery<InvStpItemCardMaster> invStpItemCardMasterQuery
            , IRepositoryQuery<InvCategories> categoriesRepositoryQuery
            , IRepositoryQuery<CategoriesPrinters> CategoriesPrintersQuery)
        {
            InvStpItemCardMasterQuery = invStpItemCardMasterQuery;
            CategoriesRepositoryQuery = categoriesRepositoryQuery;
            categoriesPrintersQuery = CategoriesPrintersQuery;
        }

        public async Task<ResponseResult> Handle(GetListOfCategoriesRequest parameters, CancellationToken cancellationToken)
        {
            var resData = CategoriesRepositoryQuery.TableNoTracking
            .Where(a => parameters.Id > 0 ? (a.Id == parameters.Id) : true)
            .Where(a => !string.IsNullOrEmpty(parameters.Name) ? a.Code.ToString().Contains(parameters.Name) || a.ArabicName.Contains(parameters.Name) || a.LatinName.Contains(parameters.Name) : true)
            .Where(a => parameters.Status != 0 ? a.Status == parameters.Status : true);
            if (string.IsNullOrEmpty(parameters.Name))
                resData = resData.OrderByDescending(a => a.Code);
            else
                resData = resData.OrderBy(a => a.Code);
            var count = resData.Count();

            foreach( var item in resData) { }

            if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            {
                resData = resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize);
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            var InvStpItemCardMaster = InvStpItemCardMasterQuery.TableNoTracking;
            var res = Mapping.Mapper.Map<List<InvCategories>, List<CategoryWithImgDto>>(resData.ToList());

            // Get Printers data for each category has Printers
            foreach( var item in res) {
                if (item.categoryType == 2)
                {
                    var printers = categoriesPrintersQuery.TableNoTracking.Include(x=>x.Printer)
                        .ThenInclude(b=>b.Branchs)
                        .Where(p => p.CategoryId == item.Id).ToList();

                    var list = new List<PrinterBranchCategoryDto>();
                    foreach (var printer in printers)
                    {
                        list.Add( new PrinterBranchCategoryDto
                        {
                            PrinterID = printer.PrinterId,
                            PrinterAr = printer.Printer.ArabicName,
                            PrinterEn = printer.Printer.LatinName,
                            PrinterIP = printer.Printer.IP,
                            BranchID = printer.Printer.BranchId,
                            BranchAr = printer.Printer.Branchs.ArabicName,
                            BranchEn = printer.Printer.Branchs.LatinName 
                        });
                    }

                    item.printers.AddRange(list);

                }
            }

            res.ForEach(x => x.CanDelete = CategoriesHelper.isCanDeleteCategory(InvStpItemCardMaster, x.Id));

            return new ResponseResult() { Data = res, DataCount = count, Id = null, Result = res.Any() ? Result.Success : Result.Failed };
        }
    }
}
