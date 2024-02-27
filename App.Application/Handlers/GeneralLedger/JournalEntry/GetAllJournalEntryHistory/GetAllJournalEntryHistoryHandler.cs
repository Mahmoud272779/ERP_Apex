using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class GetAllJournalEntryHistoryHandler : IRequestHandler<GetAllJournalEntryHistoryRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GLRecHistory> recHistoryRepositoryQuery;

        public GetAllJournalEntryHistoryHandler(IRepositoryQuery<GLRecHistory> recHistoryRepositoryQuery)
        {
            this.recHistoryRepositoryQuery = recHistoryRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(GetAllJournalEntryHistoryRequest request, CancellationToken cancellationToken)
        {
            var parentDatasQuey = recHistoryRepositoryQuery.FindQueryable(s => s.JournalEntryId == request.JournalEntryId).Include(a => a.employees);
            var historyList = new List<HistoryResponceDto>();
            foreach (var item in parentDatasQuey.ToList())
            {
                var historyDto = new HistoryResponceDto();
                if (item.LastAction != null)
                {
                    HistoryActionsNames actionName = HistoryActionsAliasNames.HistoryName[item.LastAction];

                    historyDto.TransactionTypeAr = actionName.ArabicName;
                    historyDto.TransactionTypeEn = actionName.LatinName;
                }
                else
                {
                    historyDto.TransactionTypeAr = "";
                    historyDto.TransactionTypeEn = "";
                }


                historyDto.Date = item.LastDate;

                
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

                historyList.Add(historyDto);
            }
            return new ResponseResult() { Data = historyList, Id = null, Result = Result.Success };
        }
    }
}
