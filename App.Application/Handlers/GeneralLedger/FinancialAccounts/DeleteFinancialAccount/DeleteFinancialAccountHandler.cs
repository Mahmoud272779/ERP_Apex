using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class DeleteFinancialAccountHandler : BusinessBase<GLFinancialAccount>, IRequestHandler<DeleteFinancialAccountRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLBank> _gLBanKQuery;
       // private readonly IRepositoryQuery<GLJournalEntryDetailsAccounts> _gLJournalEntryDetailsAccountsQuery;
        private readonly IRepositoryQuery<GLSafe> _gLSafeQuery;
        private readonly IRepositoryQuery<GlReciepts> _gLRecieptQuery;
        private readonly IRepositoryQuery<GLOtherAuthorities> _gLOtherAuthoritiesQuery;
        private readonly IRepositoryQuery<InvEmployees> _invEmployeesQuery;
        private readonly IRepositoryQuery<InvPersons> _invPersonsQuery;
        private readonly IRepositoryQuery<InvSalesMan> _invSalesManQuery;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _gLPurchasesAndSalesSettingsQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public DeleteFinancialAccountHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLBank> gLBanKQuery, IRepositoryQuery<GLSafe> gLSafeQuery, IRepositoryQuery<GlReciepts> gLRecieptQuery, IRepositoryQuery<GLOtherAuthorities> gLOtherAuthoritiesQuery, IRepositoryQuery<InvEmployees> invEmployeesQuery, IRepositoryQuery<InvPersons> invPersonsQuery, IRepositoryQuery<InvSalesMan> invSalesManQuery, IRepositoryQuery<GLPurchasesAndSalesSettings> gLPurchasesAndSalesSettingsQuery, IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery, IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand, ISystemHistoryLogsService systemHistoryLogsService) : base(repositoryActionResult)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            _gLBanKQuery = gLBanKQuery;
            //_gLJournalEntryDetailsAccountsQuery = gLJournalEntryDetailsAccountsQuery;
            _gLSafeQuery = gLSafeQuery;
            _gLRecieptQuery = gLRecieptQuery;
            _gLOtherAuthoritiesQuery = gLOtherAuthoritiesQuery;
            _invEmployeesQuery = invEmployeesQuery;
            _invPersonsQuery = invPersonsQuery;
            _invSalesManQuery = invSalesManQuery;
            _gLPurchasesAndSalesSettingsQuery = gLPurchasesAndSalesSettingsQuery;
            this.generalSettingRepositoryQuery = generalSettingRepositoryQuery;
            this.financialAccountRepositoryCommand = financialAccountRepositoryCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<IRepositoryActionResult> Handle(DeleteFinancialAccountRequest parameter, CancellationToken cancellationToken)
        {
            try
            {
                //var defultTreeIds = Enums.DefultGLFinancialAccountList().Select(c => c.Id).Where(c => parameter.Ids.Any(d => d == c)).Any();
                //if (defultTreeIds)
                //    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "Defult data can not be deleted");
                //parameter.Ids = parameter.Ids.Where(x => x > 0).ToArray();

                if (parameter.Ids.Count() == 0)
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.CanNotDelete, message: "Error", status: RepositoryActionStatus.BadRequest);
                var accounts = financialAccountRepositoryQuery.TableNoTracking;
                var isFinancialAccountsExist = accounts.Where(x => parameter.Ids.Contains(x.Id));
                var parentId = isFinancialAccountsExist.Select(x => x.ParentId).FirstOrDefault();
                List<GLFinancialAccount> _findAllAccountsWithChild = new List<GLFinancialAccount>();
                foreach (var account in isFinancialAccountsExist)
                {
                    var children = accounts.Where(x => x.autoCoding.StartsWith(account.autoCoding)).ToList();
                    foreach (var child in children)
                    {
                        if (!_findAllAccountsWithChild.Where(x => x.Id == child.Id).Any())
                            _findAllAccountsWithChild.Add(child);
                    }
                }


                var findAllAccountsWithChild = _findAllAccountsWithChild.Select(x => x.Id).ToArray();

                //bank
                var CheckBanks = _gLBanKQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (CheckBanks.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in GLBanks Table");

                //var check_GLJournalEntryDetailsAccounts = _gLJournalEntryDetailsAccountsQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains(x.FinancialAccountId));
                //if (check_GLJournalEntryDetailsAccounts.Any())
                 //    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in GLJournalEntryDetailsAccounts Table");
                 
                //safes
                var check_GLSafe = _gLSafeQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (check_GLSafe.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in GLSafe Table");
                //recs
                var check_gLReciept = _gLRecieptQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains(x.FinancialAccountId.Value));
                if (check_gLReciept.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in gLReciept Table");
                //oter authorities
                var check_gLOtherAuthorities = _gLOtherAuthoritiesQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (check_gLOtherAuthorities.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in gLOtherAuthorities Table");
                //Employees
                var checkEmps = _invEmployeesQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (checkEmps.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in Employees Table");
                //persons
                var checkPersons = _invPersonsQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (checkPersons.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in Persons Table");
                //salesMan
                var checkSalesMan = _invSalesManQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (checkSalesMan.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in salesman Table");
                //Relationship
                var checkRelation = _gLPurchasesAndSalesSettingsQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (checkRelation.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in gLPurchasesAndSalesSettings Table");
                var checkRelation2 = generalSettingRepositoryQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountIdBank) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdCustomer) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdEmployee) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdOtherAuthorities) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdSafe) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdSalesMan) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdSupplier));
                if (checkRelation.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in generalSetting Table");

                financialAccountRepositoryCommand.RemoveRange(_findAllAccountsWithChild.OrderByDescending(x => x.Id));
                await financialAccountRepositoryCommand.SaveAsync();
                var dataRes = new financialAccountsHelper.DeleteResponse()
                {
                    parentId = parentId
                };
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteCalculationGuide);
                return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.Deleted, message: "Deleted Successfully", result: dataRes);
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex.Message);
            }
        }
    }
}
