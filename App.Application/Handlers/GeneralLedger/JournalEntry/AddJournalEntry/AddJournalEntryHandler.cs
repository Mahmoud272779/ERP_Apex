using App.Application.Basic_Process;
using App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryHelper;
using App.Application.Helpers.Service_helper.FileHandler;
using App.Infrastructure;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class AddJournalEntryHandler : BusinessBase<GLJournalEntry>, IRequestHandler<AddJournalEntryRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand;
        private readonly IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsRepositoryCommand;
        //private readonly IRepositoryQuery<GLJournalEntryDetailsAccounts> journalEntryDetailsAccountsRepositoryQuery;
        //private readonly IRepositoryCommand<GLJournalEntryDetailsAccounts> journalEntryDetailsAccountsRepositoryCommand;
        private readonly IFileHandler fileHandler;
        private readonly IRepositoryCommand<GLJournalEntryFiles> journalEntryFilesRepositoryCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRoundNumbers _roundNumbers;


        public AddJournalEntryHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLCurrency> currencyRepositoryQuery, IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery, IHttpContextAccessor httpContext, IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand, IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand, iUserInformation iUserInformation, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery = null, IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand = null,
            IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsRepositoryCommand = null, 
             IFileHandler fileHandler = null, IRepositoryCommand<GLJournalEntryFiles> journalEntryFilesRepositoryCommand = null,
            ISystemHistoryLogsService systemHistoryLogsService = null, IRoundNumbers roundNumbers = null) : base(repositoryActionResult)
        {
            this.currencyRepositoryQuery = currencyRepositoryQuery;
            this.journalEntryRepositoryQuery = journalEntryRepositoryQuery;
            this.httpContext = httpContext;
            this.journalEntryRepositoryCommand = journalEntryRepositoryCommand;
            this.recHistoryRepositoryCommand = recHistoryRepositoryCommand;
            _iUserInformation = iUserInformation;
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.financialAccountRepositoryCommand = financialAccountRepositoryCommand;
            this.journalEntryDetailsRepositoryCommand = journalEntryDetailsRepositoryCommand;
            //this.journalEntryDetailsAccountsRepositoryQuery = journalEntryDetailsAccountsRepositoryQuery;
            //this.journalEntryDetailsAccountsRepositoryCommand = journalEntryDetailsAccountsRepositoryCommand;
            this.fileHandler = fileHandler;
            this.journalEntryFilesRepositoryCommand = journalEntryFilesRepositoryCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
            _roundNumbers = roundNumbers;
        }

        public async Task<IRepositoryActionResult> Handle(AddJournalEntryRequest parameter, CancellationToken cancellationToken)
        {
            try
            {
                if (!parameter.AddWithOutElements)
                    foreach (var item in parameter.JournalEntryDetails)
                    {
                        if (item.FinancialAccountId == null || item.FinancialAccountId == 0)
                        {
                            return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.BadRequest, message: "You miss some data");
                        }
                    }
                var list = new List<JournalEntryParameter>();
                var table = new GLJournalEntry();
                table.Notes = parameter.Notes;
                //table.CurrencyId = parameter.CurrencyId;
                var CurrencyId = currencyRepositoryQuery.TableNoTracking.Where(x => x.IsDefault);
                table.CurrencyId = CurrencyId.FirstOrDefault().Id;
                table.BranchId = parameter.BranchId;
                table.ReceiptsId = parameter.ReceiptsId;
                table.IsCompined = parameter.IsCompined;
                table.CompinedReceiptCode = parameter.CompinedReceiptCode;
                table.InvoiceId = parameter.InvoiceId;
                if (!string.IsNullOrEmpty(parameter.DocType.ToString()))
                    table.DocType = parameter.DocType;
                //if creat journal entery from invvoices
                if (!parameter.IsAccredit) { table.Code = 0; }
                else { table.Code = journalEntryRepositoryQuery.GetMaxCode(e => e.Code, e => e.Code > 0) + 1; }
                table.FTDate = parameter.FTDate;

                table.IsTransfer = false;
                table.Auto = parameter.isAuto;
                table.IsAccredit = parameter.IsAccredit;
                table.BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());
                var checkCode = await journalEntryHelper.CheckIsValidCode(journalEntryRepositoryQuery, table.Code);
                if (parameter.ReceiptsId != 0)
                    table.IsAccredit = parameter.IsAccredit;

                if (checkCode == true)
                {
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "This code Existing before ");
                }
                await journalEntryRepositoryCommand.AddAsync(table);
                journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, _iUserInformation, table.BranchId, table.Code, table.Id, table.BrowserName, "A", table.LastTransactionUser);
                foreach (var item in parameter.JournalEntryDetails)
                {
                    var journalDetails = new GLJournalEntryDetails()
                    {
                        CostCenterId = item.CostCenterId,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        DescriptionAr = item.DescriptionAr,
                        DescriptionEn = item.DescriptionEn,
                        FinancialAccountId = item.FinancialAccountId,
                        FinancialCode = item.FinancialCode,
                        FinancialName = item.FinancialName,
                        JournalEntryId = table.Id,
                        isCostSales = item.isCostSales
                    };
                    if (journalDetails.FinancialAccountId == null || journalDetails.FinancialAccountId == 0)
                    {
                        journalEntryRepositoryCommand.Remove(table);
                        await journalEntryRepositoryCommand.SaveAsync();
                        return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
                    }
                    var financial = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == journalDetails.FinancialAccountId);

                    //By Alaa
                    var sumCreditDebit = (financial.Credit - financial.Debit) + (journalDetails.Credit - journalDetails.Debit);

                    if (sumCreditDebit > 0)
                    {
                        financial.FA_Nature = (int)FA_Nature.Credit;
                        financial.Credit = sumCreditDebit;
                        financial.Debit = 0;
                    }
                    else
                    {
                        financial.FA_Nature = (int)FA_Nature.Debit;
                        financial.Debit = -sumCreditDebit;
                        financial.Credit = 0;
                    }

                    await financialAccountRepositoryCommand.UpdateAsyn(financial);
                    journalEntryDetailsRepositoryCommand.AddWithoutSaveChanges(journalDetails);
                    table.CreditTotal += journalDetails.Credit;
                    table.DebitTotal += journalDetails.Debit;
                    table.CreditTotal = _roundNumbers.GetDefultRoundNumber(table.CreditTotal);
                    table.DebitTotal = _roundNumbers.GetDefultRoundNumber(table.DebitTotal);
                    //var financialJournal = await journalEntryDetailsAccountsRepositoryQuery.GetByAsync(q => q.FinancialAccountId == journalDetails.FinancialAccountId.Value && q.JournalEntryId == table.Id);
                    //if (financialJournal == null)
                    //{
                    //    var listt = new List<GLJournalEntryDetailsAccounts>();
                    //    var financialAccountJournal = new GLJournalEntryDetailsAccounts();
                    //    if (journalDetails.FinancialAccountId.Value != financialAccountJournal.FinancialAccountId)
                    //    {
                    //        financialAccountJournal.FinancialAccountId = journalDetails.FinancialAccountId.Value;
                    //        financialAccountJournal.JournalEntryId = table.Id;
                    //    }
                    //    listt.Add(financialAccountJournal);
                    //    journalEntryDetailsAccountsRepositoryCommand.AddRange(listt);
                    //}
                }
                if (table.CreditTotal != table.DebitTotal)
                {
                    await journalEntryDetailsRepositoryCommand.DeleteAsync(q => q.JournalEntryId == table.Id);
                    await journalEntryRepositoryCommand.DeleteAsync(table.Id);
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant add this journal entry");
                }
                var listPic = new List<JournalEntryFilesDto>();
                var ListjournalDetailss = new List<GLJournalEntryFiles>();
                var img = parameter.AttachedFiles;
                if (img != null)
                {
                    foreach (var item in img)
                    {
                        var journalDetailss = new GLJournalEntryFiles();
                        journalDetailss.JournalEntryId = table.Id;
                        journalDetailss.File = fileHandler.UploadFile(item, Aliases.FilesDirectories.JournalEntriesDirectory);
                        ListjournalDetailss.Add(journalDetailss);
                    }
                    journalEntryFilesRepositoryCommand.AddRange(ListjournalDetailss);
                    await journalEntryFilesRepositoryCommand.SaveAsync();
                }
                var save = await journalEntryRepositoryCommand.SaveAsync();
                //var save2 = await journalEntryDetailsAccountsRepositoryCommand.SaveAsync();
                var save4 = await journalEntryDetailsRepositoryCommand.SaveAsync();
                journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, httpContext, _iUserInformation, table.Code, (int)HistoryTitle.Add, "A");
                if (!parameter.isAuto)
                    await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addJournalEntry);
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
    }
}
