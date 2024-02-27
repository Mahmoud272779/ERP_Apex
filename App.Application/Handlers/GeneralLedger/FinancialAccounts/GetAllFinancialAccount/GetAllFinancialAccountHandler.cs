using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{ 
    public class GetAllFinancialAccountHandler : BusinessBase<GLFinancialAccount>, IRequestHandler<GetAllFinancialAccountRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLBank> _gLBanKQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryRepositoryQuery;
        private readonly IRepositoryQuery<GLSafe> _gLSafeQuery;
        private readonly IRepositoryQuery<GlReciepts> _gLRecieptQuery;
        private readonly IRepositoryQuery<GLOtherAuthorities> _gLOtherAuthoritiesQuery;
        private readonly IHelperService _helperService;

        public GetAllFinancialAccountHandler(IRepositoryActionResult repositoryActionResult, IHelperService helperService, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLBank> gLBanKQuery, IRepositoryQuery<GLJournalEntryDetails> journalEntryRepositoryQuery, IRepositoryQuery<GLSafe> gLSafeQuery, IRepositoryQuery<GlReciepts> gLRecieptQuery, IRepositoryQuery<GLOtherAuthorities> gLOtherAuthoritiesQuery) : base(repositoryActionResult)
        {
            _helperService = helperService;
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            _gLBanKQuery = gLBanKQuery;
            this.journalEntryRepositoryQuery = journalEntryRepositoryQuery;
            _gLSafeQuery = gLSafeQuery;
            _gLRecieptQuery = gLRecieptQuery;
            _gLOtherAuthoritiesQuery = gLOtherAuthoritiesQuery;
        }
        public async Task<IRepositoryActionResult> Handle(GetAllFinancialAccountRequest paramters, CancellationToken cancellationToken)
        {
            paramters.pageNumber = paramters.pageNumber ?? 1;
            paramters.pageSize = paramters.pageSize ?? 20;
            var allAccounts = financialAccountsHelper.getAllAccounts(financialAccountRepositoryQuery);
            var allBanks = financialAccountsHelper.getAllBanks(_gLBanKQuery);
            var allJournalEntryDetails = financialAccountsHelper.getAllGLJournalEntryDetails(journalEntryRepositoryQuery);
            var allSafes = financialAccountsHelper.getAllGLSafes(_gLSafeQuery);
            var allReciptc = financialAccountsHelper.getAllGLReciept(_gLRecieptQuery);
            var allOtherAuthorities = financialAccountsHelper.getAllGLOtherAuthorities(_gLOtherAuthoritiesQuery);
            try
            {


                var searchCretiera = paramters.SearchCriteria;
                var parentDatasQuey = allAccounts.OrderBy(x => x.AccountCode).Where(x => (string.IsNullOrEmpty(paramters.SearchCriteria) ? x.ParentId == paramters.parentId : true) && x.IsBlock == false);
                var C_D = journalEntryRepositoryQuery.TableNoTracking
                            .Include(x => x.GLFinancialAccount)
                            .Include(x => x.journalEntry);

                if (!string.IsNullOrEmpty(paramters.SearchCriteria))
                    parentDatasQuey = parentDatasQuey.Where(x =>
                    searchCretiera == string.Empty ||
                    searchCretiera == null ||
                    x.ArabicName.ToLower().Contains(searchCretiera) ||
                    x.LatinName.ToLower().Contains(searchCretiera) ||
                    x.AccountCode.Replace(".", string.Empty) == searchCretiera
                    );


                var pagin = paramters.parentId != null ? parentDatasQuey.Skip(((paramters.pageNumber ?? 1) - 1) * 20).Take(20) : parentDatasQuey;
                var result = pagin.ToList();
                paramters.pageNumber = paramters.pageNumber == null ? 1 : paramters.pageNumber;
                double MaxPageNumber = parentDatasQuey.Count() / Convert.ToDouble(paramters.pageSize);
                var countofFilter = Math.Ceiling(MaxPageNumber);
                //Note Id numbe 0 is taken by load more property
                if (paramters.pageNumber != countofFilter)
                {
                    GLFinancialAccount gLFinancialAccount = new GLFinancialAccount()
                    {
                        Id = 0,
                        ArabicName = "...عرض المزيد",
                        LatinName = "...Load More",
                        nextPageNumber = paramters.pageNumber + 1,
                        ParentId = paramters.parentId,
                        FA_Nature = 3,
                        IsMain = true
                    };
                    result.Add(gLFinancialAccount);
                }

                #region new selector
                var accounts = result.Select(x => new
                {
                    nodeId = x.Id,
                    x.ArabicName,
                    x.LatinName,
                    x.ParentId,
                    x.Status,
                    x.FA_Nature,
                    x.CurrencyId,
                    x.IsMain,
                    Code = x.AccountCode.Replace(".", string.Empty),
                    x.Debit,
                    x.Credit,
                    x.FinalAccount,
                    x.HasCostCenter,
                    total = _helperService.GetFinanicalAccountTotalAmount(x.Id, x.autoCoding, C_D).Result,
                    hasChildren = financialAccountsHelper.checkIfHasChildren(x.Id, allAccounts).Result,
                    canDelete = financialAccountsHelper.CanDelete(x.autoCoding, allAccounts, allBanks, allJournalEntryDetails, allSafes, allReciptc, allOtherAuthorities).Result,
                    x.nextPageNumber
                });
                #endregion



                return repositoryActionResult.GetRepositoryActionResult(accounts, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
    }
}
