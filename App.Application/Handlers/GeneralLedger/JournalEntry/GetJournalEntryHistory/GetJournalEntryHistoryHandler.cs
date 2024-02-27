using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralLedger.JournalEntry.GetJournalEntryHistory
{
    public class GetJournalEntryHistoryHandler : IRequestHandler<GetJournalEntryHistoryRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GLRecHistory> recHistoryRepositoryQuery;

        public GetJournalEntryHistoryHandler(IRepositoryQuery<GLRecHistory> recHistoryRepositoryQuery)
        {
            this.recHistoryRepositoryQuery = recHistoryRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(GetJournalEntryHistoryRequest request, CancellationToken cancellationToken)
        {
            var journalEntry = recHistoryRepositoryQuery.FindAll(q => q.Id > 0).ToList();
            //var result = pagedListRecHistory.GetGenericPagination(journalEntry, paramters.PageNumber, paramters.PageSize, Mapper);
            //return repositoryActionResult.GetRepositoryActionResult(result, RepositoryActionStatus.Ok);
            var historyList = new List<HistoryResponceDto>();
            foreach (var item in journalEntry)
            {
                var historyDto = new HistoryResponceDto();
                historyDto.Date = item.LastDate;

                HistoryActionsNames actionName = HistoryActionsAliasNames.HistoryName[item.LastAction];
                historyDto.TransactionTypeAr = actionName.ArabicName;
                historyDto.TransactionTypeEn = actionName.LatinName;
                if (item.isTechnicalSupport)
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
