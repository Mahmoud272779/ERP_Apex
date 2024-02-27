using App.Application.Handlers.ForgetPassword;
using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.Process.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Additions.AddAdditions
{
    public class AddAdditionsHandler : IRequestHandler<AddAdditionsRequest, ResponseResult>
    {

        private readonly IRepositoryQuery<InvPurchasesAdditionalCosts> additionsQuery;
        private readonly IRepositoryCommand<InvPurchasesAdditionalCosts> additionsCommand;
        private readonly ISystemHistoryLogsService systemHistoryLogsService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<InvPurchasesAdditionalCostsHistory> history;

        public AddAdditionsHandler(IRepositoryQuery<InvPurchasesAdditionalCosts> additionsQuery, IRepositoryCommand<InvPurchasesAdditionalCosts> additionsCommand, ISystemHistoryLogsService systemHistoryLogsService, IHttpContextAccessor httpContext, IHistory<InvPurchasesAdditionalCostsHistory> history)
        {
            this.additionsQuery = additionsQuery;
            this.additionsCommand = additionsCommand;
            this.systemHistoryLogsService = systemHistoryLogsService;
            this.httpContext = httpContext;
            this.history = history;
        }

        public async Task<ResponseResult> Handle(AddAdditionsRequest request,CancellationToken cancellationToken )
        {
            request.LatinName = Helpers.Helpers.IsNullString(request.LatinName);
            request.ArabicName = Helpers.Helpers.IsNullString(request.ArabicName);
            if (string.IsNullOrEmpty(request.LatinName))
                request.LatinName = request.ArabicName;
            if (string.IsNullOrEmpty(request.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (request.Status < (int)Status.Active || request.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            List<int> AdditionalType = new List<int>(){ (int)PurchasesAdditionalType.constant, (int)PurchasesAdditionalType.RatioOfInvoiceTotal,(int) PurchasesAdditionalType.RatioOfInvoiceNet };
            if (!AdditionalType.Contains(request.AdditionalType))
                return new ResponseResult { Result = Result.Failed, Note = Actions.invalidType };


            var ArabicNameExist = await additionsQuery.GetByAsync(a => a.ArabicName == request.ArabicName);
            if (ArabicNameExist != null)
                return new ResponseResult() { Data = null, Id = ArabicNameExist.PurchasesAdditionalCostsId, Result = Result.Exist, Note = Actions.ArabicNameExist };
            var EnglishNameExist = await additionsQuery.GetByAsync(a => a.LatinName == request.LatinName);

            if (EnglishNameExist != null)
                return new ResponseResult() { Data = null, Id = EnglishNameExist.PurchasesAdditionalCostsId, Result = Result.Exist, Note = Actions.EnglishNameExist };
            var table = Mapping.Mapper.Map<PurchasesAdditionalCostsParameter, InvPurchasesAdditionalCosts>(request);
            int NextCode = additionsQuery.GetMaxCode(e => e.Code) + 1;
            table.Code = NextCode;
            table.UTime = DateTime.Now; //Set Time
            additionsCommand.Add(table);
            history.AddHistory(table.PurchasesAdditionalCostsId, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await systemHistoryLogsService.SystemHistoryLogsService(SystemActionEnum.addAdditions);
            return new ResponseResult() { Data = null, Id = table.PurchasesAdditionalCostsId, Result = Result.Success };

        }
    }
}
