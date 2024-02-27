using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts.GetAllFinancialAccountHistory
{
    public class GetAllFinancialAccountHistoryHandler : IRequestHandler<GetAllFinancialAccountHistoryRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccountHistory> financialAccountHistoryRepositoryQuery;

        public GetAllFinancialAccountHistoryHandler(IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLFinancialAccountHistory> financialAccountHistoryRepositoryQuery)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.financialAccountHistoryRepositoryQuery = financialAccountHistoryRepositoryQuery;
        }
        public async Task<ResponseResult> Handle(GetAllFinancialAccountHistoryRequest request, CancellationToken cancellationToken)
        {
            var accountCode = financialAccountRepositoryQuery.TableNoTracking.Where(x => x.Id == request.id).Select(x => x.AccountCode).FirstOrDefault();
            var parentDatasQuey = financialAccountHistoryRepositoryQuery.TableNoTracking.Where(s => s.AccountCode == accountCode).Include(a => a.employees).ToList();
            var historyList = new List<HistoryResponceDto>();
            foreach (var item in parentDatasQuey.ToList())
            {
                var historyDto = new HistoryResponceDto();
                historyDto.Date = item.LastDate;

                if(item.LastAction != null)
                {
                    HistoryActionsNames actionName = HistoryActionsAliasNames.HistoryName[item.LastAction];
                    historyDto.TransactionTypeAr = actionName.ArabicName;
                    historyDto.TransactionTypeEn = actionName.LatinName;
                }
                else
                {
                    historyDto.TransactionTypeAr ="";
                    historyDto.TransactionTypeEn = "";
                }
              
            if(item.isTechnicalSupport)
            {
                 historyDto.LatinName = HistoryActions.TechnicalSupportEn;
                 historyDto.ArabicName = HistoryActions.TechnicalSupportAr;
            }
            else
            {
                historyDto.LatinName = item.employees.LatinName;
                historyDto.ArabicName = item.employees.ArabicName;
            }
           
                historyDto.BrowserName = item.BrowserName;

                if (item.BrowserName.Contains("Chrome"))
                {
                    historyDto.BrowserName = "Chrome";
                }
                if (item.BrowserName.Contains("Firefox"))
                {
                    historyDto.BrowserName = "Firefox";
                }
                if (item.BrowserName.Contains("Opera"))
                {
                    historyDto.BrowserName = "Opera";
                }
                if (item.BrowserName.Contains("InternetExplorer"))
                {
                    historyDto.BrowserName = "InternetExplorer";
                }
                if (item.BrowserName.Contains("Microsoft Edge"))
                {
                    historyDto.BrowserName = "Microsoft Edge";
                }
                historyList.Add(historyDto);
            }
            return new ResponseResult { Data = historyList, Result = Result.Success, Note = Aliases.Actions.Success };
        }
    }
}
