using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.Process.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers
{
    public class UpdateAdditionStatusHandler : IRequestHandler<UpdateAdditionStatusRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvPurchasesAdditionalCosts> additionQuery;
        private readonly IRepositoryCommand<InvPurchasesAdditionalCosts> additionCommand;
        private readonly IHistory<InvPurchasesAdditionalCostsHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public UpdateAdditionStatusHandler(IRepositoryQuery<InvPurchasesAdditionalCosts> additionQuery,
            IRepositoryCommand<InvPurchasesAdditionalCosts> additionCommand, 
            IHistory<InvPurchasesAdditionalCostsHistory> history, 
            ISystemHistoryLogsService systemHistoryLogsService)
        {
            this.additionQuery = additionQuery;
            this.additionCommand = additionCommand;
            this.history = history;
            _systemHistoryLogsService = systemHistoryLogsService;
        }

        public async Task<ResponseResult> Handle(UpdateAdditionStatusRequest parameters, CancellationToken cancellationToken)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }

            var additions = additionQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.PurchasesAdditionalCostsId));
            var additionsList = additions.ToList();

            additionsList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            var rssult = await additionCommand.UpdateAsyn(additionsList);
            foreach (var addition in additionsList)
            {
                history.AddHistory(addition.PurchasesAdditionalCostsId, addition.LatinName, addition.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            }
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editAdditions);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }
    }

}
