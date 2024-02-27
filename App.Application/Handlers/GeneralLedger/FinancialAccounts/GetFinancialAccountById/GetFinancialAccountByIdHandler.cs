using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class GetFinancialAccountByIdHandler : BusinessBase<GLFinancialAccount>, IRequestHandler<GetFinancialAccountByIdRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLBank> _gLBanKQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryRepositoryQuery;
        private readonly IRepositoryQuery<GLSafe> _gLSafeQuery;
        private readonly IRepositoryQuery<GlReciepts> _gLRecieptQuery;
        private readonly IRepositoryQuery<GLOtherAuthorities> _gLOtherAuthoritiesQuery;
        private readonly IRepositoryQuery<GLFinancialCost> financialCostRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialBranch> financialBranchRepositoryQuery;
        //private readonly IRepositoryQuery<GLJournalEntryDetailsAccounts> _gLJournalEntryDetailsAccountsQuery;
        private readonly IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery;
        private readonly IRepositoryQuery<GLBranch> branchRepositoryQuery;

        public GetFinancialAccountByIdHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLBank> gLBanKQuery, IRepositoryQuery<GLJournalEntryDetails> journalEntryRepositoryQuery, IRepositoryQuery<GLSafe> gLSafeQuery, IRepositoryQuery<GlReciepts> gLRecieptQuery, IRepositoryQuery<GLOtherAuthorities> gLOtherAuthoritiesQuery, IRepositoryQuery<GLFinancialCost> financialCostRepositoryQuery, IRepositoryQuery<GLFinancialBranch> financialBranchRepositoryQuery,  IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery, IRepositoryQuery<GLBranch> branchRepositoryQuery) : base(repositoryActionResult)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            _gLBanKQuery = gLBanKQuery;
            this.journalEntryRepositoryQuery = journalEntryRepositoryQuery;
            _gLSafeQuery = gLSafeQuery;
            _gLRecieptQuery = gLRecieptQuery;
            _gLOtherAuthoritiesQuery = gLOtherAuthoritiesQuery;
            this.financialCostRepositoryQuery = financialCostRepositoryQuery;
            this.financialBranchRepositoryQuery = financialBranchRepositoryQuery;
           // _gLJournalEntryDetailsAccountsQuery = gLJournalEntryDetailsAccountsQuery;
            this.costCenterRepositoryQuery = costCenterRepositoryQuery;
            this.branchRepositoryQuery = branchRepositoryQuery;
        }
        public async Task<IRepositoryActionResult> Handle(GetFinancialAccountByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == 0)
                    return repositoryActionResult.GetRepositoryActionResult(null, RepositoryActionStatus.BadRequest, message: "Select cost center");
                var allAccounts = financialAccountsHelper.getAllAccounts(financialAccountRepositoryQuery);
                var allBanks = financialAccountsHelper.getAllBanks(_gLBanKQuery);
                var allJournalEntryDetails = financialAccountsHelper.getAllGLJournalEntryDetails(journalEntryRepositoryQuery);
                var allSafes = financialAccountsHelper.getAllGLSafes(_gLSafeQuery);
                var allReciptc = financialAccountsHelper.getAllGLReciept(_gLRecieptQuery);
                var allOtherAuthorities = financialAccountsHelper.getAllGLOtherAuthorities(_gLOtherAuthoritiesQuery);


                var costs = financialCostRepositoryQuery.FindQueryable(q => q.FinancialAccountId == request.Id).Select(x => x.CostCenterId).ToArray();
                var financialBranch = financialBranchRepositoryQuery.FindQueryable(q => q.FinancialId == request.Id).Select(x => x.BranchId).ToArray();

                var _account = allAccounts
                    .Include(x => x.Currency)
                    .Where(x => x.Id == request.Id);



                var account = _account.ToList()
                    .Select(x => new
                    {
                        nodeId = x.Id,
                        Code = x.AccountCode.Replace(".", string.Empty),
                        FA_Nature = x.FA_Nature,
                        FinalAccount = x.FinalAccount,
                        x.Status,
                        CurrencyId = x.CurrencyId,
                        CurrencyName = x.Currency.ArabicName,
                        x.ArabicName,
                        x.LatinName,
                        x.IsMain,
                        x.Notes,
                        //isHaveOperation = _gLJournalEntryDetailsAccountsQuery.TableNoTracking.Where(x => x.FinancialAccountId == request.Id).Any(),
                        isHaveChildren = financialAccountRepositoryQuery.TableNoTracking.Where(x => x.ParentId == request.Id).Any(),
                        x.ParentId,
                        ParentName = financialAccountRepositoryQuery.TableNoTracking.Where(q => q.Id == x.ParentId).Select(x => x.ArabicName).FirstOrDefault(),
                        costCenters = costCenterRepositoryQuery.TableNoTracking.Where(q => costs.Contains(q.Id)).Select(x => new { x.Id, x.ArabicName, x.LatinName }).ToList(),
                        financialBranchList = branchRepositoryQuery.TableNoTracking.Where(q => financialBranch.Contains(q.Id)).Select(x => new { x.Id, x.ArabicName, x.LatinName }).ToList(),
                        canDelete = financialAccountsHelper.CanDelete(x.autoCoding, allAccounts, allBanks, allJournalEntryDetails, allSafes, allReciptc, allOtherAuthorities).Result
                    }).FirstOrDefault();

                return repositoryActionResult.GetRepositoryActionResult(account, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
    }
}
