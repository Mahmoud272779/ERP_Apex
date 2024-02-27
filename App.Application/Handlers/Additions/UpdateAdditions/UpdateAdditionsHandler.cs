using App.Application.Handlers.Additions.AddAdditions;
using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.Process.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Additions.UpdateAdditions
{
    public class UpdateAdditionsHandler : IRequestHandler<UpdateAdditionsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvPurchasesAdditionalCosts> additionsQuery;
        private readonly IRepositoryCommand<InvPurchasesAdditionalCosts> additionsCommand;
        private readonly ISystemHistoryLogsService systemHistoryLogsService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<InvPurchasesAdditionalCostsHistory> history;

        public UpdateAdditionsHandler(IRepositoryQuery<InvPurchasesAdditionalCosts> additionsQuery, IRepositoryCommand<InvPurchasesAdditionalCosts> additionsCommand, ISystemHistoryLogsService systemHistoryLogsService, IHttpContextAccessor httpContext, IHistory<InvPurchasesAdditionalCostsHistory> history)
        {
            this.additionsQuery = additionsQuery;
            this.additionsCommand = additionsCommand;
            this.systemHistoryLogsService = systemHistoryLogsService;
            this.httpContext = httpContext;
            this.history = history;
        }

        public async Task<ResponseResult> Handle(UpdateAdditionsRequest request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

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

            var ArabicNameExist = await additionsQuery.GetByAsync(a => a.ArabicName == request.ArabicName && a.PurchasesAdditionalCostsId != request.Id);

            if (ArabicNameExist != null)
                return new ResponseResult() { Data = null, Id = ArabicNameExist.PurchasesAdditionalCostsId, Result = Result.Exist, Note = Actions.ArabicNameExist };
            var EnglishNameExist = await additionsQuery.GetByAsync(a => a.LatinName == request.LatinName && a.PurchasesAdditionalCostsId != request.Id);

            if (EnglishNameExist != null)
                return new ResponseResult() { Data = null, Id = EnglishNameExist.PurchasesAdditionalCostsId, Result = Result.Exist, Note = Actions.EnglishNameExist };

               var used = await additionsQuery.GetByAsync(a => a.PurchasesAdditionalCostsId == request.Id && a.InvPurchaseAdditionalCostsRelations.Count()>0 );

            if (used != null && used.AdditionalType!= request.AdditionalType)
                return new ResponseResult() { Data = null, Id = used.PurchasesAdditionalCostsId, Result = Result.CanNotBeUpdated, Note = Actions.CanNotBeUpdated };



            var data = await additionsQuery.GetByAsync(a => a.PurchasesAdditionalCostsId == request.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            var table = Mapping.Mapper.Map<UpdatePurchasesAdditionalCostsParameter, InvPurchasesAdditionalCosts>(request, data);


            table.UTime = DateTime.Now; // Set Time

            await additionsCommand.UpdateAsyn(table);

            history.AddHistory(table.PurchasesAdditionalCostsId, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            if (data != null)
                await systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editAdditions);
            return new ResponseResult() { Data = null, Id = data.PurchasesAdditionalCostsId, Result = data == null ? Result.Failed : Result.Success };

        }
    }
}
