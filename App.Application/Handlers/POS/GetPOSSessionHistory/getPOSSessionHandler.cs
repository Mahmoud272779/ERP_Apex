using App.Domain.Entities.POS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.GetPOSSessionHistory
{
    public class getPOSSessionHandler : IRequestHandler<getPOSSessionHistoryRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<POSSessionHistory> _POSSessionHistoryQuery;

        public getPOSSessionHandler(IRepositoryQuery<POSSessionHistory> pOSSessionHistoryQuery)
        {
            _POSSessionHistoryQuery = pOSSessionHistoryQuery;
        }

        public async Task<ResponseResult> Handle(getPOSSessionHistoryRequest request, CancellationToken cancellationToken)
        {
            var history = _POSSessionHistoryQuery.TableNoTracking
                .Include(x=> x.employees)
                .Where(x => x.POSSessionId == request.sessionId)
                .Select(x=> new
                {
                    date = x.LastDate,
                    arabicName = x.employees.ArabicName,
                    latinName = x.employees.LatinName,
                    transactionTypeAr = x.actionAr,
                    transactionTypeEn = x.actionEn,
                    browserName = x.BrowserName
                })
                .ToList();
            return new ResponseResult
            {
                Data = history,
                Result = Result.Success
            };
        }
    }
}
