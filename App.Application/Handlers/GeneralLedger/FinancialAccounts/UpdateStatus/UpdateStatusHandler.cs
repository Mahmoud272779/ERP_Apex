using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class UpdateStatusHandler : BusinessBase<GLFinancialAccount>,IRequestHandler<UpdateStatusRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public UpdateStatusHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand, ISystemHistoryLogsService systemHistoryLogsService) : base(repositoryActionResult)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.financialAccountRepositoryCommand = financialAccountRepositoryCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<IRepositoryActionResult> Handle(UpdateStatusRequest parameter, CancellationToken cancellationToken)
        {
            try
            {
                if (parameter.Id.Count() == 0)
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.UpdateFileWithErrors, message: "Error", status: RepositoryActionStatus.BadRequest);
                var accounts = financialAccountRepositoryQuery.TableNoTracking;
                var isFinancialAccountsExist = accounts.Where(x => parameter.Id.Contains(x.Id));
                List<GLFinancialAccount> _findAllAccountsWithChild = new List<GLFinancialAccount>();
                foreach (var account in isFinancialAccountsExist)
                {
                    var children = accounts.Where(x => x.autoCoding.StartsWith(account.autoCoding)).ToList();
                    foreach (var child in children)
                    {
                        if (!_findAllAccountsWithChild.Where(x => x.Id == child.Id).Any())
                        {
                            child.Status = parameter.Status;
                            _findAllAccountsWithChild.Add(child);
                        }
                    }
                }
                var updated = await financialAccountRepositoryCommand.UpdateAsyn(_findAllAccountsWithChild);
                if (updated)
                    await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editCalculationGuide);
                return repositoryActionResult.GetRepositoryActionResult(updated ? RepositoryActionStatus.Updated : RepositoryActionStatus.UpdateFileWithErrors, message: updated ? "Updated Successfully" : "Error", status: updated ? RepositoryActionStatus.Ok : RepositoryActionStatus.BadRequest);
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
    }
}
