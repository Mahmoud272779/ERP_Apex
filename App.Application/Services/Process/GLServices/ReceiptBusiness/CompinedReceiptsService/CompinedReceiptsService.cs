using App.Application.Handlers.GeneralLedger.JournalEntry;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Response.GeneralLedger;
using App.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.GLServices.ReceiptBusiness.CompinedReceiptsService
{
    public class CompinedReceiptsService : BaseClass, ICompinedReceiptsService
    {
        private readonly iUserInformation _Userinformation;
        private readonly IRepositoryQuery<GlReciepts> receiptQuery;
        private readonly IRepositoryCommand<GlReciepts> receiptCommand;
        private readonly IRoundNumbers RoundNumbers;
        private readonly IReceiptsService ReceiptsService;
        private readonly IRepositoryQuery<GLFinancialAccount> FinancialAccountQuery;
        private readonly IRepositoryQuery<GLOtherAuthorities> OtherAuthorityQuery;
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly IRepositoryQuery<InvSalesMan> SalesManQuery;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IFilesOfInvoices filesOfInvoices;
        private readonly IRepositoryQuery<GLBank> bankQuery;
        private readonly IRepositoryQuery<GLSafe> safeQuery;
        private readonly IRepositoryQuery<InvPaymentMethods> PaymentMethodsQuery;
        //private readonly IJournalEntryBusiness journalEntryBusiness;
        private readonly IRepositoryQuery<GLJournalEntry> _JournalenteryQuery;
        private readonly IRepositoryCommand<GLJournalEntry> _JournalenteryCommand;
        private readonly IHistoryReceiptsService ReceiptsHistory;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;
        public CompinedReceiptsService(iUserInformation Userinformation,
                                       IRepositoryQuery<GlReciepts> _receiptQuery,
                                       IRoundNumbers _RoundNumbers,
                                       IReceiptsService _ReceiptsService,
                                       IRepositoryQuery<GLFinancialAccount> _FinancialAccountQuery,
                                       IRepositoryQuery<GLOtherAuthorities> _OtherAuthorityQuery,
                                       IRepositoryQuery<InvPersons> _PersonQuery,
                                       IRepositoryQuery<InvSalesMan> _SalesManQuery,
                                       IGeneralAPIsService _GeneralAPIsService,
                                       IRepositoryCommand<GlReciepts> _receiptCommand,
                                       IFilesOfInvoices _filesOfInvoices,
                                       IRepositoryQuery<GLBank> _bankQuery,
                                       IRepositoryQuery<GLSafe> _safeQuery,
                                       //IJournalEntryBusiness _journalEntryBusiness,
                                       IHistoryReceiptsService _ReceiptsHistory,
                                       ISystemHistoryLogsService systemHistoryLogsService,
                                       IRepositoryQuery<InvPaymentMethods> _PaymentMethodsQuery,
                                       IRepositoryQuery<GLJournalEntry> JournalenteryQuery,
                                       IRepositoryCommand<GLJournalEntry> JournalenteryCommand,
                                       iAuthorizationService iAuthorizationService,
            IHttpContextAccessor httpContextAccessor,
            IMediator mediator) : base(httpContextAccessor)
        {
            _Userinformation = Userinformation;
            RoundNumbers = _RoundNumbers;
            ReceiptsService = _ReceiptsService;
            FinancialAccountQuery = _FinancialAccountQuery;
            OtherAuthorityQuery = _OtherAuthorityQuery;
            PersonQuery = _PersonQuery;
            SalesManQuery = _SalesManQuery;
            GeneralAPIsService = _GeneralAPIsService;
            receiptQuery = _receiptQuery;
            receiptCommand = _receiptCommand;
            filesOfInvoices = _filesOfInvoices;
            bankQuery = _bankQuery;
            safeQuery = _safeQuery;
            //journalEntryBusiness = _journalEntryBusiness;
            ReceiptsHistory = _ReceiptsHistory;
            _systemHistoryLogsService = systemHistoryLogsService;
            PaymentMethodsQuery = _PaymentMethodsQuery;
            _JournalenteryQuery = JournalenteryQuery;
            _JournalenteryCommand = JournalenteryCommand;
            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
        }

        public async Task<ResponseResult> AddCompinedReceipt(CompinedRecieptsRequest parameter)
        {
            try
            {
                if (parameter.RecieptTypeId == 0)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "يجب ادخال نوع السند",
                        ErrorMessageEn = "The RecieptTypeId must be entered",
                    };

                if (IsSafe(parameter.RecieptTypeId) && parameter.SafeID == 0)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = ErrorMessagesAr.SelectSafe,
                        ErrorMessageEn = ErrorMessagesEn.SelectSafe,
                    };
                if (!IsSafe(parameter.RecieptTypeId) && parameter.BankId == 0)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "يجب ادخال بنك",
                        ErrorMessageEn = "The BankId must be entered"
                    };
                if (parameter.RecieptDate == null)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "يجب ادخال تاريخ",
                        ErrorMessageEn = "The RecieptDate must be entered"
                    };

                UserInformationModel userInfo = await _Userinformation.GetUserInformation();
                string TransActoin = HistoryActions.Add;
                
                List<GlReciepts> CReciepts = new List<GlReciepts>();
                List<financialData> financialDatas = new List<financialData>();

                DateTime recdate = GeneralAPIsService.serverDate((DateTime)parameter.RecieptDate);
                int code = Autocode(parameter.RecieptTypeId, parameter.BranchId);
                var Set_RecieptType = SetRecieptTypeAndDirectoryAndNotes(parameter.RecieptTypeId);
                var serialize = Convert.ToDouble(GeneralAPIsService.CreateSerializeOfInvoice(parameter.RecieptTypeId, 0, parameter.BranchId).ToString());
                var masterId = await AddMasterReciept(parameter,code,recdate, Set_RecieptType);
                var directory = Set_RecieptType.Item2;
                if (masterId == 0)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "Error in AddReciept",
                        ErrorMessageEn = "Error in AddReciept"
                    };

                parameter.BranchId = userInfo.CurrentbranchId;
                parameter.UserId = userInfo.employeeId;

                var res = FillReciepts(parameter, masterId, code, serialize, Set_RecieptType.Item1, Set_RecieptType.Item3, Set_RecieptType.Item4, recdate,ref financialDatas);

                if (res.Result != Result.Success)
                    return res;

                CReciepts = (List<GlReciepts>)res.Data;
                receiptCommand.AddRange(CReciepts);
                var saved = await receiptCommand.SaveAsync();

                if (saved)
                {
                    if (parameter.AttachedFile != null)
                        if (parameter.AttachedFile.Count() > 0)
                            saved = await filesOfInvoices.saveFilesOfInvoices(parameter.AttachedFile, userInfo.CurrentbranchId, directory, masterId, false, null, true);

                    int? financialIdOfSafeOfBank = await GetFinantialOfSafeOrBank(parameter.RecieptTypeId,parameter.SafeID,parameter.BankId);

                    

                    bool journalSaved = await SetRecieptsInJournalEntry(parameter, CReciepts, financialIdOfSafeOfBank, financialDatas,masterId);

                    if (!journalSaved)
                    {
                        receiptCommand.RemoveRange(CReciepts);
                        await receiptCommand.SaveAsync();
                        return new ResponseResult() { Result = Result.Failed, Note = "Fail to save JournalEntry " };
                    }

                    //var safeorbank = IsSafe(parameter.RecieptTypeId) ? CReciepts.First().SafeID : CReciepts.First().BankId;
                    //ReceiptsHistory.AddReceiptsHistory(
                    //     parameter.BranchId, null, TransActoin, null,
                    //   CReciepts.First().UserId, safeorbank, CReciepts.First().Code,
                    //     CReciepts.First().RecieptDate, masterId, CReciepts.First().RecieptType, CReciepts.First().RecieptTypeId
                    //     , CReciepts.First().Signal, CReciepts.First().IsBlock, CReciepts.First().IsAccredit, CReciepts.First().Serialize,
                    //     null, CReciepts.First().Amount
                    //     );


                    SystemActionEnum systemActionEnum = new SystemActionEnum();
                    if (parameter.RecieptTypeId == (int)DocumentType.CompinedSafeCash)
                        systemActionEnum = SystemActionEnum.addCompinedSafeCashReceipt;
                    else if (parameter.RecieptTypeId == (int)DocumentType.CompinedSafePayment)
                        systemActionEnum = SystemActionEnum.addCompinedSafePaymentReceipt;
                    else if (parameter.RecieptTypeId == (int)DocumentType.CompinedBankPayment)
                        systemActionEnum = SystemActionEnum.addCompinedBankPaymentReceipt;
                    else if (parameter.RecieptTypeId == (int)DocumentType.CompinedBankCash)
                        systemActionEnum = SystemActionEnum.addCompinedBankCashReceipt;
                    if (systemActionEnum != 0)
                        await _systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);

                }

                return new ResponseResult { Result = (saved ? Result.Success : Result.Failed) };
            }
            catch(Exception e)
            {
                throw;
            }
            
        }

        private async Task<int> GetFinantialOfSafeOrBank(int RecieptTypeId,int? SafeID,int? BankId)
        {
            var financialIdOfSafeOfBank = 0;
            if (RecieptTypeId == (int)DocumentType.CompinedSafeCash || RecieptTypeId == (int)DocumentType.CompinedSafePayment)
            {
                financialIdOfSafeOfBank = (int)await safeQuery.TableNoTracking.Where(a => a.Id == SafeID).Select(h => h.FinancialAccountId).FirstOrDefaultAsync();
            }
            else if (RecieptTypeId == (int)DocumentType.CompinedBankCash || RecieptTypeId == (int)DocumentType.CompinedBankPayment)
            {
                financialIdOfSafeOfBank = (int)await bankQuery.TableNoTracking.Where(a => a.Id == BankId).Select(h => h.FinancialAccountId).FirstOrDefaultAsync();
            }
            return financialIdOfSafeOfBank;
        }

        private  ResponseResult FillReciepts(CompinedRecieptsRequest parameter, int masterId, int code, double serialize, string recieptTyoe, string noteAr, string noteEn, DateTime recdate, ref List<financialData> financialDatas)
        {
            var reciepts = parameter.Reciepts;

            List<GlReciepts> CReciepts = new List<GlReciepts>();
            int index = 1;
            foreach (var reciept in reciepts)
            {
                if (reciept.Authority == 0)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "يجب ادخال اسم الجهه",
                        ErrorMessageEn = "The Authority must be entered"
                    };

                if (reciept.Amount <= 0)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "يجب ان يكون المبلغ اكبر من الصفر",
                        ErrorMessageEn = "The Amount must be greater than Zero"
                    };


                reciept.Notes = reciept.Notes ?? "";
                reciept.Amount = RoundNumbers.GetDefultRoundNumber(reciept.Amount);
                var MappedData = new GlReciepts();
                Mapping.Mapper.Map(reciept, MappedData);
                MappedData.SafeID = parameter.SafeID == 0 ? null : parameter.SafeID;
                MappedData.BankId = parameter.BankId == 0 ? null : parameter.BankId;
                MappedData.BranchId = parameter.BranchId;
                MappedData.UserId = parameter.UserId;
                MappedData.RecieptTypeId = parameter.RecieptTypeId;
                MappedData.RecieptDate = parameter.RecieptDate ?? MappedData.RecieptDate;

                financialData financialForBenfiteUser =  getFinantialAccIdForAuthorty(reciept.Authority, reciept.BenefitId, MappedData).Result;
                financialDatas.Add(financialForBenfiteUser);

                if (financialForBenfiteUser == null)
                {
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = " لا يوجد هذاالمستفيد   ",
                        ErrorMessageEn = "that Benfit is not have financial account "
                    };
                }
                if (financialForBenfiteUser.financialId == 0)
                {
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "المستفيد ليس له حساب مالى ",
                        ErrorMessageEn = "that Benfit is not have financial account "
                    };
                }

                var data = financialForBenfiteUser.ReceiptsData;
                data.Signal = GeneralAPIsService.GetSignal(parameter.RecieptTypeId);



                data.CompinedParentId = masterId;
                data.RecieptTypeId = parameter.RecieptTypeId;
                data.UserId = parameter.UserId;
                data.BranchId = parameter.BranchId;
                data.Code = code;
                data.IsAccredit = true;
                data.IsCompined = true;
                data.Serialize = serialize;
                data.RecieptType = parameter.BranchId + "-" + recieptTyoe + "-" + data.Code;
                data.PaperNumber = parameter.BranchId + "-" + recieptTyoe + "-" + data.Code;
                data.RectypeWithPayment = data.RecieptType + "-" + data.PaymentMethodId + "-" + index;
                data.Creditor = data.Signal > 0 ? data.Amount : 0;
                data.Debtor = data.Signal < 0 ? data.Amount : 0;
                data.RecieptDate = recdate;
                data.CreationDate = recdate;
                data.NoteAR = string.Concat(noteAr, " _ ", data.RecieptType);
                data.NoteEN = noteEn + " _ " + data.RecieptType;

                CReciepts.Add(data);
                index++;
            }
            return new ResponseResult() { Result = Result.Success,Data = CReciepts};
        }

        public async Task<int> AddMasterReciept(CompinedRecieptsRequest req, int code, DateTime recdate, Tuple<string, string, string, string> recieptTypeInfo)
        {
            GlReciepts master = new GlReciepts();
            master.Code = code;
            master.BranchId = req.BranchId;
            master.UserId = req.UserId;
            master.RecieptTypeId = req.RecieptTypeId;
            master.RecieptDate = recdate;
            master.SafeID = req.SafeID == 0 ? null : req.SafeID;
            master.BankId = req.BankId == 0 ? null : req.BankId;
            master.IsCompined = true;
            master.PaymentMethodId = 1;
            master.RecieptType = req.BranchId + "-" + recieptTypeInfo.Item1 + "-" + code;
            master.PaperNumber = req.BranchId + "-" + recieptTypeInfo.Item1 + "-" + code;
            master.NoteAR = string.Concat(recieptTypeInfo.Item3, " _ ", master.RecieptType);
            master.NoteEN = string.Concat(recieptTypeInfo.Item4, " _ ", master.RecieptType);
            master.Amount = RoundNumbers.GetDefultRoundNumber(req.Reciepts.Sum(a => a.Amount));
            master.Notes = req.Notes ?? "";

            var saved = await receiptCommand.AddAsync(master);
            return saved ? master.Id : 0;
        }

        public async Task<bool> SetRecieptsInJournalEntry(CompinedRecieptsRequest parameter, List<GlReciepts> reciepts, int? financialIdOfSafeOfBank, List<financialData> financialForBenfiteUser,int masterId)
        {
            AddJournalEntryRequest JEntry = new AddJournalEntryRequest();
            JEntry.ReceiptsId = masterId;
            JEntry.BranchId = parameter.BranchId;
            JEntry.FTDate = parameter.RecieptDate;
            JEntry.Notes = reciepts.First().NoteAR;
            JEntry.isAuto = true;
            JEntry.IsAccredit = true;
            JEntry.IsCompined = true;
            JEntry.CompinedReceiptCode = reciepts.First().Code;

            JEntry.JournalEntryDetails.Add(new JournalEntryDetail()
            {
                FinancialAccountId = financialIdOfSafeOfBank,
                Credit = reciepts.First().Signal < 0 ? reciepts.Sum(a => a.Amount) : 0,
                Debit = reciepts.First().Signal > 0 ? reciepts.Sum(a => a.Amount) : 0,
                DescriptionAr = reciepts.First().NoteAR,
                DescriptionEn = reciepts.First().NoteEN,
            });

            for (int i = 0; i < reciepts.Count; i++)
            {
                var data = reciepts[i];
                var financialData = await getFinantialAccIdForAuthorty(data.Authority, data.BenefitId, data); // financialForBenfiteUser[i];



                JEntry.JournalEntryDetails.Add(new JournalEntryDetail()
                {
                    FinancialAccountId = financialData.financialId,
                    FinancialCode = financialData.financialCode,
                    FinancialName = financialData.FinancialName,
                    Credit = data.Signal > 0 ? data.Amount : 0,
                    Debit = data.Signal < 0 ? data.Amount : 0,
                    DescriptionAr = data.NoteAR,
                    DescriptionEn = data.NoteEN,
                });
            }

            //var res = await journalEntryBusiness.AddJournalEntry(JEntry);
            var res = await _mediator.Send(JEntry);
            return res.Status == RepositoryActionStatus.Created;
        }
        public int Autocode(int recieptTypeId, int BranchId)
        {
            var Code = 0;
            Code = receiptQuery.GetMaxCode(e => e.Code, a => a.IsCompined == true && a.BranchId == BranchId && a.RecieptTypeId == recieptTypeId);
            Code++;

            return Code;
        }
        public async Task<financialData> getFinantialAccIdForAuthorty(int type, int ID, GlReciepts RData)
        {
            financialData FA = new financialData();
            if (type == AuthorityTypes.customers || type == AuthorityTypes.suppliers)
            {
                FA = await PersonQuery.TableNoTracking.Where(a => a.Id == ID && (type == AuthorityTypes.customers ? a.IsCustomer == true : a.IsSupplier == true))
                            .Include(h => h.FinancialAccount)
                            .Select(s => new financialData()
                            {
                                financialId = s.FinancialAccountId.GetValueOrDefault(0),
                                financialCode = s.FinancialAccount.AccountCode,
                                FinancialName = s.FinancialAccount.ArabicName
                            }).FirstOrDefaultAsync();

                RData.PersonId = ID;
                RData.OtherAuthorityId = null;
                RData.SalesManId = null;
                RData.FinancialAccountId = null;

            }
            else if (type == AuthorityTypes.other)
            {
                FA = await OtherAuthorityQuery.TableNoTracking.Where(a => a.Id == ID).Include(h => h.FinancialAccount)
                                    .Select(s => new financialData()
                                    {
                                        financialId = s.FinancialAccountId.GetValueOrDefault(0),
                                        financialCode = s.FinancialAccount.AccountCode,
                                        FinancialName = s.FinancialAccount.ArabicName
                                    }).FirstOrDefaultAsync();
                RData.PersonId = null;
                RData.OtherAuthorityId = ID;
                RData.SalesManId = null;
                RData.FinancialAccountId = null;

            }
            else if (type == AuthorityTypes.salesman)
            {
                FA = await SalesManQuery.TableNoTracking.Where(a => a.Id == ID).Include(h => h.FinancialAccount)
                                    .Select(s => new financialData()
                                    {
                                        financialId = s.FinancialAccountId.GetValueOrDefault(0),
                                        financialCode = s.FinancialAccount.AccountCode,
                                        FinancialName = s.FinancialAccount.ArabicName
                                    }).FirstOrDefaultAsync();
                RData.SalesManId = ID;
                RData.PersonId = null;
                RData.OtherAuthorityId = null;

                RData.FinancialAccountId = null;
            }
            else if (type == AuthorityTypes.DirectAccounts)
            {
                FA = await FinancialAccountQuery.TableNoTracking.Where(h => h.Id == ID).Select(s => new financialData()
                {
                    financialId = s.Id,
                    financialCode = s.AccountCode,
                    FinancialName = s.ArabicName
                }).FirstOrDefaultAsync();
                RData.FinancialAccountId = ID;
                RData.PersonId = null;
                RData.OtherAuthorityId = null;
                RData.SalesManId = null;

            }
            if (FA != null)
                FA.ReceiptsData = RData;
            return FA;
        }

        public Tuple<string, string, string, string> SetRecieptTypeAndDirectoryAndNotes(int recieptTypeId)
        {
            var recieptType = "";
            var Directory = "";
            var NoteAr = "";
            var NoteEn = "";
            if (recieptTypeId == (int)DocumentType.CompinedSafeCash)
            {
                recieptType = InvoicesCode.CompinedSafeCash;
                Directory = FilesDirectories.SafeCash;
                NoteAr = NotesOfReciepts.CompinedSafeCashAr;
                NoteEn = NotesOfReciepts.CompinedSafeCashEn;
            }
            else if (recieptTypeId == (int)DocumentType.CompinedSafePayment)
            {
                recieptType = InvoicesCode.CompinedSafePayment;
                Directory = FilesDirectories.SafePayment;
                NoteAr = NotesOfReciepts.CompinedSafePaymentAr;
                NoteEn = NotesOfReciepts.CompinedSafePaymentEn;
            }
            else if (recieptTypeId == (int)DocumentType.CompinedBankCash)
            {
                recieptType = InvoicesCode.CompinedBankCash;
                Directory = FilesDirectories.BankCash;
                NoteAr = NotesOfReciepts.CompinedBankCashAr;
                NoteEn = NotesOfReciepts.CompinedBankCashEn;
            }
            else if (recieptTypeId == (int)DocumentType.CompinedBankPayment)
            {
                recieptType = InvoicesCode.CompinedBankPayment;
                Directory = FilesDirectories.BankPayment;
                NoteAr = NotesOfReciepts.CompinedBankPaymentAr;
                NoteEn = NotesOfReciepts.CompinedBankPaymentEn;
            }
            
            return new Tuple<string, string, string, string>(recieptType, Directory, NoteAr, NoteEn);
        }

        public async Task<ResponseResult> GetAllCompinedReciepts(GetRecieptsData parameter)
        {
            UserInformationModel userInfo = await _Userinformation.GetUserInformation();

            if(userInfo == null)
                return new ResponseResult(){Result = Result.UnAuthorized,ErrorMessageAr = "ليس لديك صلاحيه",ErrorMessageEn = "Unautorized" };
            if (parameter.PageSize <= 0 || parameter.PageNumber <= 0)
                return new ResponseResult() { Result = Result.RequiredData, ErrorMessageAr = "خطا ف رقم الصفحه", ErrorMessageEn = "Invalid Page Number or Page Size" };
            if (parameter.ReceiptsType == 0)
                return new ResponseResult() { Result = Result.RequiredData, ErrorMessageAr = "يجب ادخال نوع السند", ErrorMessageEn = "ReceiptsType must be entered" };

            var totalCount = receiptQuery.TableNoTracking.Where(a => a.BranchId == userInfo.CurrentbranchId 
                                                                && a.RecieptTypeId == parameter.ReceiptsType).GroupBy(a => a.Code).Count();

            if(totalCount == 0)
                return new ResponseResult() { Result = Result.NotFound, ErrorMessageAr = "لا يوجد سندات", ErrorMessageEn = "No Reciepts  Found" };

            var data = receiptQuery.TableNoTracking.
                Where(a => a.BranchId == userInfo.CurrentbranchId && a.RecieptTypeId == parameter.ReceiptsType &&
                       (parameter.SafeOrBankId > 0  ? IsSafe(parameter.ReceiptsType) ? a.SafeID == parameter.SafeOrBankId : a.BankId == parameter.SafeOrBankId 
                       : true) &&
                        (!string.IsNullOrEmpty(parameter.Code) ? (a.Code.ToString() == parameter.Code || a.RecieptType == parameter.Code) : true) &&
                        (parameter.DateFrom != null ? a.RecieptDate.Date >= parameter.DateFrom.Value.Date : true) &&
                        (parameter.DateTo != null ? a.RecieptDate.Date <= parameter.DateTo.Value.Date : true ) &&
                        (a.CompinedParentId == null || a.CompinedParentId == 0) && a.IsCompined == true  )
                        .Skip((parameter.PageNumber - 1) * parameter.PageSize).Take(parameter.PageSize).ToList();



            //var groupedData = data.GroupBy(a => a.Code).Skip((parameter.PageNumber - 1) * parameter.PageSize).Take(parameter.PageSize);
            List<CompinedRecieptResponse> response = new List<CompinedRecieptResponse>();
            foreach(var item in data)
            {
                var safeInfo = IsSafe(parameter.ReceiptsType) ?  safeQuery.GetByIdAsync(item.SafeID).Result : null ;
                var BankInfo = IsSafe(parameter.ReceiptsType) ?  null : bankQuery.GetByIdAsync(item.BankId).Result;
                response.Add(new CompinedRecieptResponse()
                {
                    RecieptId = item.Id,
                    Code = item.Code,
                    Amount = RoundNumbers.GetRoundNumber(item.Amount),
                    date = item.RecieptDate,
                    safeOrBankNameAr = IsSafe(parameter.ReceiptsType) ? safeInfo.ArabicName : BankInfo.ArabicName,
                    safeOrBankNameEn = IsSafe(parameter.ReceiptsType) ? safeInfo.LatinName : BankInfo.LatinName,
                    ReceiptsType = item.RecieptType,
                });
            }

            return new ResponseResult() { Result = Result.Success ,Data = response,TotalCount = totalCount,DataCount = response.Count };

        }

        bool IsSafe(int type)
        {
            return (type == (int)DocumentType.CompinedSafeCash || type == (int)DocumentType.CompinedSafePayment);
        }

        public async Task<ResponseResult> GetCompinedRecieptById(int Id)
        {
            UserInformationModel userInfo = await _Userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult() { Result = Result.UnAuthorized, ErrorMessageAr = "ليس لديك صلاحيه", ErrorMessageEn = "Unautorized" };

            if (Id == 0)
                return new ResponseResult() { Data = null, Result = Result.RequiredData,ErrorMessageAr = "recieptType and code is required", ErrorMessageEn = "recieptType and code is required" };

            var data = receiptQuery.TableNoTracking.Include(h => h.RecieptsFiles)
                                                   .FirstOrDefault(a => a.BranchId == userInfo.CurrentbranchId &&
                                                          a.Id == Id &&
                                                          a.CompinedParentId == null &&
                                                          a.IsCompined == true);

            if(data == null )
                    return new ResponseResult() { Result = Result.NoDataFound, ErrorMessageAr = "لا يوجد بيانات", ErrorMessageEn = "No Datta Found" };

            var reciepts = receiptQuery.TableNoTracking.Where(a => a.CompinedParentId == data.Id && a.IsBlock == false).ToList();

            CompinedRecieptDetailsResponse response = new CompinedRecieptDetailsResponse();

            var bankInfo = await bankQuery.GetByIdAsync(data.BankId);
            var safeInfo = await safeQuery.GetByIdAsync(data.SafeID);
            bool isSafe = IsSafe(data.RecieptTypeId);

            response.RecieptDate = data.RecieptDate;
            response.UserId = data.UserId;
            response.BranchId = data.BranchId;
            response.safeOrBankID = isSafe ? safeInfo.Id : bankInfo.Id;
            response.safeOrBankNameArabic = isSafe ? safeInfo.ArabicName : bankInfo.ArabicName;
            response.safeOrBankNameLatin = isSafe ? safeInfo.LatinName : bankInfo.LatinName;
            response.PaperNumber = data.PaperNumber;
            response.Code = data.Code;
            response.RecieptTypeId = data.RecieptTypeId;
            response.RecieptType = data.RecieptType;
            response.RecieptsFiles = data.RecieptsFiles;
            response.Amount = data.Amount;

            foreach (var item in reciepts)
            {
                var paymentInfo = await PaymentMethodsQuery.GetByIdAsync(item.PaymentMethodId);
                var authorityInfo = await GetAuthorityInfo(item, item.Authority);
                response.Reciepts.Add(new CompinedRecieptItem
                {
                    RecieptId = item.Id,
                    Amount = RoundNumbers.GetRoundNumber(item.Amount),
                    Authority = item.Authority,
                    authorityNameArabic = Lists.receiptsAuthorities.Where(h => h.Id == item.Authority).Select(h => h.arabicName).FirstOrDefault(""),
                    authorityNameLatin = Lists.receiptsAuthorities.Where(h => h.Id == item.Authority).Select(h => h.latinName).FirstOrDefault(""),
                    PaymentMethodId = item.PaymentMethodId,
                    PaymentMethodNameArabic = paymentInfo.ArabicName,
                    PaymentMethodNameLatin = paymentInfo.LatinName,
                    BenefitId = authorityInfo.Item1,
                    benefitNameArabic = authorityInfo.Item2,
                    benefitNameLatin = authorityInfo.Item3,
                    ChequeNumber = item.ChequeNumber,
                    ChequeDate = item.ChequeDate,
                    ChequeBankName = item.ChequeBankName,
                    Notes = item.Notes,
                    IsIncludeVat = item.IsIncludeVat
                });
            }

            return new ResponseResult() { Result = Result.Success,Data = response};

        }

        async Task<Tuple<int, string, string>> GetAuthorityInfo(GlReciepts item,int authorityId)
        {
            int binifitId = 0;
            string nameAr = "";
            string nameEn = "";
            var personInfo = await PersonQuery.GetByIdAsync(item.PersonId);
            var salesManInfo = await SalesManQuery.GetByIdAsync(item.SalesManId);
            var otherAuthInfo = await OtherAuthorityQuery.GetByIdAsync(item.OtherAuthorityId);
            var financialInfo = await FinancialAccountQuery.GetByIdAsync(item.FinancialAccountId);
            if (authorityId == AuthorityTypes.salesman)
            {
                binifitId = salesManInfo.Id;
                nameAr = salesManInfo.ArabicName;
                nameEn = salesManInfo.LatinName;
            }
            //لو الجهه حسابات عامه
            else if (authorityId == AuthorityTypes.DirectAccounts)
            {
                binifitId = financialInfo.Id;
                nameAr = financialInfo.ArabicName;
                nameEn = financialInfo.LatinName;
            }
            // لو الجهه عميل او مورد 
            else if (authorityId == AuthorityTypes.customers || authorityId == AuthorityTypes.suppliers)
            {
                binifitId = personInfo.Id;
                nameAr = personInfo.ArabicName;
                nameEn = personInfo.LatinName;
            }
            //لو الجهه جهات اخرى
            else if (authorityId == AuthorityTypes.other)
            {
                binifitId = otherAuthInfo.Id;
                nameAr = otherAuthInfo.ArabicName;
                nameEn = otherAuthInfo.LatinName;
            }
            return new Tuple<int, string, string>(binifitId,nameAr,nameEn);
        }

        public async Task<ResponseResult> UpdateCompinedReceipt(UpdateCompinedRecieptsRequest parameter)
        {
            try
            {
                if (parameter.RecieptTypeId == 0)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "يجب ادخال نوع السند",
                        ErrorMessageEn = "The RecieptTypeId must be entered",
                    };

                if (IsSafe(parameter.RecieptTypeId) &&  parameter.SafeID == 0)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "يجب ادخال خزينه",
                        ErrorMessageEn = "The safeId must be entered"
                    };
                if (!IsSafe(parameter.RecieptTypeId) && parameter.BankId == 0)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "يجب ادخال بنك",
                        ErrorMessageEn = "The BankId must be entered"
                    };
                if (parameter.ParentRecieptId == 0)
                    return new ResponseResult()
                    {
                        Result = Result.RequiredData,
                        ErrorMessageAr = "The ParentRecieptId is required",
                        ErrorMessageEn = "The ParentRecieptId is required"
                    };

                if (parameter.Reciepts == null || parameter.Reciepts.Count == 0)
                    return new ResponseResult()
                    {
                        Result = Result.RequiredData,
                        ErrorMessageAr = "No Resiepts Added",
                        ErrorMessageEn = "No Resiepts Added"
                    };

                UserInformationModel userInfo = await _Userinformation.GetUserInformation();

                var ParentReciept = receiptQuery.TableNoTracking.FirstOrDefault(x => x.BranchId == userInfo.CurrentbranchId && x.Id == parameter.ParentRecieptId 
                                                                                && x.RecieptTypeId == parameter.RecieptTypeId
                                                                                && x.IsBlock == false && x.CompinedParentId == null); 

                if (ParentReciept == null)
                    return new ResponseResult()
                    {
                        Result = Result.NotFound,
                        ErrorMessageAr = "No Data Found for this ParentRecieptId",
                        ErrorMessageEn = "No Data Found for this ParentRecieptId"
                    };


                List<financialData> financialDatas = new List<financialData>();
                List<GlReciepts> CReciepts = new List<GlReciepts>();
                var recieptInfo= SetRecieptTypeAndDirectoryAndNotes(ParentReciept.RecieptTypeId);
                var res = FillReciepts(parameter, ParentReciept.Id, ParentReciept.Code, ParentReciept.Serialize, recieptInfo.Item1, recieptInfo.Item3, recieptInfo.Item4, DateTime.Now, ref financialDatas);
                if (res.Result != Result.Success)
                    return res;

                CReciepts = (List<GlReciepts>)res.Data;

                receiptCommand.StartTransaction();

                await receiptCommand.DeleteAsync(q => q.CompinedParentId == parameter.ParentRecieptId);


                receiptCommand.AddRange(CReciepts);
                await receiptCommand.SaveAsync();


                // modify Parent
                ParentReciept.Amount = parameter.Reciepts.Sum(a => a.Amount);
                ParentReciept.Creditor = ParentReciept.Signal > 0 ? ParentReciept.Amount : 0;
                ParentReciept.Debtor = ParentReciept.Signal < 0 ? ParentReciept.Amount : 0;
                ParentReciept.SafeID = parameter.SafeID == 0 ? null : parameter.SafeID;
                ParentReciept.BankId = parameter.SafeID == 0 ? null : parameter.BankId;
                ParentReciept.RecieptDate = parameter.RecieptDate ?? ParentReciept.RecieptDate;
                ParentReciept.PaperNumber = parameter.PaperNumber;
                ParentReciept.Notes = parameter.Notes;
                await receiptCommand.UpdateAsyn(ParentReciept);

                receiptCommand.CommitTransaction();
                
                await filesOfInvoices.saveFilesOfInvoices(parameter.AttachedFile, parameter.BranchId, recieptInfo.Item2, ParentReciept.Id, true, parameter.FileId, true);

                

                int? financialIdOfSafeOfBank = await GetFinantialOfSafeOrBank(ParentReciept.RecieptTypeId,parameter.SafeID, parameter.BankId);
                var saved  = UpdateRecieptsInJournalEntry(CReciepts, financialIdOfSafeOfBank, null, ParentReciept.Id).Result;

                if(!saved)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "Fail in save JournalEntry",
                        ErrorMessageEn = "Fail in save JournalEntry"
                    };

                return new ResponseResult() { Result = Result.Success};
            }
            catch(Exception ex)
            {
                receiptCommand.Rollback();
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "Fail in save receipts",
                    ErrorMessageEn = "Fail in save receipts"
                };
            }

        }

        public async Task<bool> UpdateRecieptsInJournalEntry( List<GlReciepts> reciepts, int? financialIdOfSafeOfBank, List<financialData> financialForBenfiteUser,int masterId)
        {
            try
            {
                var journalEntry = _JournalenteryQuery.TableNoTracking.FirstOrDefault(h => h.ReceiptsId == masterId);

                UpdateJournalEntryRequest JEntry = new UpdateJournalEntryRequest();
                JEntry.BranchId = reciepts.First().BranchId;
                JEntry.FTDate = reciepts.First().RecieptDate;
                JEntry.Id = journalEntry.Id;
                JEntry.Notes = reciepts.First().NoteAR;
                JEntry.fromSystem = true;


                JEntry.journalEntryDetails.Add(new JournalEntryDetail()
                {
                    FinancialAccountId = financialIdOfSafeOfBank,
                    Credit = reciepts.First().Signal < 0 ? reciepts.Sum(a => a.Amount) : 0,
                    Debit = reciepts.First().Signal > 0 ? reciepts.Sum(a => a.Amount) : 0,
                    DescriptionAr = reciepts.First().NoteAR,
                    DescriptionEn = reciepts.First().NoteEN,
                });
                foreach (var data in reciepts)
                {
                    //var data = reciepts[i];
                    var financialData =  getFinantialAccIdForAuthorty(data.Authority, data.BenefitId, data).Result;

                    JEntry.journalEntryDetails.Add(new JournalEntryDetail()
                    {
                        FinancialAccountId = financialData.financialId,
                        FinancialCode = financialData.financialCode,
                        FinancialName = financialData.FinancialName,
                        Credit = data.Signal > 0 ? data.Amount : 0,
                        Debit = data.Signal < 0 ? data.Amount : 0,
                        DescriptionAr = data.NoteAR,
                        DescriptionEn = data.NoteEN,
                    });
                }

                //var res =  journalEntryBusiness.UpdateJournalEntry(JEntry).Result;
                var res = await _mediator.Send(JEntry);
                return res.Status == RepositoryActionStatus.Updated;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<ResponseResult> DeleteCompinedReciept(List<int> Id)
        {
            if(Id == null || Id.Count == 0)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "No Id Entered",
                    ErrorMessageEn = "No Id Entered"
                };
            UserInformationModel userInfo = await _Userinformation.GetUserInformation();

            var reciepts = receiptQuery.TableNoTracking.Where(a => Id.Contains(a.Id) && a.IsBlock == false && a.CompinedParentId == null
                                                                && a.IsCompined == true && a.BranchId == userInfo.CurrentbranchId).ToList();

            if (reciepts.Where(x => x.RecieptTypeId == (int)DocumentType.CompinedSafeCash).Any())
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.CashReceiptForSafe, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            if (reciepts.Where(x => x.RecieptTypeId == (int)DocumentType.CompinedSafePayment).Any())
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.PayReceiptForSafe, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            if (reciepts.Where(x => x.RecieptTypeId == (int)DocumentType.CompinedBankCash).Any())
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.banks, (int)SubFormsIds.CashReceiptForBank, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            if (reciepts.Where(x => x.RecieptTypeId == (int)DocumentType.CompinedBankPayment).Any())
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.banks, (int)SubFormsIds.PayReceiptForBank, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }

            if (reciepts.Count == 0)
                return new ResponseResult()
                {
                    Result = Result.NotFound,
                    ErrorMessageAr = "No Data Found",
                    ErrorMessageEn = "No Data Found"
                };

            var RecieptDel = reciepts.Select(a => { a.IsBlock = true; return a; }).ToList();

            var saved =  receiptCommand.UpdateAsyn(RecieptDel).Result;
            

            if (!saved)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "Fail while save",
                    ErrorMessageEn = "Fail while save"
                };
            var recIds = RecieptDel.Select(a => a.Id).ToList();
            var JEIds = _JournalenteryQuery.TableNoTracking.Where(a => recIds.Contains((int)a.ReceiptsId)).Select(a => a.Id).ToList();
            //await journalEntryBusiness.BlockJournalEntry(new BlockJournalEntry() { Ids = JEIds.ToArray() });
            await _mediator.Send(new BlockJournalEntryReqeust { Ids = JEIds.ToArray() });
        
            foreach (var item in RecieptDel)
            {
                ReceiptsHistory.AddReceiptsHistory(
                                     item.BranchId, item.BenefitId, HistoryActions.Delete, item.PaymentMethodId,
                                   item.UserId, item.BankId != null ? item.BankId.Value : item.SafeID.Value, item.Code,
                                     item.RecieptDate, item.Id, item.RecieptType, item.RecieptTypeId
                                     , item.Signal, item.IsBlock, item.IsAccredit, item.Serialize,
                                     item.Authority, item.Amount,0,userInfo);

            }
            var receiptsType = RecieptDel.GroupBy(x => x.RecieptTypeId).Select(y => y.FirstOrDefault());
            foreach (var item in receiptsType)
            {
                SystemActionEnum systemActionEnum = new SystemActionEnum();
                if (item.RecieptTypeId == (int)DocumentType.CompinedSafeCash)
                    systemActionEnum = SystemActionEnum.editCompinedSafeCashReceipt;
                else if (item.RecieptTypeId == (int)DocumentType.CompinedSafePayment)
                    systemActionEnum = SystemActionEnum.editCompinedSafePaymentReceipt;
                else if (item.RecieptTypeId == (int)DocumentType.CompinedBankPayment)
                    systemActionEnum = SystemActionEnum.editCompinedBankPaymentReceipt;
                else if (item.RecieptTypeId == (int)DocumentType.CompinedBankCash)
                    systemActionEnum = SystemActionEnum.editCompinedBankCashReceipt;
                await _systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);
            }
            return new ResponseResult() {Result = Result.Success };

        }

    }
}
