//using App.Application.Basic_Process;
//using App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryHelper;
//using App.Application.Helpers.Service_helper.ApplicationSettingsHandler;
//using App.Application.Helpers.Service_helper.FileHandler;
//using App.Infrastructure.settings;
//using Attendleave.Erp.Core.APIUtilities;
//using Attendleave.Erp.ServiceLayer.Abstraction;

//namespace App.Application.Services.Process.JournalEntries
//{
//    public class JournalEntryBusiness : BusinessBase<GLJournalEntry>, IJournalEntryBusiness
//    {
//        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
//        private readonly IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand;
//        private readonly IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand;
//        private readonly IRepositoryQuery<GLRecHistory> recHistoryRepositoryQuery;
//        private readonly IRepositoryQuery<GLGeneralSetting> _generalSettingRepositoryQuery;
//        private readonly IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsRepositoryCommand;
//        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery;
//        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
//        private readonly IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery;
//        private readonly IRepositoryQuery<GLBranch> branchRepositoryQuery;
//        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
//        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
//        private readonly IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoryQuery;
//        private readonly IPagedList<GLJournalEntry, GLJournalEntry> pagedListJournalEntryDto;
//        private readonly IPagedList<GLJournalEntryDetails, GLJournalEntryDetails> getJournalEntryByFinancialAccountCodeResponseDto;
//        private readonly IFileHandler fileHandler;
//        private readonly IRoundNumbers roundNumbers;
//        private readonly IInvGeneralSettingsHandler invGeneralSettingsHandler;
//        private readonly IRepositoryQuery<GLJournalEntryFilesDraft> journalEntryFilesDraftRepositoryQuery;
//        private readonly IPagedList<GLRecHistory, GLRecHistory> pagedListRecHistory;
//        private readonly IHttpContextAccessor httpContext;
//        private readonly iUserInformation _iUserInformation;
//        private readonly IPrintService _iprintService;
//        private readonly IFilesMangerService _filesMangerService;


//        private readonly ICompanyDataService _CompanyDataService;
//        private readonly IGeneralPrint _iGeneralPrint;

//        public JournalEntryBusiness(
//            IRepositoryQuery<GLJournalEntry> JournalEntryRepositoryQuery,
//            IRepositoryCommand<GLJournalEntry> JournalEntryRepositoryCommand,
//            IRepositoryCommand<GLJournalEntryDetails> JournalEntryDetailsRepositoryCommand,
//            IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsRepositoryQuery,
//            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
//            IRepositoryQuery<GLCostCenter> CostCenterRepositoryQuery,
//            IRepositoryQuery<GLCurrency> CurrencyRepositoryQuery,
//            IRepositoryQuery<GLBranch> BranchRepositoryQuery,
//            IRepositoryQuery<GLJournalEntryFilesDraft> JournalEntryFilesDraftRepositoryQuery,
//            IRepositoryCommand<GLRecHistory> RecHistoryRepositoryCommand,
//            IRepositoryQuery<GLRecHistory> RecHistoryRepositoryQuery,
//            IRepositoryQuery<GLGeneralSetting> _GeneralSettingRepositoryQuery,
//            IPagedList<GLRecHistory, GLRecHistory> PagedListRecHistory,
//            ISystemHistoryLogsService systemHistoryLogsService,
//            IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoryQuery,
//            IHttpContextAccessor HttpContext,
//            IPagedList<GLJournalEntry, GLJournalEntry> PagedListJournalEntryDto,
//            IPagedList<GLJournalEntryDetails, GLJournalEntryDetails> GetJournalEntryByFinancialAccountCodeResponseDto,
//            IFileHandler fileHandler,
//            IRoundNumbers roundNumbers,
//            IInvGeneralSettingsHandler invGeneralSettingsHandler,
//            IRepositoryActionResult repositoryActionResult, iUserInformation iUserInformation, IPrintService iprintService, ICompanyDataService companyDataService, IFilesMangerService filesMangerService, IGeneralPrint iGeneralPrint) : base(repositoryActionResult)
//        {
//            journalEntryRepositoryQuery = JournalEntryRepositoryQuery;

//            pagedListRecHistory = PagedListRecHistory;
//            journalEntryRepositoryCommand = JournalEntryRepositoryCommand;
//            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
//            currencyRepositoryQuery = CurrencyRepositoryQuery;
//            costCenterRepositoryQuery = CostCenterRepositoryQuery;
//            branchRepositoryQuery = BranchRepositoryQuery;
//            journalEntryFilesDraftRepositoryQuery = JournalEntryFilesDraftRepositoryQuery;
//            journalEntryDetailsRepositoryCommand = JournalEntryDetailsRepositoryCommand;
//            journalEntryDetailsRepositoryQuery = JournalEntryDetailsRepositoryQuery;
//            _systemHistoryLogsService = systemHistoryLogsService;
//            this.generalSettingsRepositoryQuery = generalSettingsRepositoryQuery;
//            recHistoryRepositoryCommand = RecHistoryRepositoryCommand;
//            recHistoryRepositoryQuery = RecHistoryRepositoryQuery;
//            _generalSettingRepositoryQuery = _GeneralSettingRepositoryQuery;
//            pagedListJournalEntryDto = PagedListJournalEntryDto;
//            getJournalEntryByFinancialAccountCodeResponseDto = GetJournalEntryByFinancialAccountCodeResponseDto;
//            this.fileHandler = fileHandler;
//            this.roundNumbers = roundNumbers;
//            this.invGeneralSettingsHandler = invGeneralSettingsHandler;
//            httpContext = HttpContext;
//            _iUserInformation = iUserInformation;
//            _iprintService = iprintService;
//            _CompanyDataService = companyDataService;
//            _filesMangerService = filesMangerService;

//            _iGeneralPrint = iGeneralPrint;
//        }
//        //public async Task<string> AddAutomaticCode()
//        //{
//        //    var code = journalEntryRepositoryQuery.FindQueryable(q => q.Id > 0);
//        //    if (code.Count() > 0)
//        //    {
//        //        var code2 = journalEntryRepositoryQuery.FindQueryable(q => q.Id > 0).ToList().Last();
//        //        int codee = (Convert.ToInt32(code2.Code));
//        //        if (codee == 0)
//        //        {
//        //        }
//        //        var NewCode = codee + 1;
//        //        return NewCode.ToString();

//        //    }
//        //    else
//        //    {
//        //        var NewCode = 1;
//        //        return NewCode.ToString();

//        //    }
//        //}

//        //public async Task<IRepositoryActionResult> AddJournalEntry(JournalEntryParameter parameter)
//        //{
//        //    try
//        //    {
//        //        foreach (var item in parameter.JournalEntryDetails)
//        //        {
//        //            if (item.FinancialAccountId == null || item.FinancialAccountId == 0)
//        //            {
//        //                return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.BadRequest, message: "You miss some data");
//        //            }
//        //        }
//        //        var list = new List<JournalEntryParameter>();
//        //        var table = new GLJournalEntry();
//        //        table.Notes = parameter.Notes;
//        //        //table.CurrencyId = parameter.CurrencyId;
//        //        var CurrencyId = currencyRepositoryQuery.TableNoTracking.Where(x => x.IsDefault);
//        //        table.CurrencyId = CurrencyId.FirstOrDefault().Id;
//        //        table.BranchId = parameter.BranchId;
//        //        table.ReceiptsId = parameter.ReceiptsId;
//        //        table.IsCompined = parameter.IsCompined;
//        //        table.CompinedReceiptCode = parameter.CompinedReceiptCode;
//        //        table.InvoiceId = parameter.InvoiceId;
//        //        if (!string.IsNullOrEmpty(parameter.DocType.ToString()))
//        //            table.DocType = parameter.DocType;
//        //        //if creat journal entery from invvoices
//        //        if (!parameter.IsAccredit) { table.Code = 0; }
//        //        else { table.Code = journalEntryRepositoryQuery.GetMaxCode(e => e.Code, e => e.Code > 0) + 1; }

//        //        table.FTDate = parameter.FTDate;

//        //        table.IsTransfer = false;
//        //        table.Auto = parameter.isAuto;
//        //        table.IsAccredit = parameter.IsAccredit;
//        //        table.BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());
//        //        var checkCode = await journalEntryHelper.CheckIsValidCode(journalEntryRepositoryQuery,table.Code);
//        //        if (parameter.ReceiptsId != 0)
//        //            table.IsAccredit = parameter.IsAccredit;

//        //        if (checkCode == true)
//        //        {
//        //            return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "This code Existing before ");
//        //        }
//        //        await journalEntryRepositoryCommand.AddAsync(table);
//        //        journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand,_iUserInformation,table.BranchId, table.Code, table.Id, table.BrowserName, table.LastTransactionAction, table.LastTransactionUser);
//        //        foreach (var item in parameter.JournalEntryDetails)
//        //        {
//        //            var journalDetails = new GLJournalEntryDetails()
//        //            {
//        //                CostCenterId = item.CostCenterId,
//        //                Credit = item.Credit,
//        //                Debit = item.Debit,
//        //                DescriptionAr = item.DescriptionAr,
//        //                DescriptionEn = item.DescriptionEn,
//        //                FinancialAccountId = item.FinancialAccountId,
//        //                FinancialCode = item.FinancialCode,
//        //                FinancialName = item.FinancialName,
//        //                JournalEntryId = table.Id
//        //            };
//        //            if (journalDetails.FinancialAccountId == null || journalDetails.FinancialAccountId == 0)
//        //            {
//        //                journalEntryRepositoryCommand.Remove(table);
//        //                await journalEntryRepositoryCommand.SaveAsync();
//        //                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
//        //            }
//        //            var financial = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == journalDetails.FinancialAccountId);

//        //            //By Alaa
//        //            var sumCreditDebit = (financial.Credit - financial.Debit) + (journalDetails.Credit - journalDetails.Debit);

//        //            if (sumCreditDebit > 0)
//        //            {
//        //                financial.FA_Nature = (int)FA_Nature.Credit;
//        //                financial.Credit = sumCreditDebit;
//        //                financial.Debit = 0;
//        //            }
//        //            else
//        //            {
//        //                financial.FA_Nature = (int)FA_Nature.Debit;
//        //                financial.Debit = -sumCreditDebit;
//        //                financial.Credit = 0;
//        //            }

//        //            await financialAccountRepositoryCommand.UpdateAsyn(financial);
//        //            journalEntryDetailsRepositoryCommand.AddWithoutSaveChanges(journalDetails);
//        //            table.CreditTotal += journalDetails.Credit;
//        //            table.DebitTotal += journalDetails.Debit;

//        //            var financialJournal = await journalEntryDetailsAccountsRepositoryQuery.GetByAsync(q => q.FinancialAccountId == journalDetails.FinancialAccountId.Value && q.JournalEntryId == table.Id);
//        //            if (financialJournal == null)
//        //            {
//        //                var listt = new List<GLJournalEntryDetailsAccounts>();
//        //                var financialAccountJournal = new GLJournalEntryDetailsAccounts();
//        //                if (journalDetails.FinancialAccountId.Value != financialAccountJournal.FinancialAccountId)
//        //                {
//        //                    financialAccountJournal.FinancialAccountId = journalDetails.FinancialAccountId.Value;
//        //                    financialAccountJournal.JournalEntryId = table.Id;
//        //                }
//        //                listt.Add(financialAccountJournal);
//        //                journalEntryDetailsAccountsRepositoryCommand.AddRange(listt);
//        //            }
//        //        }
//        //        if (table.CreditTotal != table.DebitTotal)
//        //        {
//        //            await journalEntryDetailsRepositoryCommand.DeleteAsync(q => q.JournalEntryId == table.Id);
//        //            await journalEntryRepositoryCommand.DeleteAsync(table.Id);
//        //            return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant add this journal entry");
//        //        }
//        //        var listPic = new List<JournalEntryFilesDto>();
//        //        var ListjournalDetailss = new List<GLJournalEntryFiles>();
//        //        var img = parameter.AttachedFiles;
//        //        if (img != null)
//        //        {
//        //            foreach (var item in img)
//        //            {
//        //                var journalDetailss = new GLJournalEntryFiles();
//        //                journalDetailss.JournalEntryId = table.Id;
//        //                journalDetailss.File = fileHandler.UploadFile(item, Aliases.FilesDirectories.JournalEntriesDirectory);
//        //                ListjournalDetailss.Add(journalDetailss);
//        //            }
//        //            journalEntryFilesRepositoryCommand.AddRange(ListjournalDetailss);
//        //            await journalEntryFilesRepositoryCommand.SaveAsync();
//        //        }
//        //        var save = await journalEntryRepositoryCommand.SaveAsync();
//        //        var save2 = await journalEntryDetailsAccountsRepositoryCommand.SaveAsync();
//        //        var save4 = await journalEntryDetailsRepositoryCommand.SaveAsync();
//        //        journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand,httpContext,_iUserInformation,table.Code, (int)HistoryTitle.Add, "A");
//        //        await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addJournalEntry);
//        //        return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return repositoryActionResult.GetRepositoryActionResult(ex);
//        //    }
//        //}




//        //public async Task<getAllJournalEntryResponse> getJournalEntryServ(PageParameterJournalEntries paramters, bool isPrint = false)
//        //{
//        //    var list = new List<JournalEntryDto>();
//        //    //var journalEntry = journalEntryRepositoryQuery.FindAll(q => q.Id > 0);

//        //    var journalEntry = journalEntryRepositoryQuery.TableNoTracking
//        //                                                    .Include(x => x.JournalEntryDetails)
//        //                                                    .OrderByDescending(x => x.Code)
//        //                                                    .Where(x => x.JournalEntryDetails.Any() && x.IsAccredit == true)
//        //                                                    .Where(q => paramters.Search.From != null ? q.FTDate.Value.Date >= paramters.Search.From.Value.Date : true)
//        //                                                    .Where(q => paramters.Search.To != null ? q.FTDate.Value.Date <= paramters.Search.To.Value.Date : true)
//        //                                                    .Where(q => paramters.Search.Code != 0 ? q.Code == paramters.Search.Code : true)
//        //                                                    .Where(q => paramters.Search.IsTransfer != null ? q.IsTransfer == (paramters.Search.IsTransfer.Value == 1 ? true : false) : true);
//        //    var settings = generalSettingsRepositoryQuery.TableNoTracking.FirstOrDefault();

//        //    var _result = !isPrint ? journalEntry.Skip((paramters.PageNumber - 1) * paramters.PageSize).Take(paramters.PageSize) : journalEntry;
//        //    var result = _result.ToList();
//        //    //pagedListJournalEntryDto.GetGenericPagination(journalEntry, paramters.PageNumber, paramters.PageSize, Mapper);
//        //    var branches = branchRepositoryQuery.TableNoTracking;
//        //    var costs = costCenterRepositoryQuery.TableNoTracking;
//        //    var allAccounts = journalEntryHelper.getAllAccounts(financialAccountRepositoryQuery);
//        //    var ActionsList = Actions.fundLists();
//        //    var allFinancialAccounts = journalEntryHelper.getAllAccounts(financialAccountRepositoryQuery);
//        //    var data = result
//        //        .Select(x => new getAllJournalEntryPrepering
//        //        {
//        //            Id = x.Id,
//        //            Code = x.Code,
//        //            CreditTotal = roundNumbers.GetRoundNumber(x.JournalEntryDetails.Sum(d => d.Credit)    /*x.CreditTotal*/),
//        //            DebitTotal = roundNumbers.GetRoundNumber(x.JournalEntryDetails.Sum(d => d.Debit)     /*x.DebitTotal*/),
//        //            CurrencyId = x.CurrencyId,
//        //            FTDate = x.FTDate != null ? x.FTDate.Value.ToString(defultData.datetimeFormat) : "",
//        //            Notes = x.Code < 0 ? ActionsList.Where(d => d.id == x.Id).FirstOrDefault().descAr : x.Notes,
//        //            BranchId = x.BranchId,
//        //            BranchNameAr = branches.Where(c => c.Id == x.BranchId).Select(c => c.ArabicName).FirstOrDefault(),
//        //            BranchNameEn = branches.Where(c => c.Id == x.BranchId).Select(c => c.LatinName).FirstOrDefault(),
//        //            IsBlock = x.IsBlock,
//        //            IsDraft = false,
//        //            IsTransfer = x.IsTransfer,
//        //            Auto = x.Auto,
//        //            canDelete = x.JournalEntryDetails.FirstOrDefault().isStoreFund,
//        //            journalEntryDetailsDtos = x.JournalEntryDetails
//        //                    .ToList()
//        //                    .Select(c => new journalEntryDetailsDtosResponse
//        //                    {
//        //                        Id = c.Id,
//        //                        FinancialAccountId = c.FinancialAccountId,
//        //                        JournalEntryId = c.JournalEntryId,
//        //                        DescriptionAr = c.DescriptionAr,
//        //                        DescriptionEn = c.DescriptionEn,
//        //                        CostCenterId = c.CostCenterId,
//        //                        Credit = roundNumbers.GetRoundNumber(c.Credit    /*x.CreditTotal*/),
//        //                        Debit = roundNumbers.GetRoundNumber(c.Debit     /*x.DebitTotal*/),
//        //                        financialAccountNameAr = allAccounts.Where(d => d.Id == c.FinancialAccountId).FirstOrDefault().ArabicName,
//        //                        financialAccountNameEn = allAccounts.Where(d => d.Id == c.FinancialAccountId).FirstOrDefault().LatinName,
//        //                        financialAccountCode = allAccounts.Where(d => d.Id == c.FinancialAccountId).FirstOrDefault().AccountCode.Replace(".", string.Empty),
//        //                        CostCenterName = costs.Where(d => d.Id == c.CostCenterId).Select(d => d.ArabicName).FirstOrDefault(),
//        //                        CostCenterNameEn = costs.Where(d => d.Id == c.CostCenterId).Select(d => d.LatinName).FirstOrDefault(),

//        //                    })
//        //        });
//        //    return new getAllJournalEntryResponse()
//        //    {
//        //        data = data,
//        //        journalEntry = journalEntry
//        //    };
//        //}



//        //public async Task<IRepositoryActionResult> GetJournalEntry(PageParameterJournalEntries paramters)
//        //{
//        //    var servData = await journalEntryHelper.getJournalEntryServ(paramters);
//        //    var data = servData.data;
//        //    var listCount = servData.journalEntry.Count();
//        //    double MaxPageNumber = listCount / Convert.ToDouble(paramters.PageSize);
//        //    var countofFilter = int.Parse(Math.Ceiling(MaxPageNumber).ToString());
//        //    var response = new PaginationResponse()
//        //    {
//        //        Data = data,
//        //        ListCount = listCount,
//        //        PageNumber = paramters.PageNumber,
//        //        PageSize = paramters.PageSize,
//        //        TotalPages = countofFilter
//        //    };



//        //    return repositoryActionResult.GetRepositoryActionResult(response, RepositoryActionStatus.Ok);
//        //}


//        //public async Task<IRepositoryActionResult> UpdateJournalEntry(UpdateJournalEntryParameter parameter)
//        //{
//        //    try
//        //    {
//        //        var financialAccounts = financialAccountRepositoryQuery.TableNoTracking.Where(x => parameter.journalEntryDetails.Select(c => c.FinancialAccountId).ToArray().Contains(x.Id));
//        //        if (parameter.journalEntryDetails.Select(x => x.FinancialAccountId).Distinct().ToList().Count() != financialAccounts.Count())
//        //            return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.BadRequest, message: "Some of the financial accounts dose not exist");
//        //        var list = new List<UpdateJournalEntryParameter>();
//        //        var journalentry = await journalEntryRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);
//        //        if (journalentry.Auto == true && !parameter.fromSystem)
//        //        {
//        //            var updated = await journalEntryHelper.updateAutoJoruanlsCostCenter(journalEntryDetailsRepositoryCommand, costCenterRepositoryQuery, journalEntryDetailsRepositoryQuery, parameter, journalentry);
//        //            journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, _iUserInformation, parameter.BranchId, journalentry.Code, parameter.Id, journalentry.BrowserName, journalentry.LastTransactionAction, journalentry.LastTransactionUser);
//        //            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editJournalEntry);
//        //            return repositoryActionResult.GetRepositoryActionResult(parameter.Id, RepositoryActionStatus.Updated, message: "Updated Successfully");
//        //        }
//        //        var table = new UpdateJournalEntryParameter();
//        //        journalentry.BranchId = parameter.BranchId;
//        //        //journalentry.CurrencyId = parameter.CurrencyId;
//        //        journalentry.CurrencyId = currencyRepositoryQuery.TableNoTracking.Where(x => x.IsDefault).FirstOrDefault().Id;
//        //        journalentry.FTDate = parameter.FTDate;
//        //        journalentry.Notes = parameter.Notes != "null" ? parameter.Notes : "";
//        //        journalentry.IsTransfer = journalentry.IsTransfer;
//        //        journalentry.CreditTotal = 0;
//        //        journalentry.DebitTotal = 0;
//        //        journalentry.BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());
//        //        //if(!parameter.fromSystem)
//        //        //{
//        //        //    return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.BadRequest, message: "You cant Update this journal entry");
//        //        //}
//        //        var journalentrySaved = await journalEntryRepositoryCommand.UpdateAsyn(journalentry);
//        //        journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, _iUserInformation, parameter.BranchId, journalentry.Code, parameter.Id, journalentry.BrowserName, journalentry.LastTransactionAction, journalentry.LastTransactionUser);
//        //        await journalEntryDetailsRepositoryCommand.DeleteAsync(q => q.JournalEntryId == parameter.Id);
//        //        foreach (var item in parameter.journalEntryDetails)
//        //        {
//        //            var journalDetails = new GLJournalEntryDetails()
//        //            {
//        //                CostCenterId = item.CostCenterId,
//        //                Credit = item.Credit,
//        //                Debit = item.Debit,
//        //                DescriptionAr = item.DescriptionAr,
//        //                DescriptionEn = item.DescriptionEn,
//        //                FinancialAccountId = item.FinancialAccountId,
//        //                FinancialCode = item.FinancialCode,
//        //                FinancialName = item.FinancialName,
//        //                JournalEntryId = journalentry.Id
//        //            };
//        //            journalEntryDetailsRepositoryCommand.Add(journalDetails);

//        //            var financial = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == journalDetails.FinancialAccountId);

//        //            //By Alaa
//        //            var sumCreditDebit = (financial.Credit - financial.Debit) + (journalDetails.Credit - journalDetails.Debit);

//        //            if (sumCreditDebit > 0)
//        //            {
//        //                financial.FA_Nature = (int)FA_Nature.Credit;
//        //                financial.Credit = sumCreditDebit;
//        //                financial.Debit = 0;
//        //            }
//        //            else
//        //            {
//        //                financial.FA_Nature = (int)FA_Nature.Debit;
//        //                financial.Debit = -sumCreditDebit;
//        //                financial.Credit = 0;
//        //            }
//        //            #region Commented code of calculations
//        //            //if (journalDetails.Debit != 0)
//        //            //{
//        //            //    if (financial.Debit != 0)
//        //            //    {
//        //            //        financial.Debit = financial.Debit + journalDetails.Debit;
//        //            //    }
//        //            //    if (financial.Credit != 0 && financial.Credit > journalDetails.Debit)
//        //            //    {
//        //            //        financial.Credit = financial.Credit - journalDetails.Debit;
//        //            //    }
//        //            //    if (financial.Credit != 0 && financial.Credit < journalDetails.Debit)
//        //            //    {
//        //            //        financial.Credit = journalDetails.Debit - financial.Credit;
//        //            //    }
//        //            //    if (financial.Credit == 0 && financial.Debit == 0)
//        //            //    {
//        //            //        financial.Debit = journalDetails.Debit;
//        //            //    }
//        //            //}
//        //            //if (journalDetails.Credit != 0)
//        //            //{
//        //            //    if (financial.Debit != 0 && financial.Debit > journalDetails.Credit)
//        //            //    {
//        //            //        financial.Debit = financial.Debit - journalDetails.Credit;
//        //            //    }
//        //            //    if (financial.Debit != 0 && financial.Debit < journalDetails.Credit)
//        //            //    {
//        //            //        financial.Debit = journalDetails.Credit - financial.Debit;
//        //            //    }
//        //            //    if (financial.Credit != 0)
//        //            //    {
//        //            //        financial.Credit = financial.Credit + journalDetails.Credit;
//        //            //    }
//        //            //    if (financial.Credit == 0 && financial.Debit == 0)
//        //            //    {
//        //            //        financial.Credit = journalDetails.Credit;
//        //            //    }
//        //            //}

//        //            #endregion

//        //            await financialAccountRepositoryCommand.UpdateAsyn(financial);

//        //            if (journalDetails.Credit != 0)
//        //            {
//        //                journalentry.CreditTotal += journalDetails.Credit;
//        //            }
//        //            if (journalDetails.Debit != 0)
//        //            {
//        //                journalentry.DebitTotal += journalDetails.Debit;

//        //            }
//        //        }
//        //        await journalEntryDetailsRepositoryCommand.SaveAsync();
//        //        var save = await journalEntryRepositoryCommand.SaveAsync();
//        //        if (journalentrySaved)
//        //        {
//        //            journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, httpContext, _iUserInformation, journalentry.Code, (int)HistoryTitle.Update, journalentry.LastTransactionAction);
//        //        }

//        //        //put your algorithm to update files after that line

//        //        var oldFiles = journalEntryFilesRepositoryQuery.FindAll(f => f.JournalEntryId == journalentry.Id);
//        //        if ((parameter.FileIds != null && parameter.FileIds.Count > 0) || oldFiles.Count > 0)
//        //        {
//        //            if (oldFiles.Count > 0 && parameter.FileIds == null)
//        //            {
//        //                foreach (var item in oldFiles)
//        //                {
//        //                    fileHandler.DeleteImage(item.File);
//        //                }
//        //                journalEntryFilesRepositoryCommand.RemoveRange(oldFiles);
//        //                await journalEntryFilesRepositoryCommand.SaveAsync();
//        //            }
//        //            else if (parameter.FileIds.Count > 0)
//        //            {
//        //                var willDeleteFiles = oldFiles.Where(o => !parameter.FileIds.Contains(o.Id));
//        //                var filesTobeDeleted = willDeleteFiles.Select(f => f.File);
//        //                if (filesTobeDeleted.Count() > 0)
//        //                {
//        //                    foreach (var item in filesTobeDeleted)
//        //                    {
//        //                        fileHandler.DeleteImage(item);
//        //                    }
//        //                }
//        //                journalEntryFilesRepositoryCommand.RemoveRange(willDeleteFiles);
//        //                var iscommit = await journalEntryFilesRepositoryCommand.CommitUnSaved();
//        //                await journalEntryFilesRepositoryCommand.SaveAsync();
//        //            }


//        //        }
//        //        if (parameter.AttachedFiles != null)
//        //        {
//        //            var ListjournalDetailss = new List<GLJournalEntryFiles>();
//        //            foreach (var item in parameter.AttachedFiles)
//        //            {
//        //                var journalDetailss = new GLJournalEntryFiles();
//        //                journalDetailss.JournalEntryId = journalentry.Id;
//        //                journalDetailss.File = fileHandler.UploadFile(item, Aliases.FilesDirectories.JournalEntriesDirectory);
//        //                ListjournalDetailss.Add(journalDetailss);
//        //            }

//        //            journalEntryFilesRepositoryCommand.AddRange(ListjournalDetailss);
//        //            await journalEntryFilesRepositoryCommand.SaveAsync();
//        //        }
//        //        await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editJournalEntry);
//        //        return repositoryActionResult.GetRepositoryActionResult(parameter.Id, RepositoryActionStatus.Updated, message: "Updated Successfully");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return repositoryActionResult.GetRepositoryActionResult(ex.Message);
//        //    }
//        //}




//        //public async Task<IRepositoryActionResult> BlockJournalEntry(BlockJournalEntry parameter)
//        //{
//        //    try
//        //    {
//        //        foreach (var item in parameter.Ids)
//        //        {
//        //            var journalEntry = await journalEntryRepositoryQuery.GetByAsync(q => q.Id == item);

//        //            journalEntry.IsBlock = true;
//        //            var save = await journalEntryRepositoryCommand.UpdateAsyn(journalEntry);
//        //            if (save == true)
//        //            {
//        //                journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, _iUserInformation, journalEntry.BranchId, journalEntry.Code, journalEntry.Id, journalEntry.BrowserName, "D", "D");

//        //            }
//        //        }
//        //        await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteJournalEntry);
//        //        return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.Deleted, message: "Blocked Successfully");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return repositoryActionResult.GetRepositoryActionResult(ex);
//        //    }
//        //}



//        //public async Task<IRepositoryActionResult> AddJournalEntryFiles(JournalEntryFilesDto parameter)
//        //{
//        //    try
//        //    {
//        //        var list = new List<JournalEntryFilesDto>();
//        //        var journalDetails = new GLJournalEntryFiles();
//        //        journalDetails.JournalEntryId = parameter.JournalEntryId;
//        //        var img = parameter.ImagePath;
//        //        if (img == null)
//        //        {
//        //            if (string.IsNullOrEmpty(journalDetails.File) || journalDetails.File == "string")
//        //            {
//        //                journalDetails.File = "JournalEntry\\8-30-2020 5-45-17 PMCapture.PNG";
//        //            }
//        //        }
//        //        else
//        //        {
//        //            //foreach (var item in img)
//        //            //{
//        //            //    var path = _hostingEnvironment.WebRootPath;
//        //            //    string filePath = Path.Combine("JournalEntry\\", DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + item.FileName.Replace(" ", ""));
//        //            //    string actulePath = Path.Combine(path, filePath);
//        //            //    using (var fileStream = new FileStream(actulePath, FileMode.Create))
//        //            //    {
//        //            //        await item.CopyToAsync(fileStream);
//        //            //    }
//        //            //    journalDetails.File = filePath;
//        //            //}
//        //        }
//        //        journalEntryFilesRepositoryCommand.Add(journalDetails);
//        //        return repositoryActionResult.GetRepositoryActionResult(journalDetails.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return repositoryActionResult.GetRepositoryActionResult(ex);
//        //    }
//        //}


//        //public async Task<IRepositoryActionResult> GetJournalEntryFiles(int JournalEntryId, int JournalEntryDraftId)
//        //{
//        //    try
//        //    {
//        //        if (JournalEntryId != 0)
//        //        {
//        //            var journalentry = journalEntryFilesRepositoryQuery.FindAll(q => q.JournalEntryId == JournalEntryId);

//        //            var list = new List<JournalEntriesFilesDto>();
//        //            foreach (var item in journalentry)
//        //            {
//        //                var journal = new JournalEntriesFilesDto();
//        //                journal.Id = item.Id;
//        //                journal.File = item.File;
//        //                journal.JournalEntryId = item.JournalEntryId;
//        //                char[] chara = { 'M', 'م', 'ص' };
//        //                string[] fileName = journal.File.Split(chara);
//        //                journal.FileName = fileName.Last();
//        //                list.Add(journal);
//        //            }
//        //            return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok);

//        //        }
//        //        else
//        //        {
//        //            var journalentry = journalEntryFilesDraftRepositoryQuery.FindAll(q => q.JournalEntryDraftId == JournalEntryDraftId);

//        //            var list = new List<JournalEntriesFilesDto>();
//        //            foreach (var item in journalentry)
//        //            {
//        //                var journal = new JournalEntriesFilesDto();
//        //                journal.Id = item.Id;
//        //                journal.File = item.File;
//        //                journal.JournalEntryId = item.JournalEntryDraftId;
//        //                char[] chara = { 'M', 'م', 'ص' };
//        //                string[] fileName = journal.File.Split(chara);
//        //                journal.FileName = fileName.Last();
//        //                list.Add(journal);
//        //            }
//        //            return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok);

//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return repositoryActionResult.GetRepositoryActionResult(ex);
//        //    }
//        //}



//        //public async Task<IRepositoryActionResult> RemoveJournalEntryFiles(FileTrans parameter)
//        //{
//        //    try
//        //    {
//        //        var journal = await journalEntryFilesRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);
//        //        journalEntryFilesRepositoryCommand.Remove(journal);
//        //        await journalEntryFilesRepositoryCommand.SaveAsync();
//        //        return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.Created, message: "Deleted Successfully");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return repositoryActionResult.GetRepositoryActionResult(ex);
//        //    }
//        //}



//        //public async Task<ResponseResult> GetJournalEntryHistory(PageParameter paramters)
//        //{
//        //    var journalEntry = recHistoryRepositoryQuery.FindAll(q => q.Id > 0).ToList();
//        //    //var result = pagedListRecHistory.GetGenericPagination(journalEntry, paramters.PageNumber, paramters.PageSize, Mapper);
//        //    //return repositoryActionResult.GetRepositoryActionResult(result, RepositoryActionStatus.Ok);
//        //    var historyList = new List<HistoryResponceDto>();
//        //    foreach (var item in journalEntry)
//        //    {
//        //        var historyDto = new HistoryResponceDto();
//        //        historyDto.Date = item.LastDate;

//        //        HistoryActionsNames actionName = HistoryActionsAliasNames.HistoryName[item.LastAction];
//        //        historyDto.TransactionTypeAr = actionName.ArabicName;
//        //        historyDto.TransactionTypeEn = actionName.LatinName;
//        //        historyDto.LatinName = item.employees.LatinName;
//        //        historyDto.ArabicName = item.employees.ArabicName;
//        //        historyDto.BrowserName = item.BrowserName;

//        //        if (item.BrowserName.Contains("Chrome"))
//        //        {
//        //            historyDto.BrowserName = "Chrome";
//        //        }
//        //        if (item.BrowserName.Contains("Firefox"))
//        //        {
//        //            historyDto.BrowserName = "Firefox";
//        //        }
//        //        if (item.BrowserName.Contains("Opera"))
//        //        {
//        //            historyDto.BrowserName = "Opera";
//        //        }
//        //        if (item.BrowserName.Contains("InternetExplorer"))
//        //        {
//        //            historyDto.BrowserName = "InternetExplorer";
//        //        }
//        //        if (item.BrowserName.Contains("Microsoft Edge"))
//        //        {
//        //            historyDto.BrowserName = "Microsoft Edge";
//        //        }
//        //        historyList.Add(historyDto);
//        //    }
//        //    return new ResponseResult { Data = historyList, Result = Result.Success, Note = Aliases.Actions.Success };


//        //}


//        //public async Task<GetJournalEntryByID> JournalEntryById(int Id)
//        //{
//        //    var costCenterData = journalEntryRepositoryQuery.TableNoTracking.Where(s => s.Id == Id);
//        //    var branch = branchRepositoryQuery.TableNoTracking.Where(q => q.Id == costCenterData.FirstOrDefault().BranchId).FirstOrDefault();
//        //    var currency = await currencyRepositoryQuery.GetByAsync(q => q.Id == costCenterData.FirstOrDefault().CurrencyId);
//        //    var _jouranlDetails = journalEntryDetailsRepositoryQuery.FindAll(q => q.JournalEntryId == costCenterData.FirstOrDefault().Id);
//        //    var financial = financialAccountRepositoryQuery.TableNoTracking.Where(q => _jouranlDetails.Select(x => x.FinancialAccountId).ToArray().Contains(q.Id)).ToList();
//        //    var costs = costCenterRepositoryQuery.TableNoTracking;
//        //    var jouranlDetails = _jouranlDetails.Select(x => new GetJournalEntryDetailsByID
//        //    {
//        //        Id = x.Id,
//        //        JournalEntryId = x.JournalEntryId,
//        //        CostCenterId = x.CostCenterId,

//        //        FinancialAccountId = x.FinancialAccountId,
//        //        financialAccountNameAr = financial.Where(c => c.Id == x.FinancialAccountId).FirstOrDefault().ArabicName,
//        //        financialAccountNameEn = financial.Where(c => c.Id == x.FinancialAccountId).FirstOrDefault().LatinName,
//        //        FinancialAccountCode = financial.Where(c => c.Id == x.FinancialAccountId).FirstOrDefault().AccountCode.Replace(".", String.Empty),
//        //        Debit = roundNumbers.GetRoundNumber(x.Debit),
//        //        Credit = roundNumbers.GetRoundNumber(x.Credit),
//        //        DescriptionAr = /*costCenterData.FirstOrDefault().Auto ? costCenterData.FirstOrDefault().Notes :*/ x.DescriptionAr,
//        //        DescriptionEn = /*costCenterData.FirstOrDefault().Auto ? costCenterData.FirstOrDefault().Notes :*/ x.DescriptionEn,
//        //        CostCenterName = costs.Where(c => c.Id == x.CostCenterId).Select(c => c.ArabicName).FirstOrDefault(),
//        //        CostCenterNameEn = costs.Where(c => c.Id == x.CostCenterId).Select(c => c.LatinName).FirstOrDefault(),
//        //        isStoreFund = x.isStoreFund
//        //    }).ToList();

//        //    var res = costCenterData.Select(x => new GetJournalEntryByID
//        //    {
//        //        Id = x.Id,
//        //        BranchId = x.BranchId,
//        //        BranchName = branch.ArabicName,
//        //        BranchNameEn = branch.LatinName,
//        //        CreditTotal = roundNumbers.GetRoundNumber(x.CreditTotal),
//        //        DebitTotal = roundNumbers.GetRoundNumber(x.DebitTotal),
//        //        Auto = x.Auto,
//        //        CurrencyId = x.CurrencyId,
//        //        CurrencyName = currency.ArabicName,
//        //        FTDate = x.FTDate,
//        //        IsBlock = x.IsBlock,
//        //        Code = x.Code,
//        //        Notes = x.Notes,
//        //        JournalEntryDetailsDtos = jouranlDetails,
//        //    }).FirstOrDefault();
//        //    return res;

//        //}


//        //public async Task<IRepositoryActionResult> GetJournalEntryById(int Id)
//        //{
//        //    try
//        //    {
//        //        var res = await JournalEntryById(Id);
//        //        return repositoryActionResult.GetRepositoryActionResult(res, RepositoryActionStatus.Ok, message: "Ok");

//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return repositoryActionResult.GetRepositoryActionResult(ex);
//        //    }
//        //}

//        //public async Task<WebReport> JournalEntryPrint(string ids, exportType exportType, bool isArabic)
//        //{
//        //    // var allData= getJournalEntryServ
//        //    List<GetJournalEntryByID> MainData = new List<GetJournalEntryByID>();
//        //    List<GetJournalEntryDetailsByID> Listdata = new List<GetJournalEntryDetailsByID>();
//        //    string[] getIds = ids.Split(",");
//        //    var data = new GetJournalEntryByID();
//        //    int y = 1;
//        //    foreach (var id in getIds)
//        //    {
//        //        data = await JournalEntryById(Convert.ToInt32(id));
//        //        data.GroupId = y;
//        //        data.Date = data.FTDate?.ToString("yyyy/MM/dd");
//        //        foreach (var item in data.JournalEntryDetailsDtos)
//        //        {
//        //            item.GroupId = y;
//        //        }
//        //        MainData.Add(data);
//        //        Listdata.AddRange(data.JournalEntryDetailsDtos);
//        //        y++;
//        //    }

//        //    var userInfo = _iUserInformation.GetUserInformation();

//        //    var otherdata = new ReportOtherData()
//        //    {


//        //        EmployeeName = userInfo.employeeNameAr.ToString(),
//        //        EmployeeNameEn = userInfo.employeeNameEn.ToString(),
//        //        Date = DateTime.Now.ToString("yyyy/MM/dd")

//        //    };
//        //    var tableNames = new TablesNames()
//        //    {
//        //        FirstListName = "FinancialAccount",
//        //        SecondListName = "FinancialAccountList"
//        //    };
//        //    var report = await _iGeneralPrint.PrintReport<object, GetJournalEntryByID, GetJournalEntryDetailsByID>(null, MainData, Listdata
//        //        , tableNames, otherdata, (int)SubFormsIds.AccountingEntries_GL, exportType, isArabic);
//        //    return report;

//        //}

//        //public async Task<IRepositoryActionResult> GetJournalEntryByFinancialAccountCode(int financialId, int pageNumber, int pageSize)
//        //{
//        //    try
//        //    {
//        //        var costCenterData = journalEntryDetailsRepositoryQuery.TableNoTracking.Include(a => a.journalEntry)
//        //            .Where(s => s.FinancialAccountId == financialId && s.journalEntry.IsBlock != true && s.journalEntry.IsAccredit == true).ToList();
//        //        var settings = generalSettingsRepositoryQuery.TableNoTracking.FirstOrDefault();
//        //        var _res = getJournalEntryByFinancialAccountCodeResponseDto.GetGenericPagination(costCenterData, pageNumber, pageSize, Mapper);

//        //        var res = new PaginationResponse()
//        //        {
//        //            Data = _res.Data.Select(x => new
//        //            {
//        //                id = x.journalEntry.Id,
//        //                code = x.journalEntry.Code,
//        //                time = x.journalEntry.FTDate != null ? x.journalEntry.FTDate.Value.ToString(defultData.datetimeFormat) : null,
//        //                credit = roundNumbers.GetRoundNumber(x.Credit),
//        //                debit = roundNumbers.GetRoundNumber(x.Debit),
//        //                balanceAfterOperation = roundNumbers.GetRoundNumber(journalEntryHelper.countTotal(x.Id, financialId, costCenterData)),
//        //            }).Select(x => new
//        //            {
//        //                x.id,
//        //                x.code,
//        //                x.credit,
//        //                x.debit,
//        //                x.time,
//        //                balanceAfterOperation = x.balanceAfterOperation < 0 ? (x.balanceAfterOperation * -1) : x.balanceAfterOperation,
//        //                FA_Nature = x.balanceAfterOperation > 0 ? (int)FA_Nature.Credit : (int)FA_Nature.Debit
//        //            }),
//        //            ListCount = _res.ListCount,
//        //            PageNumber = _res.PageNumber,
//        //            PageSize = _res.PageSize,
//        //            TotalPages = _res.TotalPages
//        //        };

//        //        #region old method
//        //        //var list = new List<JournalEntriesForFinancialDto>();
//        //        //double debit = 0;
//        //        //double credit = 0;
//        //        //double total = 0;


//        //        //foreach (var item in costCenterData)
//        //        //{
//        //        //    var dto = new JournalEntriesForFinancialDto();
//        //        //    // var journalEntry = await journalEntryRepositoryQuery.GetByAsync(s => s.Id == item.JournalEntryId);
//        //        //    dto.Id = item.JournalEntryId;
//        //        //    dto.Code = item.journalEntry.Code;
//        //        //    dto.Time = Convert.ToDateTime(item.journalEntry.FTDate).ToShortDateString();
//        //        //    dto.Credit = item.Credit;
//        //        //    credit = dto.Credit;
//        //        //    dto.Debit = item.Debit;
//        //        //    debit = dto.Debit;

//        //        //    //By Alaa
//        //        //    var sum = credit - debit;
//        //        //    dto.BalanceAfterOperation = total + (sum);
//        //        //    total += sum;
//        //        //    if (dto.BalanceAfterOperation > 0)
//        //        //        dto.FA_Nature = (int)FA_Nature.Credit;
//        //        //    else
//        //        //    {
//        //        //        dto.FA_Nature = (int)FA_Nature.Debit;
//        //        //        dto.BalanceAfterOperation *= -1;

//        //        //    }

//        //        //    /* if (credit !=0 )
//        //        //     {
//        //        //         if (total==0)
//        //        //         { 
//        //        //             dto.BalanceAfterOperation = credit;
//        //        //             total = dto.BalanceAfterOperation;
//        //        //         }
//        //        //         else
//        //        //         {
//        //        //             if (list.Last().Debit!=0)
//        //        //             {
//        //        //                 dto.BalanceAfterOperation = total - credit;
//        //        //                 total = dto.BalanceAfterOperation;
//        //        //             }
//        //        //             else
//        //        //             {
//        //        //                 dto.BalanceAfterOperation = total + credit;
//        //        //                 total = dto.BalanceAfterOperation;
//        //        //             }
//        //        //         }
//        //        //         dto.FA_Nature = 2;
//        //        //     }
//        //        //     if (debit != 0)
//        //        //     {
//        //        //         if (total == 0)
//        //        //         {
//        //        //             dto.BalanceAfterOperation = debit;
//        //        //             total = dto.BalanceAfterOperation;
//        //        //         }
//        //        //         else
//        //        //         {
//        //        //             if (list.Last().Debit != 0)
//        //        //             {
//        //        //                 dto.BalanceAfterOperation = total + debit;
//        //        //                 total = dto.BalanceAfterOperation;
//        //        //             }
//        //        //             else
//        //        //             {
//        //        //                 dto.BalanceAfterOperation = total - debit;
//        //        //                 total = dto.BalanceAfterOperation;
//        //        //             }
//        //        //         }
//        //        //         dto.FA_Nature = 1;
//        //        //     }*/
//        //        //    list.Add(dto);
//        //        //}
//        //        #endregion

//        //        return repositoryActionResult.GetRepositoryActionResult(res, RepositoryActionStatus.Ok, message: "Ok");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return repositoryActionResult.GetRepositoryActionResult(ex);

//        //    }
//        //}


//        //public async Task<IRepositoryActionResult> UpdateJournalEntryTransfer(UpdateTransfer parameter)
//        //{
//        //    try
//        //    {
//        //        foreach (var item in parameter.Id)
//        //        {
//        //            var journalentry = await journalEntryRepositoryQuery.GetByAsync(q => q.Id == item);
//        //            journalentry.IsTransfer = (parameter.IsTransfer == 1 ? true : false);
//        //            await journalEntryRepositoryCommand.UpdateAsyn(journalentry);
//        //            journalEntryRepositoryCommand.SaveChanges();
//        //            journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, _iUserInformation, journalentry.BranchId, journalentry.Code, journalentry.Id, journalentry.BrowserName, journalentry.LastTransactionAction, journalentry.LastTransactionUser);
//        //        }

//        //        return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.Updated, message: "Updated Successfully");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return repositoryActionResult.GetRepositoryActionResult(ex);
//        //    }
//        //}
//        //public async Task<ResponseResult> GetAllJournalEntryHistory(int JournalEntryId)
//        //{
//        //    var parentDatasQuey = recHistoryRepositoryQuery.FindQueryable(s => s.JournalEntryId == JournalEntryId).Include(a => a.employees);
//        //    var historyList = new List<HistoryResponceDto>();
//        //    foreach (var item in parentDatasQuey.ToList())
//        //    {
//        //        var historyDto = new HistoryResponceDto();

//        //        HistoryActionsNames actionName = HistoryActionsAliasNames.HistoryName[item.LastAction];


//        //        historyDto.Date = item.LastDate;

//        //        historyDto.TransactionTypeAr = actionName.ArabicName;
//        //        historyDto.TransactionTypeEn = actionName.LatinName;
//        //        historyDto.LatinName = item.employees.LatinName;
//        //        historyDto.ArabicName = item.employees.ArabicName;
//        //        historyDto.BrowserName = item.BrowserName;

//        //        historyList.Add(historyDto);
//        //    }
//        //    return new ResponseResult() { Data = historyList, Id = null, Result = Result.Success };

//        //    //return repositoryActionResult.GetRepositoryActionResult(historyList, RepositoryActionStatus.Ok, message: "Ok");
//        //}
//        //public async Task<IRepositoryActionResult> addEntryFunds(addEntryFunds parameter, int docId = -1, int Fund_FAId = 0)
//        //{
//        //    if (_generalSettingRepositoryQuery.TableNoTracking.FirstOrDefault().isFundsClosed)
//        //        return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "القيود الافتتاحية مغلفة", ErrorMessageEn = "The Funds is closed" }, RepositoryActionStatus.BadRequest, message: "The Funds is closed");
//        //    var balance = parameter.EntryFunds.Select(x => x.Debit).Sum() - parameter.EntryFunds.Select(x => x.Credit).Sum();
//        //    if (balance != 0 && docId == -1)
//        //        return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Note = "The constraint is unbalanced", Result = Result.Failed, ErrorMessageAr = "هذا القيد غير متزن", ErrorMessageEn = "The constraint is unbalanced" }, RepositoryActionStatus.BadRequest, message: "The constraint is unbalanced");
//        //    var findAccounts = financialAccountRepositoryQuery.TableNoTracking.Where(x => parameter.EntryFunds.Select(c => c.FinancialAccountId).Contains(x.Id)).Count();

//        //    if (findAccounts != parameter.EntryFunds.ToList().Select(x => x.FinancialAccountId).Distinct().Count())
//        //        return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "بعض الحسابات المالية غير موجودة", ErrorMessageEn = "Finanical Accounts not found" }, RepositoryActionStatus.BadRequest, message: "The constraint is unbalanced");
//        //    var entryFunds = journalEntryRepositoryQuery.GetAll().Where(x => x.Id == docId).FirstOrDefault();
//        //    if (entryFunds != null)
//        //    {
//        //        if (!parameter.isFund)
//        //            entryFunds.FTDate = parameter.date;
//        //        //var sumCredit  = parameter.EntryFunds.Select(x => x.Credit).Sum();
//        //        //var sumDebit = parameter.EntryFunds.Select(x => x.Debit).Sum();
//        //        //entryFunds.CreditTotal = sumCredit;
//        //        //entryFunds.DebitTotal = sumDebit;
//        //        //entryFunds.Notes = "قيد افتتاحي";
//        //        var lastEntrryFunds = journalEntryDetailsRepositoryQuery.TableNoTracking.Where(x => x.JournalEntryId == docId && !x.isStoreFund);
//        //        if (lastEntrryFunds.Any() && !parameter.isFund)
//        //        {
//        //            journalEntryDetailsRepositoryCommand.RemoveRange(lastEntrryFunds);
//        //            await journalEntryDetailsRepositoryCommand.SaveAsync();
//        //        }

//        //        var gLJournalEntryDetails = Mapping.Mapper.Map<List<EntryFunds>, List<GLJournalEntryDetails>>(parameter.EntryFunds);
//        //        journalEntryDetailsRepositoryCommand.AddRange(gLJournalEntryDetails);
//        //        var saved = await journalEntryDetailsRepositoryCommand.SaveAsync();



//        //        if (docId != -1)
//        //        {
//        //            var fundAccount = await journalEntryDetailsRepositoryQuery.GetAllAsyn();
//        //            fundAccount = fundAccount.Where(x => x.JournalEntryId == docId).ToList();
//        //            if (fundAccount.Where(x => x.FinancialAccountId == Fund_FAId).Any())
//        //            {
//        //                var lis = fundAccount.Where(x => x.FinancialAccountId == Fund_FAId && x.StoreFundId == null);
//        //                foreach (var item in lis)
//        //                {
//        //                    await journalEntryDetailsRepositoryCommand.DeleteAsync(item.Id);
//        //                }
//        //                //await journalEntryDetailsRepositoryCommand.SaveAsync();
//        //            }
//        //            var ActionsList = Actions.fundLists().Where(x => x.id == docId).FirstOrDefault();
//        //            var amount = fundAccount.Where(x => x.StoreFundId != null).Sum(x => x.Credit - x.Debit);
//        //            var fund_re = new EntryFunds()
//        //            {

//        //                Credit = amount < 0 ? amount * -1 : 0,
//        //                Debit = amount > 0 ? amount : 0,
//        //                DescriptionAr = ActionsList.descAr,
//        //                DescriptionEn = ActionsList.descEn,
//        //                FinancialAccountId = Fund_FAId,
//        //                JournalEntryId = parameter.EntryFunds.FirstOrDefault().JournalEntryId,
//        //                isStoreFund = true,

//        //                DocType = parameter.EntryFunds.FirstOrDefault().DocType
//        //            };
//        //            var _fund_re = Mapping.Mapper.Map<EntryFunds, GLJournalEntryDetails>(fund_re);
//        //            journalEntryDetailsRepositoryCommand.Add(_fund_re);
//        //            await journalEntryDetailsRepositoryCommand.SaveAsync();
//        //        }
//        //        saved = await journalEntryRepositoryCommand.UpdateAsyn(entryFunds);

//        //        if (saved)
//        //            return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Result = Result.Success }, RepositoryActionStatus.Ok, message: "Ok");
//        //        else
//        //            return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Result = Result.Failed }, RepositoryActionStatus.BadRequest, message: "BadRequest");
//        //    }
//        //    journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, httpContext, _iUserInformation, -1, (int)HistoryTitle.Add, "A");
//        //    if (docId == -1)
//        //        await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editOpeningBalance);
//        //    return repositoryActionResult.GetRepositoryActionResult(new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "بعض الحسابات المالية غير موجودة", ErrorMessageEn = "Finanical Accounts not found" }, RepositoryActionStatus.BadRequest, message: "The constraint is not exist");
//        //}
//        //public async Task<bool> removeStoreFundFromJournalDetiales(int[] storeFundIds, int DocType)
//        //{
//        //    var _storeFundJournalEntry = journalEntryDetailsRepositoryQuery.TableNoTracking.Where(x => x.DocType == DocType);

//        //    var storeFundJournalEntry = _storeFundJournalEntry.Where(x => storeFundIds.Contains(x.StoreFundId ?? 0));
//        //    if (storeFundJournalEntry.Any())
//        //    {
//        //        journalEntryDetailsRepositoryCommand.RemoveRange(storeFundJournalEntry);
//        //        var deleted = await journalEntryDetailsRepositoryCommand.SaveAsync();
//        //        var MainAccount = _storeFundJournalEntry.Where(x => x.StoreFundId == null).FirstOrDefault();
//        //        var balance = _storeFundJournalEntry.Where(x => x.StoreFundId != null).Sum(x => x.Credit - x.Debit);
//        //        if (balance == 0)
//        //        {
//        //            journalEntryDetailsRepositoryCommand.Remove(MainAccount);
//        //            await journalEntryDetailsRepositoryCommand.SaveAsync();
//        //            return deleted;

//        //        }
//        //        MainAccount.Credit = balance < 0 ? (balance * -1) : 0;
//        //        MainAccount.Debit = balance < 0 ? 0 : balance;
//        //        await journalEntryDetailsRepositoryCommand.UpdateAsyn(MainAccount);
//        //        return deleted;
//        //    }

//        //    return true;

//        //}
//    }
//}
