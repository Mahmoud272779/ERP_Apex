using App.Application.Basic_Process;
using App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryHelper;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class addEntryFundsHandler : BusinessBase<GLJournalEntry>, IRequestHandler<addEntryFundsRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLGeneralSetting> _generalSettingRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery;
        private readonly IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsRepositoryCommand;
        private readonly IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand;
        private readonly IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand;
        private readonly IHttpContextAccessor httpContext;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly iUserInformation _iUserInformation;

        public addEntryFundsHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery, IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery, IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsRepositoryCommand, IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand, IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand, IHttpContextAccessor httpContext, ISystemHistoryLogsService systemHistoryLogsService, iUserInformation iUserInformation) : base(repositoryActionResult)
        {
            _generalSettingRepositoryQuery = generalSettingRepositoryQuery;
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.journalEntryRepositoryQuery = journalEntryRepositoryQuery;
            this.journalEntryDetailsRepositoryQuery = journalEntryDetailsRepositoryQuery;
            this.journalEntryDetailsRepositoryCommand = journalEntryDetailsRepositoryCommand;
            this.journalEntryRepositoryCommand = journalEntryRepositoryCommand;
            this.recHistoryRepositoryCommand = recHistoryRepositoryCommand;
            this.httpContext = httpContext;
            _systemHistoryLogsService = systemHistoryLogsService;
            _iUserInformation = iUserInformation;
        }
        public async Task<IRepositoryActionResult> Handle(addEntryFundsRequest parameter, CancellationToken cancellationToken)
        {
            if (_generalSettingRepositoryQuery.TableNoTracking.FirstOrDefault().isFundsClosed)
                return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "القيود الافتتاحية مغلفة", ErrorMessageEn = "The Funds is closed" }, RepositoryActionStatus.BadRequest, message: "The Funds is closed");
            var balance = parameter.EntryFunds.Select(x => x.Debit).Sum() - parameter.EntryFunds.Select(x => x.Credit).Sum();
            if (balance != 0 && parameter.docId == -1)
                return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Note = "The constraint is unbalanced", Result = Result.Failed, ErrorMessageAr = "هذا القيد غير متزن", ErrorMessageEn = "The constraint is unbalanced" }, RepositoryActionStatus.BadRequest, message: "The constraint is unbalanced");
            var findAccounts = financialAccountRepositoryQuery.TableNoTracking.Where(x => parameter.EntryFunds.Select(c => c.FinancialAccountId).Contains(x.Id)).Count();

            if (findAccounts != parameter.EntryFunds.ToList().Select(x => x.FinancialAccountId).Distinct().Count())
                return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "بعض الحسابات المالية غير موجودة", ErrorMessageEn = "Finanical Accounts not found" }, RepositoryActionStatus.BadRequest, message: "The constraint is unbalanced");
            var entryFunds = journalEntryRepositoryQuery.GetAll().Where(x => x.Id == parameter.docId).FirstOrDefault();
            if (entryFunds != null)
            {
                if (!parameter.isFund)
                    entryFunds.FTDate = parameter.date;
                //var sumCredit  = parameter.EntryFunds.Select(x => x.Credit).Sum();
                //var sumDebit = parameter.EntryFunds.Select(x => x.Debit).Sum();
                //entryFunds.CreditTotal = sumCredit;
                //entryFunds.DebitTotal = sumDebit;
                //entryFunds.Notes = "قيد افتتاحي";
                var lastEntrryFunds = journalEntryDetailsRepositoryQuery.TableNoTracking.Where(x => x.JournalEntryId == parameter.docId && !x.isStoreFund);
                if (lastEntrryFunds.Any() && !parameter.isFund)
                {
                    journalEntryDetailsRepositoryCommand.RemoveRange(lastEntrryFunds);
                    await journalEntryDetailsRepositoryCommand.SaveAsync();
                }

                var gLJournalEntryDetails = Mapping.Mapper.Map<List<EntryFunds>, List<GLJournalEntryDetails>>(parameter.EntryFunds);
                journalEntryDetailsRepositoryCommand.AddRange(gLJournalEntryDetails);
                var saved = await journalEntryDetailsRepositoryCommand.SaveAsync();



                if (parameter.docId != -1)
                {
                    var fundAccount = await journalEntryDetailsRepositoryQuery.GetAllAsyn();
                    fundAccount = fundAccount.Where(x => x.JournalEntryId == parameter.docId).ToList();
                    if (fundAccount.Where(x => x.FinancialAccountId == parameter.Fund_FAId).Any())
                    {
                        var lis = fundAccount.Where(x => x.FinancialAccountId == parameter.Fund_FAId && x.StoreFundId == null);
                        foreach (var item in lis)
                        {
                            await journalEntryDetailsRepositoryCommand.DeleteAsync(item.Id);
                        }
                        //await journalEntryDetailsRepositoryCommand.SaveAsync();
                    }
                    var ActionsList = Actions.fundLists().Where(x => x.id == parameter.docId).FirstOrDefault();
                    if(ActionsList == null)
                    {
                        ActionsList = Actions.fundLists().Where(x => x.id == (parameter.EntryFunds.FirstOrDefault().DocType == (int)DocumentType.CustomerFunds ? -2 : -3) ).FirstOrDefault();
                    }
                    var amount = fundAccount.Where(x => x.StoreFundId != null).Sum(x => x.Credit - x.Debit);
                    var fund_re = new EntryFunds()
                    {

                        Credit = amount < 0 ? amount * -1 : 0,
                        Debit = amount > 0 ? amount : 0,
                        DescriptionAr = ActionsList.descAr,
                        DescriptionEn = ActionsList.descEn,
                        FinancialAccountId = parameter.Fund_FAId,
                        JournalEntryId = parameter.EntryFunds.FirstOrDefault().JournalEntryId,
                        isStoreFund = true,

                        DocType = parameter.EntryFunds.FirstOrDefault().DocType
                    };
                    var _fund_re = Mapping.Mapper.Map<EntryFunds, GLJournalEntryDetails>(fund_re);
                    journalEntryDetailsRepositoryCommand.Add(_fund_re);
                    await journalEntryDetailsRepositoryCommand.SaveAsync();
                }
                saved = await journalEntryRepositoryCommand.UpdateAsyn(entryFunds);

                if (saved)
                    return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Result = Result.Success }, RepositoryActionStatus.Ok, message: "Ok");
                else
                    return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Result = Result.Failed }, RepositoryActionStatus.BadRequest, message: "BadRequest");
            }
            journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, httpContext, _iUserInformation, -1, (int)HistoryTitle.Add, "A");
            if (parameter.docId == -1)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editOpeningBalance);
            return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "بعض الحسابات المالية غير موجودة", ErrorMessageEn = "Finanical Accounts not found" }, RepositoryActionStatus.BadRequest, message: "The constraint is not exist");
        }
    }
}
