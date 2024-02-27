using App.Application.Handlers.Units;
using App.Domain.Entities.Process.Store;
using App.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Additions.DeleteAdditions
{
    public class DeleteAdditionsHandler : IRequestHandler<DeleteAdditionsRequest , ResponseResult>
    {

        private readonly IRepositoryQuery<InvPurchasesAdditionalCosts> additionsQuery;
        private readonly IRepositoryCommand<InvPurchasesAdditionalCosts> additionsCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRepositoryCommand<DeletedRecords> _deletedRecordCommand;
        public DeleteAdditionsHandler(IRepositoryQuery<InvPurchasesAdditionalCosts> additionsQuery,
            IRepositoryCommand<InvPurchasesAdditionalCosts> additionsCommand, 
            ISystemHistoryLogsService systemHistoryLogsService, 
            IRepositoryCommand<DeletedRecords> deletedRecordCommand)
        {
            this.additionsQuery = additionsQuery;
            this.additionsCommand = additionsCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
            _deletedRecordCommand = deletedRecordCommand;
        }

        public async Task<ResponseResult> Handle(DeleteAdditionsRequest parameter, CancellationToken cancellationToken)
        {
            var methods = additionsQuery.FindAll(e => parameter.Ids.Contains(e.PurchasesAdditionalCostsId) 
            &&  !e.InvPurchaseAdditionalCostsRelations.Select(a=>a.AddtionalCostId).Contains(e.PurchasesAdditionalCostsId)).ToList();
            additionsCommand.RemoveRange(methods);
            await additionsCommand.SaveAsync();
            var Ids = methods.Select(a => a.PurchasesAdditionalCostsId);
            if (Ids.ToList().Count > 0)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteAdditions);

            //Fill The DeletedRecordTable
            var listRecords = new List<DeletedRecords>();
            foreach (var item in Ids.ToList())
            {
                var deleted = new DeletedRecords
                {
                    Type = 1,
                    DTime = DateTime.Now,
                    RecordID = item
                };
                listRecords.Add(deleted);
            }
            _deletedRecordCommand.AddRangeAsync(listRecords);

            return new ResponseResult() { Data = Ids, Id = null, Result = Ids.ToList().Count > 0 ? Result.Success : Result.Failed ,ErrorMessageAr= Ids.ToList().Count == 0 ?ErrorMessagesAr.CanNotDelete:ErrorMessagesAr.DeletedSuccessfully 
            , ErrorMessageEn = Ids.ToList().Count == 0 ? ErrorMessagesEn.CanNotDelete : ErrorMessagesEn.DeletedSuccessfully};
        }

    }
}
