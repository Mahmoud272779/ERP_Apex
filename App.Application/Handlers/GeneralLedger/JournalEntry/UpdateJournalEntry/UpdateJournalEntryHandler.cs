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
    public class UpdateJournalEntryHandler : BusinessBase<GLJournalEntry>, IRequestHandler<UpdateJournalEntryRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
        private readonly IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsRepositoryCommand;
        private readonly IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery;
        private readonly IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand;
        private readonly iUserInformation _iUserInformation;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly IRepositoryQuery<GLJournalEntryFiles> journalEntryFilesRepositoryQuery;
        private readonly IFileHandler fileHandler;
        private readonly IRepositoryCommand<GLJournalEntryFiles> journalEntryFilesRepositoryCommand;

        public UpdateJournalEntryHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery, IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsRepositoryCommand, IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery, IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery, IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand, iUserInformation iUserInformation, ISystemHistoryLogsService systemHistoryLogsService, IRepositoryQuery<GLCurrency> currencyRepositoryQuery, IHttpContextAccessor httpContext, IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand, IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand, IRepositoryQuery<GLJournalEntryFiles> journalEntryFilesRepositoryQuery, IFileHandler fileHandler, IRepositoryCommand<GLJournalEntryFiles> journalEntryFilesRepositoryCommand) : base(repositoryActionResult)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.journalEntryRepositoryQuery = journalEntryRepositoryQuery;
            this.journalEntryDetailsRepositoryCommand = journalEntryDetailsRepositoryCommand;
            this.costCenterRepositoryQuery = costCenterRepositoryQuery;
            this.journalEntryDetailsRepositoryQuery = journalEntryDetailsRepositoryQuery;
            this.recHistoryRepositoryCommand = recHistoryRepositoryCommand;
            _iUserInformation = iUserInformation;
            _systemHistoryLogsService = systemHistoryLogsService;
            this.currencyRepositoryQuery = currencyRepositoryQuery;
            this.httpContext = httpContext;
            this.journalEntryRepositoryCommand = journalEntryRepositoryCommand;
            this.financialAccountRepositoryCommand = financialAccountRepositoryCommand;
            this.journalEntryFilesRepositoryQuery = journalEntryFilesRepositoryQuery;
            this.fileHandler = fileHandler;
            this.journalEntryFilesRepositoryCommand = journalEntryFilesRepositoryCommand;
        }
        public async Task<IRepositoryActionResult> Handle(UpdateJournalEntryRequest parameter, CancellationToken cancellationToken)
        {
            try
            {
                var userInfo = await _iUserInformation.GetUserInformation();
                var financialAccounts = financialAccountRepositoryQuery.TableNoTracking.Where(x => parameter.journalEntryDetails.Select(c => c.FinancialAccountId).ToArray().Contains(x.Id));
                if (parameter.journalEntryDetails.Select(x => x.FinancialAccountId).Distinct().ToList().Count() != financialAccounts.Count())
                    return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.BadRequest, message: "Some of the financial accounts dose not exist");
                var list = new List<UpdateJournalEntryParameter>();
                var journalentry = await journalEntryRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);
                if(journalentry.DocType != (int)DocumentType.Purchase && !userInfo.otherSettings.PurchasesallowEdidJournalEntry)
                    if (journalentry.Auto == true && !parameter.fromSystem)
                    {
                        //update cost center
                        var updated = await journalEntryHelper.updateAutoJoruanlsCostCenter(journalEntryDetailsRepositoryCommand, costCenterRepositoryQuery, journalEntryDetailsRepositoryQuery, parameter, journalentry);
                        //add attachments files
                        if (parameter.AttachedFiles != null)
                        {
                            var ListjournalDetailss = new List<GLJournalEntryFiles>();
                            foreach (var item in parameter.AttachedFiles)
                            {
                                var journalDetailss = new GLJournalEntryFiles();
                                journalDetailss.JournalEntryId = journalentry.Id;
                                journalDetailss.File = fileHandler.UploadFile(item, Aliases.FilesDirectories.JournalEntriesDirectory);
                                ListjournalDetailss.Add(journalDetailss);
                            }

                            journalEntryFilesRepositoryCommand.AddRange(ListjournalDetailss);
                            await journalEntryFilesRepositoryCommand.SaveAsync();
                        }
                        //add history
                        journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, _iUserInformation, parameter.BranchId, journalentry.Code, parameter.Id, journalentry.BrowserName, "U", journalentry.LastTransactionUser);
                        //add history logs
                        await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editJournalEntry);

                        return repositoryActionResult.GetRepositoryActionResult(parameter.Id, RepositoryActionStatus.Updated, message: "Updated Successfully");
                    }
                var table = new UpdateJournalEntryParameter();
                journalentry.BranchId = parameter.BranchId;
                //journalentry.CurrencyId = parameter.CurrencyId;
                journalentry.CurrencyId = currencyRepositoryQuery.TableNoTracking.Where(x => x.IsDefault).FirstOrDefault().Id;
                journalentry.FTDate = parameter.FTDate;
                journalentry.Notes = parameter.Notes != "null" ? parameter.Notes : "";
                journalentry.IsTransfer = journalentry.IsTransfer;
                journalentry.CreditTotal = 0;
                journalentry.DebitTotal = 0;
                journalentry.BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());
                //if(!parameter.fromSystem)
                //{
                //    return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.BadRequest, message: "You cant Update this journal entry");
                //}
                var journalentrySaved = await journalEntryRepositoryCommand.UpdateAsyn(journalentry);
                journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, _iUserInformation, parameter.BranchId, journalentry.Code, parameter.Id, journalentry.BrowserName, "U", journalentry.LastTransactionUser);
                await journalEntryDetailsRepositoryCommand.DeleteAsync(q => q.JournalEntryId == parameter.Id);
                foreach (var item in parameter.journalEntryDetails)
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
                        JournalEntryId = journalentry.Id,
                        isCostSales=item.isCostSales,
                    };
                    journalEntryDetailsRepositoryCommand.Add(journalDetails);

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

                    if (journalDetails.Credit != 0)
                    {
                        journalentry.CreditTotal += journalDetails.Credit;
                    }
                    if (journalDetails.Debit != 0)
                    {
                        journalentry.DebitTotal += journalDetails.Debit;

                    }
                }
                await journalEntryDetailsRepositoryCommand.SaveAsync();
                var save = await journalEntryRepositoryCommand.SaveAsync();
                if (journalentrySaved)
                {
                    journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, httpContext, _iUserInformation, journalentry.Code, (int)HistoryTitle.Update, journalentry.LastTransactionAction);
                }

                //put your algorithm to update files after that line

                var oldFiles = journalEntryFilesRepositoryQuery.FindAll(f => f.JournalEntryId == journalentry.Id);
                if ((parameter.FileIds != null && parameter.FileIds.Count > 0) || oldFiles.Count > 0)
                {
                    if (oldFiles.Count > 0 && parameter.FileIds == null)
                    {
                        foreach (var item in oldFiles)
                        {
                            fileHandler.DeleteImage(item.File);
                        }
                        journalEntryFilesRepositoryCommand.RemoveRange(oldFiles);
                        await journalEntryFilesRepositoryCommand.SaveAsync();
                    }
                    else if (parameter.FileIds.Count > 0)
                    {
                        var willDeleteFiles = oldFiles.Where(o => !parameter.FileIds.Contains(o.Id));
                        var filesTobeDeleted = willDeleteFiles.Select(f => f.File);
                        if (filesTobeDeleted.Count() > 0)
                        {
                            foreach (var item in filesTobeDeleted)
                            {
                                fileHandler.DeleteImage(item);
                            }
                        }
                        journalEntryFilesRepositoryCommand.RemoveRange(willDeleteFiles);
                        var iscommit = await journalEntryFilesRepositoryCommand.CommitUnSaved();
                        await journalEntryFilesRepositoryCommand.SaveAsync();
                    }


                }
                if (parameter.AttachedFiles != null)
                {
                    var ListjournalDetailss = new List<GLJournalEntryFiles>();
                    foreach (var item in parameter.AttachedFiles)
                    {
                        var journalDetailss = new GLJournalEntryFiles();
                        journalDetailss.JournalEntryId = journalentry.Id;
                        journalDetailss.File = fileHandler.UploadFile(item, Aliases.FilesDirectories.JournalEntriesDirectory);
                        ListjournalDetailss.Add(journalDetailss);
                    }

                    journalEntryFilesRepositoryCommand.AddRange(ListjournalDetailss);
                    await journalEntryFilesRepositoryCommand.SaveAsync();
                }
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editJournalEntry);
                return repositoryActionResult.GetRepositoryActionResult(parameter.Id, RepositoryActionStatus.Updated, message: "Updated Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex.Message);
            }
        }
    }
}
