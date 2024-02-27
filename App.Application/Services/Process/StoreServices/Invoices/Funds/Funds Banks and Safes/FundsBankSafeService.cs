using App.Application.Handlers.GeneralLedger.JournalEntry;
using App.Application.Handlers.Helper.SafesBanksFundService;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using MediatR;
using Delete = App.Domain.Models.Security.Authentication.Request.Delete;

namespace App.Application.Services.Process.Funds_Banks_and_Safes
{
    public class FundsBankSafeService : BaseClass, IFundsBankSafeService
    {
        private readonly IRepositoryQuery<InvFundsBanksSafesMaster> fundsMasterQuery;
        private readonly IRepositoryCommand<InvFundsBanksSafesMaster> fundsMasterCommand;
        private readonly IRepositoryCommand<GLJournalEntry> _gLJournalEntryCommand;
        private readonly IRepositoryQuery<InvFundsBanksSafesDetails> fundsDetailsQuery;
        private readonly IRepositoryQuery<GlReciepts> _glRecieptsQuery;
        private readonly IRepositoryQuery<GLJournalEntry> _gLJournalEntryQuery;
        private readonly IRepositoryQuery<GLBank> _gLBankQuery;
        private readonly IRepositoryQuery<GLSafe> _gLSafeQuery;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _gLPurchasesAndSalesSettingsQuery;
        private readonly IRepositoryQuery<InvPaymentMethods> _invPaymentMethodsQuery;
        private readonly IRepositoryCommand<GlReciepts> _receiptCommand;
        private readonly IRepositoryCommand<InvFundsBanksSafesDetails> fundsDetailsCommand;
        private readonly IReceiptsService _receiptsService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<InvFundsBanksSafesHistory> history;
        private readonly iUserInformation Userinformation;
        private readonly IMediator _mediator;

        public FundsBankSafeService(IRepositoryQuery<InvFundsBanksSafesMaster> _fundsMasterQuery,
                                    IRepositoryCommand<InvFundsBanksSafesMaster> _fundsMasterCommand,
                                    IRepositoryCommand<GLJournalEntry> GLJournalEntryCommand,
                                     IRepositoryQuery<InvFundsBanksSafesDetails> _fundsDetailsQuery,
                                     IRepositoryQuery<GlReciepts> GlRecieptsQuery,
                                     IRepositoryQuery<GLJournalEntry> GLJournalEntryQuery,
                                     IRepositoryQuery<GLBank> GLBankQuery,
                                     IRepositoryQuery<GLSafe> GLSafeQuery,
                                     IRepositoryQuery<GLPurchasesAndSalesSettings> GLPurchasesAndSalesSettingsQuery,
                                     IRepositoryQuery<InvPaymentMethods> InvPaymentMethodsQuery,
                                     IRepositoryCommand<GlReciepts> receiptCommand,
                                     IRepositoryCommand<InvFundsBanksSafesDetails> _fundsDetailsCommand,
                                     IReceiptsService receiptsService,
                                     iAuthorizationService iAuthorizationService,
                                     ISystemHistoryLogsService systemHistoryLogsService,
                                     IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery,
                                     IHistory<InvFundsBanksSafesHistory> history, iUserInformation Userinformation,
                                     IHttpContextAccessor _httpContext
, IMediator mediator
            ) : base(_httpContext)
        {
            fundsMasterQuery = _fundsMasterQuery;
            fundsMasterCommand = _fundsMasterCommand;
            _gLJournalEntryCommand = GLJournalEntryCommand;
            fundsDetailsQuery = _fundsDetailsQuery;
            _glRecieptsQuery = GlRecieptsQuery;
            _gLJournalEntryQuery = GLJournalEntryQuery;
            _gLBankQuery = GLBankQuery;
            _gLSafeQuery = GLSafeQuery;
            //_gLJournalEntryDetailsQuery = GLJournalEntryDetailsQuery;
            _gLPurchasesAndSalesSettingsQuery = GLPurchasesAndSalesSettingsQuery;
            _invPaymentMethodsQuery = InvPaymentMethodsQuery;
            _receiptCommand = receiptCommand;
            fundsDetailsCommand = _fundsDetailsCommand;
            _receiptsService = receiptsService;
            _iAuthorizationService = iAuthorizationService;
            _systemHistoryLogsService = systemHistoryLogsService;
            _invGeneralSettingsQuery = InvGeneralSettingsQuery;
            httpContext = _httpContext;
            this.history = history;
            this.Userinformation = Userinformation;
            _mediator = mediator;
        }

        #region Helper
        public async Task<bool> CheckFundSettings(bool isBank)
        {
            var settings = await _invGeneralSettingsQuery.GetByIdAsync(1);
            if (isBank)
                return settings.Funds_Banks;
            else
                return settings.Funds_Safes;
        }
        public async Task deleteRecAndJournalEntry(int[] Ids, int DocType, bool fromDeleteApi = false)
        {
            var findRecs = await _glRecieptsQuery.GetAllAsyn(x => Ids.Contains(x.EntryFundId ?? 0));
            if (findRecs.Any())
            {
                if (!fromDeleteApi)
                {
                    _receiptCommand.RemoveRange(findRecs);
                    var RecDeleted = await _receiptCommand.SaveAsync();
                }
                else
                {
                    findRecs.ToList().ForEach(x => x.IsBlock = true);
                    await _receiptCommand.UpdateAsyn(findRecs);
                }
            }
        }
        public async Task SetDateOfJournalEntry(DateTime date, bool isSafe)
        {
            int docId = isSafe ? -4 : -5;
            var JournalEntry = await _gLJournalEntryQuery.GetByIdAsync(docId);
            JournalEntry.FTDate = date;
            bool saved = await _gLJournalEntryCommand.UpdateAsyn(JournalEntry);
            _gLJournalEntryCommand.ClearTracking();
        }
        //public async Task _relationsWithRecAndJournalEntry(List<InvFundsBanksSafesDetails> fundsDetailsList, InvFundsBanksSafesMaster table
        //    , UserInformationModel userInfo, bool isUpdate)
        //{

        //    //Reciepts
        //    int FA_Id = 0;
        //    string recNoteAr = "";
        //    string recNoteEn = "";
        //    double amount = 0;
        //    int ParentTypeId = 0;
        //    int RecieptTypeId = 0;
        //    if (table.IsBank)
        //    {
        //        FA_Id = _gLBankQuery.TableNoTracking.Where(x => x.Id == table.BankId).FirstOrDefault().FinancialAccountId ?? 0;
        //        recNoteAr = "ارصدة اول المدة البنوك" + "-" + table.Code.ToString();
        //        recNoteEn = "Entry Fund Bank" + "-" + table.Code.ToString();
        //    }
        //    else
        //    {
        //        FA_Id = _gLSafeQuery.TableNoTracking.Where(x => x.Id == table.SafeId).FirstOrDefault().FinancialAccountId ?? 0;
        //        recNoteAr = "ارصدة اول المدة الخزائن" + "-" + table.Code.ToString();
        //        recNoteEn = "Entry Fund Safe" + "-" + table.Code.ToString();
        //    }

        //    var rec = new List<GlReciepts>();
        //    foreach (var item in fundsDetailsList)
        //    {

        //        ParentTypeId = table.IsBank ? (int)DocumentType.BankFunds : (int)DocumentType.SafeFunds;
        //        amount = item.Creditor - item.Debtor;
        //        if (table.IsBank)
        //        {
        //            if (amount < 0)
        //                RecieptTypeId = (int)DocumentType.BankCash;
        //            else if (amount > 0)
        //                RecieptTypeId = (int)DocumentType.BankPayment;
        //        }
        //        else if (!table.IsBank)
        //        {
        //            if (amount < 0)
        //                RecieptTypeId = (int)DocumentType.SafeCash;
        //            else if (amount > 0)
        //                RecieptTypeId = (int)DocumentType.SafePayment;
        //        }


        //        if (isUpdate)
        //        {
        //            await deleteRecAndJournalEntry(new int[] { table.DocumentId }, ParentTypeId);

        //        }

        //        var rec2 = new GlReciepts()
        //        {
        //            Amount = amount,
        //            Creditor = item.Creditor,
        //            Debtor = item.Debtor,
        //            PaymentMethodId = item.PaymentId,
        //            BenefitId = table.IsBank ? table.BankId ?? 0 : table.SafeId ?? 0,
        //            Code = table.Code, // find
        //            FinancialAccountId = FA_Id,
        //            IsAccredit = false,
        //            Notes = "",
        //            CreationDate = DateTime.Now,
        //            BranchId = userInfo.CurrentbranchId,
        //            SafeID = table.SafeId,
        //            BankId = table.BankId,
        //            ParentTypeId = ParentTypeId,
        //            Authority = (int)AuthorityTypes.DirectAccounts,
        //            NoteAR = recNoteAr,
        //            NoteEN = recNoteEn,
        //            RecieptDate = table.DocDate,
        //            UserId = userInfo.userId,
        //            RecieptTypeId = RecieptTypeId,     //find
        //            Signal = amount < 0 ? 1 : -1,
        //            RecieptType = table.Code.ToString(), // find
        //            ChequeNumber = "",
        //            Serialize = 0,
        //            EntryFundId = table.DocumentId
        //        };
        //        rec.Add(rec2);
        //    }

        //    _receiptCommand.AddRange(rec);
        //    bool saved = await _receiptCommand.SaveAsync();
        //    if (saved)
        //    {
        //        var GLSettings = _gLPurchasesAndSalesSettingsQuery.TableNoTracking.Where(x => x.branchId == userInfo.CurrentbranchId).Where(x => x.MainType == 4 && (table.IsBank ? x.RecieptsType == 29 : x.RecieptsType == 28));
        //        int Fund_FAId = GLSettings.OrderBy(x => x.Id).LastOrDefault().FinancialAccountId ?? 0;
        //        var entryDetails = new List<JournalEntryDetail>();
        //        var totalAmount = fundsDetailsList.Sum(x => x.Creditor - x.Debtor);

        //        entryDetails.AddRange(new[]
        //        {
        //            new JournalEntryDetail
        //            {
        //                Credit = totalAmount < 0 ? Math.Abs(totalAmount) : 0,
        //                Debit = totalAmount > 0 ? Math.Abs(totalAmount) : 0,
        //                DescriptionAr = recNoteAr,
        //                DescriptionEn = recNoteEn,
        //                FinancialAccountId = Fund_FAId,
        //            },
        //            new JournalEntryDetail
        //            {
        //                Credit = totalAmount > 0 ? Math.Abs(totalAmount) : 0,
        //                Debit = totalAmount < 0 ? Math.Abs(totalAmount) : 0,
        //                DescriptionAr = recNoteAr,
        //                DescriptionEn = recNoteEn,
        //                FinancialAccountId =FA_Id,
        //            }

        //        });

        //        var note = table.IsBank ? $"ارصدة اول المدة بنوك {table.Code}" : $"ارصدة اول المدة خزائن {table.Code}";
        //        if (!isUpdate)
        //        {

        //            await _mediator.Send(new AddJournalEntryRequest
        //            {
        //                BranchId = userInfo.CurrentbranchId,
        //                FTDate = table.DocDate,
        //                InvoiceId = table.DocumentId,
        //                DocType = table.IsBank ? (int)DocumentType.BankFunds : (int)DocumentType.SafeFunds,
        //                isAuto = true,
        //                IsAccredit = true,
        //                JournalEntryDetails = entryDetails,
        //                Notes = note,
        //            });
        //            //await _journalEntryBusiness.AddJournalEntry(new JournalEntryParameter()
        //            //{
        //            //    BranchId = userInfo.CurrentbranchId,
        //            //    FTDate = table.DocDate,
        //            //    InvoiceId = table.DocumentId,
        //            //    DocType = table.IsBank ? (int)DocumentType.BankFunds : (int)DocumentType.SafeFunds,
        //            //    isAuto = true,
        //            //    IsAccredit = true,
        //            //    JournalEntryDetails = entryDetails,
        //            //    Notes = note,
        //            //});
        //        }
        //        else
        //        {
        //            var jeId = _gLJournalEntryQuery.TableNoTracking.Where(x => x.InvoiceId == table.DocumentId && x.DocType == (table.IsBank ? (int)DocumentType.BankFunds : (int)DocumentType.SafeFunds)).FirstOrDefault()?.Id ?? 0;
        //            await _mediator.Send(new UpdateJournalEntryRequest
        //            {
        //                BranchId = userInfo.CurrentbranchId,
        //                FTDate = table.DocDate,
        //                fromSystem = true,
        //                IsAccredit = true,
        //                Notes = note,
        //                journalEntryDetails = entryDetails,
        //                Id = jeId
        //            });
        //            //await _journalEntryBusiness.UpdateJournalEntry(new UpdateJournalEntryParameter()
        //            //{
        //            //    BranchId = userInfo.CurrentbranchId,
        //            //    FTDate = table.DocDate,
        //            //    fromSystem = true,
        //            //    IsAccredit = true,
        //            //    Notes = note,
        //            //    journalEntryDetails = entryDetails,
        //            //    Id = jeId
        //            //});
        //        }

        //    }
        //}
        #endregion



        public async Task<ResponseResult> AddFundsBankSafe(FundsBankSafeRequest parameter)
        {
            if (CheckFundSettings(parameter.IsBank).Result)
                return new ResponseResult()
                {
                    Note = parameter.IsBank ? Actions.BankFundsIsClosed : Actions.safeFundsIsClosed,
                    Result = Result.Failed
                };
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult()
                {
                    Note = Actions.JWTError,
                    Result = Result.Failed
                };

            if (parameter.FundsDetails_B_S == null || parameter.FundsDetails_B_S.Count == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData };

            var NextCode = 1;
            var Bank = fundsMasterQuery.TableNoTracking.Where(a => a.IsBank == true).Count();
            var safe = fundsMasterQuery.TableNoTracking.Where(a => a.IsSafe == true).Count();

            if (parameter.IsBank && Bank > 0)
                NextCode = fundsMasterQuery.GetMaxCode(e => e.Code, a => a.IsBank == true) + 1;
            else if (!parameter.IsBank && safe > 0)
                NextCode = fundsMasterQuery.GetMaxCode(e => e.Code, a => a.IsSafe == true) + 1;



            var table = new InvFundsBanksSafesMaster();

            table.Code = NextCode;
            table.DocDate = parameter.DocDate;
            if (parameter.IsBank)
                table.BankId = parameter.SafeBankId;
            else
                table.SafeId = parameter.SafeBankId;
            table.IsBank = parameter.IsBank;
            table.IsSafe = !parameter.IsBank;
            table.BranchId = userInfo.CurrentbranchId;
            table.Notes = parameter.Notes;



            fundsMasterCommand.Add(table);
            var fundsDetailsList = new List<InvFundsBanksSafesDetails>();
            foreach (var item in parameter.FundsDetails_B_S)
            {
                if (fundsDetailsList.Where(x => x.PaymentId == item.paymentMethod).Any())
                    return new ResponseResult()
                    {
                        Note = Actions.CannotAddSamePaymentMethod,
                        Result = Result.Failed
                    };
                if ((item.Creditor - item.Debtor) == 0)
                    return new ResponseResult()
                    {
                        Note = Actions.ValuesCanNotBeZero,
                        Result = Result.Failed
                    };
                var fundsDetails = new InvFundsBanksSafesDetails()
                {
                    DocumentId = table.DocumentId,
                    PaymentId = item.paymentMethod,
                    Debtor = item.Debtor ?? 0,
                    Creditor = item.Creditor ?? 0
                };
                fundsDetailsList.Add(fundsDetails);
            }
            fundsDetailsCommand.AddRange(fundsDetailsList);
            await fundsDetailsCommand.SaveAsync();
            await _mediator.Send(new SafesBanksFundServiceRequest
            {
                fundsDetailsList = fundsDetailsList,
                table = table,
                userInfo= userInfo,
                isUpdate = false,
            });
            //await relationsWithRecAndJournalEntry(fundsDetailsList, table, userInfo, false);


            history.AddHistory(table.DocumentId, "", "", Aliases.HistoryActions.Add, userInfo.username.ToString());
            await _systemHistoryLogsService.SystemHistoryLogsService(parameter.IsBank ? SystemActionEnum.addBanksFund : SystemActionEnum.addSafesFund);
            return new ResponseResult() { Data = null, Id = table.DocumentId, Result = Result.Success };

        }
        public async Task<ResponseResult> UpdateFundsBankSafe(UpdateFundsBankSafeRequest parameter)
        {
            if (CheckFundSettings(parameter.IsBank).Result)
                return new ResponseResult()
                {
                    Note = parameter.IsBank ? Actions.BankFundsIsClosed : Actions.safeFundsIsClosed,
                    Result = Result.Failed
                };
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult()
                {
                    Note = Actions.JWTError,
                    Result = Result.Failed
                };
            if (parameter.DocumentId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameter.FundsDetails_B_S == null || parameter.FundsDetails_B_S.Count == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData };

            var data = await fundsMasterQuery.GetByAsync(a => a.DocumentId == parameter.DocumentId && a.IsBank == parameter.IsBank && a.IsSafe == !parameter.IsBank);
            if (data.isBlock)
                return new ResponseResult() { Note = Actions.CannotEditDeletedItems, Result = Result.Failed };
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            //data.Code = parameter.Code;
            data.DocDate = parameter.DocDate;
            if (parameter.IsBank)
                data.BankId = parameter.SafeBankId;
            else
                data.SafeId = parameter.SafeBankId;

            data.IsBank = parameter.IsBank;
            data.IsSafe = !parameter.IsBank;
            data.BranchId = userInfo.CurrentbranchId;
            data.Notes = parameter.Notes;

            await fundsMasterCommand.UpdateAsyn(data);
            await fundsDetailsCommand.DeleteAsync(a => a.DocumentId == parameter.DocumentId);

            var fundsDetailsList = new List<InvFundsBanksSafesDetails>();
            foreach (var item in parameter.FundsDetails_B_S)
            {
                if (fundsDetailsList.Where(x => x.PaymentId == item.paymentMethod).Any())
                    return new ResponseResult()
                    {
                        Note = Actions.CannotAddSamePaymentMethod,
                        Result = Result.Failed
                    };
                if ((item.Creditor - item.Debtor) == 0)
                    return new ResponseResult()
                    {
                        Note = Actions.ValuesCanNotBeZero,
                        Result = Result.Failed
                    };
                var fundsDetails = new InvFundsBanksSafesDetails()
                {
                    DocumentId = data.DocumentId,
                    PaymentId = item.paymentMethod,
                    Debtor = item.Debtor ?? 0,
                    Creditor = item.Creditor ?? 0
                };
                fundsDetailsList.Add(fundsDetails);
            }
            fundsDetailsCommand.AddRange(fundsDetailsList);
            await fundsDetailsCommand.SaveAsync();
            await _mediator.Send(new SafesBanksFundServiceRequest
            {
                fundsDetailsList = fundsDetailsList,
                table = data,
                userInfo = userInfo,
                isUpdate = true,
            });
            //await relationsWithRecAndJournalEntry(fundsDetailsList, data, userInfo, true);
            history.AddHistory(data.DocumentId, "", "", Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(parameter.IsBank ? SystemActionEnum.editBanksFund : SystemActionEnum.editSafesFund);
            return new ResponseResult() { Data = null, Id = data.DocumentId, Result = Result.Success };
        }
        public async Task<ResponseResult> DeleteFundsBankSafe(Delete ListCode)
        {
            var userInfo = await Userinformation.GetUserInformation();
            var funds = await fundsMasterQuery.GetAllAsyn(x => ListCode.Ids.Contains(x.DocumentId));
            var isBanks = funds.Where(x => x.IsBank).Any();
            var isSafe = funds.Where(x => x.IsSafe).Any();
            if (isBanks)
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, (int)SubFormsIds.Banks_Fund, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            if (isSafe)
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, (int)SubFormsIds.Safes_Fund, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }

            if (CheckFundSettings(isBanks).Result)
                return new ResponseResult()
                {
                    Note = Actions.BankFundsIsClosed,
                    Result = Result.Failed
                };

            if (CheckFundSettings(!isBanks).Result)
                return new ResponseResult()
                {
                    Note = Actions.BankFundsIsClosed,
                    Result = Result.Failed
                };

            // funds.ToList().ForEach(x => x.isBlock = true);

            foreach (var item in funds)
            {
                item.isBlock = true;
                history.AddHistory(item.DocumentId, "", "", Aliases.HistoryActions.Delete, Aliases.TemporaryRequiredData.UserName);
            }



            var settings = _gLPurchasesAndSalesSettingsQuery.TableNoTracking.Where(x => x.branchId == userInfo.CurrentbranchId);
            var bankFAId = settings.Where(x => x.RecieptsType == (int)DocumentType.BankFunds).FirstOrDefault().FinancialAccountId;
            var bankFunds = funds.Where(x => x.IsBank == true).Select(x => x.DocumentId).ToArray();



            var SafeFAId = settings.Where(x => x.RecieptsType == (int)DocumentType.SafeFunds).FirstOrDefault().FinancialAccountId;
            var SafeFunds = funds.Where(x => x.IsSafe == true).Select(x => x.DocumentId).ToArray();

            if (bankFunds.Any())
                await deleteRecAndJournalEntry(bankFunds, (int)DocumentType.BankFunds, true);
            if (SafeFunds.Any())
                await deleteRecAndJournalEntry(SafeFunds, (int)DocumentType.BankFunds, true);

            var BanksJeIds = _gLJournalEntryQuery.TableNoTracking.Where(x => x.DocType == (int)DocumentType.BankFunds && bankFunds.Contains(x.InvoiceId ?? 0)).Select(c => c.Id).ToArray();
            var SafeJeIds = _gLJournalEntryQuery.TableNoTracking.Where(x => x.DocType == (int)DocumentType.SafeFunds && SafeFunds.Contains(x.InvoiceId ?? 0)).Select(c => c.Id).ToArray();
            var ids = new List<int>();
            ids.AddRange(BanksJeIds);
            ids.AddRange(SafeJeIds);
            await _mediator.Send(new BlockJournalEntryReqeust
            {
                Ids = ids.ToArray()
            });
            //await _journalEntryBusiness.BlockJournalEntry(new BlockJournalEntry()
            //{
            //    Ids = ids.ToArray()
            //});


            await fundsMasterCommand.UpdateAsyn(funds);
            await _systemHistoryLogsService.SystemHistoryLogsService(isBanks ? SystemActionEnum.deleteBanksFund : SystemActionEnum.deleteSafesFund,funds.Count);

            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }
        public async Task<ResponseResult> GetFundsBankSafeHistory(int documentId)
        {
            var res = await history.GetHistory(a => a.EntityId == documentId);

            var fund = fundsMasterQuery.TableNoTracking.Where(x => x.DocumentId == documentId).FirstOrDefault();
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, fund.IsBank ? (int)SubFormsIds.Banks_Fund : (int)SubFormsIds.Safes_Fund, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            return res;
        }
        public async Task<ResponseResult> GetListOfFundsBanksSafes(fundsSearch parameters)
        {
            if (CheckFundSettings(parameters.IsBank).Result)
                return new ResponseResult()
                {
                    Note = Actions.BankFundsIsClosed,
                    Result = Result.Failed
                };
            var resData = fundsMasterQuery.TableNoTracking
                                            .Include(x => x.Safe)
                                            .Include(x => x.Bank)
                                            .Include(x => x.FundsDetails_B_S)
                                            .Where(x => (parameters.IsBank ? x.IsBank == true : x.IsSafe == true));
            var count = resData.Count();
            var paymentMehtod = _invPaymentMethodsQuery.TableNoTracking.Select(x => new
            {
                Id = x.PaymentMethodId,
                x.ArabicName,
                x.LatinName
            });
            if (parameters.id != null)
                resData = resData.Where(x => (parameters.id != 0 ? x.DocumentId == parameters.id : 1 == 1));
            string[] safeOrBanksArr = { };
            if (!string.IsNullOrEmpty(parameters.SafeOrBankList))
                safeOrBanksArr = parameters.SafeOrBankList.Split(',');

            var _response = resData.OrderByDescending(x => x.DocumentId).Where(x => 1 == 1);

            if (parameters.SafeOrBankList != null && safeOrBanksArr.Count() > 0)
            {
                if (parameters.IsBank)
                    _response = resData.Where(e => safeOrBanksArr.Contains(e.BankId.ToString()));
                else
                    _response = resData.Where(e => safeOrBanksArr.Contains(e.SafeId.ToString()));

            }
            var dataCount = _response.Count();
            if (!string.IsNullOrEmpty(parameters.searchCriteria))
            {
                _response = _response.Where(a => (parameters.IsBank ? (a.Bank.ArabicName.Contains(parameters.searchCriteria) || a.Bank.LatinName.Contains(parameters.searchCriteria)) :
                                                                    (a.Safe.ArabicName.Contains(parameters.searchCriteria) || a.Safe.LatinName.Contains(parameters.searchCriteria)))
                                                                     || a.Code.ToString().Contains(parameters.searchCriteria)).OrderBy(x => x.DocumentId);
                dataCount = _response.Count();
            }
            if (parameters.DateFrom != null && parameters.DateTo != null)
                _response = _response.Where(a => a.DocDate.Date >= parameters.DateFrom && a.DocDate.Date <= parameters.DateTo);


            if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            {
                _response = _response.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize);
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            var response = _response.Select(x => new
            {
                Id = x.DocumentId,
                documentNumber = x.Code,
                documentDate = x.DocDate,
                //safeOrBankId = x.IsBank ? x.Bank.Id : x.Safe.Id,
                arabicName = x.IsBank ? x.Bank.ArabicName : x.Safe.ArabicName,
                latinName = x.IsBank ? x.Bank.LatinName : x.Safe.LatinName,
                //FundsDetails = x.FundsDetails_B_S.Select(d=> new
                //{
                //    paymentMehtod = paymentMehtod.Where(c=> c.Id == d.PaymentId).FirstOrDefault(),
                //    d.Debtor,
                //    d.Creditor
                //}).FirstOrDefault(),
                //x.Notes,
                isDeleted = x.isBlock
            });
            return new ResponseResult() { TotalCount = count, Data = response, DataCount = dataCount, Id = null, Result = Result.Success };

        }

        public async Task<ResponseResult> GetFundsBanksSafesById(int id)
        {

            var resData = fundsMasterQuery.TableNoTracking
                                            .Include(x => x.Safe)
                                            .Include(x => x.Bank)
                                            .Include(x => x.FundsDetails_B_S)
                                            .Where(x => x.DocumentId == id);
            if (CheckFundSettings(resData.FirstOrDefault().IsBank).Result)
                return new ResponseResult()
                {
                    Note = resData.FirstOrDefault().IsBank ? Actions.BankFundsIsClosed : Actions.safeFundsIsClosed,
                    Result = Result.Failed
                };
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, resData.FirstOrDefault().IsBank ? (int)SubFormsIds.Banks_Fund : (int)SubFormsIds.Safes_Fund, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var paymentMehtod = _invPaymentMethodsQuery.TableNoTracking.Select(x => new
            {
                paymentMethodId = x.PaymentMethodId,
                x.ArabicName,
                x.LatinName
            });
            var response = resData.Select(x => new
            {
                Id = x.DocumentId,
                documentNumber = x.Code,
                documentDate = x.DocDate,
                safeOrBankId = x.IsBank ? x.Bank.Id : x.Safe.Id,
                arabicName = x.IsBank ? x.Bank.ArabicName : x.Safe.ArabicName,
                latinName = x.IsBank ? x.Bank.LatinName : x.Safe.LatinName,
                FundsDetails = x.FundsDetails_B_S.Select(d => new
                {
                    paymentMethod = paymentMehtod.Where(c => c.paymentMethodId == d.PaymentId).FirstOrDefault(),
                    d.Debtor,
                    d.Creditor
                }),
                x.Notes
            }).FirstOrDefault();
            return new ResponseResult() { Data = response, Id = null, Result = resData.Any() ? Result.Success : Result.NoDataFound };

        }
    }
}
