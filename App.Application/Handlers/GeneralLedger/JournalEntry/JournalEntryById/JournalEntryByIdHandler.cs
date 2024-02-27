using App.Application.Services.Process.GeneralServices.RoundNumber;
using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class JournalEntryByIdHandler : IRequestHandler<JournalEntryByIdRequest, GetJournalEntryByID>
    {
        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
        private readonly IRepositoryQuery<GLBranch> branchRepositoryQuery;
        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery;
        private readonly IRoundNumbers roundNumbers;
        private readonly iUserInformation _iUserInformation;

        public JournalEntryByIdHandler(IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery, IRepositoryQuery<GLBranch> branchRepositoryQuery, IRepositoryQuery<GLCurrency> currencyRepositoryQuery, IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery, IRoundNumbers roundNumbers, iUserInformation iUserInformation)
        {
            this.journalEntryRepositoryQuery = journalEntryRepositoryQuery;
            this.branchRepositoryQuery = branchRepositoryQuery;
            this.currencyRepositoryQuery = currencyRepositoryQuery;
            this.journalEntryDetailsRepositoryQuery = journalEntryDetailsRepositoryQuery;
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.costCenterRepositoryQuery = costCenterRepositoryQuery;
            this.roundNumbers = roundNumbers;
            _iUserInformation = iUserInformation;
        }
        public async Task<GetJournalEntryByID> Handle(JournalEntryByIdRequest request, CancellationToken cancellationToken)
        {
            var costCenterData = journalEntryRepositoryQuery.TableNoTracking.Where(s => request.docId == null ? s.Id == request.Id : (request.docType == 1 ? s.InvoiceId == request.docId : s.ReceiptsId == request.docId));
            var userInfo = await _iUserInformation.GetUserInformation();


            var branch = branchRepositoryQuery.TableNoTracking.Where(q => q.Id == costCenterData.FirstOrDefault().BranchId).FirstOrDefault();
            var currency = await currencyRepositoryQuery.GetByAsync(q => q.Id == costCenterData.FirstOrDefault().CurrencyId);
            var _jouranlDetails = journalEntryDetailsRepositoryQuery.FindAll(q => q.JournalEntryId == costCenterData.FirstOrDefault().Id);
            var financial = financialAccountRepositoryQuery.TableNoTracking.Where(q => _jouranlDetails.Select(x => x.FinancialAccountId).ToArray().Contains(q.Id)).ToList();
            var costs = costCenterRepositoryQuery.TableNoTracking;
            var jouranlDetails = _jouranlDetails.Select(x => new GetJournalEntryDetailsByID
            {
                Id = x.Id,
                JournalEntryId = x.JournalEntryId,
                CostCenterId = x.CostCenterId,

                FinancialAccountId = x.FinancialAccountId,
                financialAccountNameAr = financial.Where(c => c.Id == x.FinancialAccountId).FirstOrDefault().ArabicName,
                financialAccountNameEn = financial.Where(c => c.Id == x.FinancialAccountId).FirstOrDefault().LatinName,
                FinancialAccountCode = financial.Where(c => c.Id == x.FinancialAccountId).FirstOrDefault().AccountCode.Replace(".", String.Empty),
                Debit = roundNumbers.GetRoundNumber(x.Debit),
                Credit = roundNumbers.GetRoundNumber(x.Credit),
                DescriptionAr = x.DescriptionAr,
                DescriptionEn = x.DescriptionEn,
                CostCenterName = costs.Where(c => c.Id == x.CostCenterId).Select(c => c.ArabicName).FirstOrDefault(),
                CostCenterNameEn = costs.Where(c => c.Id == x.CostCenterId).Select(c => c.LatinName).FirstOrDefault(),
                isStoreFund = x.isStoreFund
            }).ToList();

            var res = costCenterData.Select(x => new GetJournalEntryByID
            {
                Id = x.Id,
                BranchId = x.BranchId,
                BranchName = branch.ArabicName,
                BranchNameEn = branch.LatinName,
                CreditTotal = roundNumbers.GetRoundNumber(x.CreditTotal),
                DebitTotal = roundNumbers.GetRoundNumber(x.DebitTotal),
                Auto = userInfo.otherSettings.PurchasesallowEdidJournalEntry && x.DocType == (int)DocumentType.Purchase ? false : x.Auto,
                CurrencyId = x.CurrencyId,
                CurrencyName = currency.ArabicName,
                FTDate = x.FTDate,
                IsBlock = x.IsBlock,
                Code = x.Code,
                Notes = x.Notes,
                JournalEntryDetailsDtos = jouranlDetails,
            }).FirstOrDefault();
            return res;
        }
    }
}
