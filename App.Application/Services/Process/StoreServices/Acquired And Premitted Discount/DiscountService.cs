using App.Application.Handlers.GeneralLedger.JournalEntry;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Printing;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Application.Services.Process.Invoices.RecieptsWithInvoices;
using App.Application.Services.Process.Persons;
using App.Domain.Enums;
using App.Domain.Models.Response.Store;
using App.Domain.Models.Response.Store.Reports.Store;
using App.Infrastructure.settings;
using App.Infrastructure.UserManagementDB;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace App.Application.Services.Acquired_And_Premitted_Discount
{
    public class DiscountService : BaseClass, IDiscountService
    {
        private readonly IRepositoryQuery<InvDiscount_A_P> Discount_A_PQuery;
        private readonly IRepositoryQuery<InvPersons> _personsQuery;
        private readonly IRepositoryQuery<GLBranch> _gLBranchQuery;
        private readonly IRepositoryQuery<GlReciepts> _recieptRepositoryQuery;
        private readonly IRepositoryCommand<GlReciepts> recieptRepositoryCommand;
        private readonly IRepositoryCommand<InvDiscount_A_P> Discount_A_PCommand;
        private readonly IRepositoryCommand<GLJournalEntry> GLJournalEntryCommand;
        private readonly IRepositoryQuery<GLJournalEntryDetails> _GLJournalEntryDetailsQuery;
        private readonly IRepositoryQuery<GLJournalEntry> _gLJournalEntryQuery;
        private readonly IGeneralAPIsService generalAPIsService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<InvDiscount_A_P_History> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly iUserInformation _iUserInformation;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IReceiptsService _receiptsService;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _PurchasesAndSalesSettingsquery;
        private readonly IMediator _mediator;
        private readonly IGeneralPrint _iGeneralPrint;


        public DiscountService(IRepositoryQuery<InvDiscount_A_P> _Discount_A_PQuery,
                                 IRepositoryQuery<InvPersons> personsQuery,
                                 IRepositoryQuery<GLBranch> GLBranchQuery,
                                 IRepositoryQuery<GLJournalEntryDetails> GLJournalEntryDetailsQuery,
                                 IRepositoryQuery<GLJournalEntry> GLJournalEntryQuery,
                                 IGeneralAPIsService GeneralAPIsService,
                                 IRepositoryQuery<GlReciepts> _RecieptRepositoryQuery,
                                 IRepositoryCommand<GlReciepts> _RecieptRepositoryCommand,
                                 IRepositoryCommand<InvDiscount_A_P> _Discount_A_PCommand,
                                 IHistory<InvDiscount_A_P_History> history,
                                 ISystemHistoryLogsService systemHistoryLogsService,
                                 iUserInformation iUserInformation,
                                 iAuthorizationService iAuthorizationService,
                                 IReceiptsService receiptsService,
                                 IRepositoryCommand<GLJournalEntry> gLJournalEntryCommand,
                                 IRepositoryQuery<GLJournalEntry> gLJournalEntryQuery,
                                 IRepositoryQuery<GLPurchasesAndSalesSettings> PurchasesAndSalesSettingsQuery,
                                 IHttpContextAccessor _httpContext,
                                 IGeneralPrint iGeneralPrint,
                                 IMediator mediator) : base(_httpContext)
        {
            Discount_A_PQuery = _Discount_A_PQuery;
            _personsQuery = personsQuery;
            _gLBranchQuery = GLBranchQuery;
            _recieptRepositoryQuery = _RecieptRepositoryQuery;
            recieptRepositoryCommand = _RecieptRepositoryCommand;
            Discount_A_PCommand = _Discount_A_PCommand;
            this.history = history;
            _systemHistoryLogsService = systemHistoryLogsService;
            _iUserInformation = iUserInformation;
            _iAuthorizationService = iAuthorizationService;
            _receiptsService = receiptsService;
            _PurchasesAndSalesSettingsquery = PurchasesAndSalesSettingsQuery;
            httpContext = _httpContext;
            GLJournalEntryCommand = gLJournalEntryCommand;
            GLJournalEntryCommand = gLJournalEntryCommand;
            _GLJournalEntryDetailsQuery = GLJournalEntryDetailsQuery;
            _gLJournalEntryQuery = GLJournalEntryQuery;
            generalAPIsService = GeneralAPIsService;
            _iGeneralPrint = iGeneralPrint;
            _mediator = mediator;
        }


        public async Task<InvPersons> getPerson(int personId) => _personsQuery.Find(x => x.Id == personId);
        //public async Task<string> getCode() => _recieptRepositoryQuery.TableNoTracking.OrderBy(x => Convert.ToInt64(x.Code)).LastOrDefault().Code;
        public async Task<ResponseResult> AddDiscount(DiscountRequest parameter)
        {
            if (parameter.amountMoney <= 0)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    Note = Actions.AcmountCantNotBeZeroOrLess
                };
            var findPerson = await _personsQuery.GetByIdAsync(parameter.Person);
            if (findPerson == null)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    Note = Actions.PersonDoseNotExist
                };
            if (findPerson.IsCustomer != parameter.IsCustomer)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    Note = Actions.PersonDoseNotExist
                };
            var userInformation = await _iUserInformation.GetUserInformation();
            int NextCode = 1;
            int currentCode = Discount_A_PQuery.TableNoTracking.Where(e => e.DocType == parameter.DocType && e.BranchId == userInformation.CurrentbranchId).Count() > 0 ?
                                                                Discount_A_PQuery.TableNoTracking
                                                                .Where(e => e.DocType == parameter.DocType && e.BranchId == userInformation.CurrentbranchId)
                                                                .OrderBy(x => x.Id)
                                                                .LastOrDefault().Code : 0;
            if (Discount_A_PQuery.Count() != 0)
                NextCode = currentCode + 1;

            var data = new InvDiscount_A_P();
            data.Code = NextCode;
            parameter.DocDate = generalAPIsService.serverDate(parameter.DocDate);
            if (parameter.DocType != (int)DocumentType.AcquiredDiscount && parameter.DocType != (int)DocumentType.PermittedDiscount)
                return new ResponseResult() { Result = Result.Failed, Note = "Use '14' for AcquiredDiscount and '15' for PermittedDiscount" };
            if (parameter.DocType == (int)DocumentType.AcquiredDiscount)
            {
                data.DocNumber = Aliases.InvoicesCode.AcquiredDiscount + "-" + NextCode.ToString();
                data.Amount = -1 * parameter.amountMoney;
                data.Creditor = 0;
                data.Debtor = parameter.amountMoney;
            }
            else if (parameter.DocType == (int)DocumentType.PermittedDiscount)
            {
                data.DocNumber = Aliases.InvoicesCode.PermittedDiscount + "-" + NextCode.ToString();
                data.Amount = parameter.amountMoney;
                data.Creditor = parameter.amountMoney;
                data.Debtor = 0;
            }
            data.Notes = parameter.Notes;
            data.PaperNumber = parameter.PaperNumber;
            data.DocDate = parameter.DocDate;
            data.IsCustomer = parameter.IsCustomer;
            data.PersonId = parameter.Person;
            data.DocType = parameter.DocType;
            data.IsDeleted = false;
            data.Refrience = data.DocNumber;
            data.BranchId = userInformation.CurrentbranchId;
            var person = await getPerson(data.PersonId);

            Discount_A_PCommand.Add(data);
            //AddToReciepts 
            //await addEntry(parameter, userInformation,data.Code,data.Id);


            await AddJEntery(parameter, userInformation, data.Code, data.Id);
            var rec = await _receiptsService.AddReceipt(new Domain.RecieptsRequest()
            {
                Amount = parameter.amountMoney,
                Creditor = parameter.DocType == (int)DocumentType.AcquiredDiscount ? 0 : parameter.amountMoney,
                Debtor = parameter.DocType == (int)DocumentType.PermittedDiscount ? 0 : parameter.amountMoney,
                PaperNumber = data.PaperNumber,
                Notes = data.Notes,
                RecieptTypeId = parameter.DocType == (int)DocumentType.AcquiredDiscount ? (int)DocumentType.AcquiredDiscount : (int)DocumentType.PermittedDiscount,
                BranchId = userInformation.CurrentbranchId, // set current branch of employee
                UserId = userInformation.userId,
                IsIncludeVat = false,
                Authority = parameter.IsCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers,
                Code = data.Code,
                IsAccredit = false,
                ParentId = data.Id,
                ParentTypeId = parameter.DocType,//== (int)DocumentType.AcquiredDiscount ?  (int)RecieptsParentType.AcquiredDiscount : (int)RecieptsParentType.PermittedDiscount,
                ParentType = data.BranchId.ToString() + "-" + data.DocNumber,
                BenefitId = parameter.Person,
                PaymentMethodId = 1,
                ReceiptOnly = true,
                Deferre = true,
                RecieptDate = parameter.DocDate,
                FA_Id = parameter.DocType == (int)DocumentType.AcquiredDiscount ? (int)_PurchasesAndSalesSettingsquery.TableNoTracking.Where(c => c.branchId == userInformation.CurrentbranchId).Where(x => x.RecieptsType == (int)DocumentType.AcquiredDiscount).FirstOrDefault().FinancialAccountId : (int)_PurchasesAndSalesSettingsquery.TableNoTracking.Where(c => c.branchId == userInformation.CurrentbranchId).Where(x => x.RecieptsType == (int)DocumentType.PermittedDiscount).FirstOrDefault().FinancialAccountId


            });


            var TotalAmount = await _receiptsService.GetReceiptBalanceForBenifit(person.IsCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers, person.Id);

            history.AddHistory(data.Id, "", "", Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            SystemActionEnum systemActionEnum = new SystemActionEnum();
            if (parameter.DocType == (int)DocumentType.AcquiredDiscount)
                systemActionEnum = SystemActionEnum.addAcquiredDiscount;
            else if (parameter.DocType == (int)DocumentType.PermittedDiscount)
                systemActionEnum = SystemActionEnum.addPermittedDiscount;
            await _systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);
            return new ResponseResult() { Data = TotalAmount, Id = data.Id, Result = Result.Success };

        }

        //البيان
        public async Task<financialData> getFinantialAcc(int ID)
        {
            financialData FA = new financialData();

            return FA = await _personsQuery.TableNoTracking.Where(a => a.Id == ID)
                         .Include(h => h.FinancialAccount)
                         .Select(s => new financialData()
                         {
                             financialId = s.FinancialAccountId.GetValueOrDefault(0),
                             financialCode = s.FinancialAccount.AccountCode,
                             FinancialName = s.FinancialAccount.ArabicName
                         }).FirstOrDefaultAsync();


        }
        public async Task DeleteJEntry(int[] docType, int[] DocId)
        {
            var jouranlEntryExist = _gLJournalEntryQuery.TableNoTracking.Where(x => docType.Contains(x.DocType.Value) && DocId.Contains(x.InvoiceId.Value)).ToList();
            if (!jouranlEntryExist.Any())
                return;
            jouranlEntryExist.ForEach(x => x.IsBlock = true);
            await _systemHistoryLogsService.SystemHistoryLogsService(SystemActionEnum.deleteJournalEntry, jouranlEntryExist.Count());

            var deleted = await GLJournalEntryCommand.UpdateAsyn(jouranlEntryExist);

        }
        //update journalEntery
        public async Task UpdateJEntery(UpdateDiscountRequest parameter, UserInformationModel userInformation, int DocCode, int DocId)
        {
            var jouranlEntryExist = _GLJournalEntryDetailsQuery.TableNoTracking.Include(x => x.journalEntry).Where(x => parameter.DocType == x.journalEntry.DocType.Value && x.journalEntry.InvoiceId == DocId);
            int FA = jouranlEntryExist.FirstOrDefault().JournalEntryId;
            if (!jouranlEntryExist.Any())
                return;

            DiscountRequest parm = new DiscountRequest()
            {
                amountMoney = parameter.amountMoney,
                DocDate = parameter.DocDate,
                DocType = parameter.DocType,
                IsCustomer = parameter.IsCustomer,
                Person = parameter.Person
            };
            var details = await EntryDetails(parm, userInformation, DocCode, DocId);
            UpdateJournalEntryRequest jentery = new UpdateJournalEntryRequest()
            {
                Id = FA,
                fromSystem = true,
                IsAccredit = true,
                BranchId = userInformation.CurrentbranchId,
                FTDate = parameter.DocDate,
                Notes = details.notes,
                journalEntryDetails = details.journalEntryDetail
            };
            //await JEntery.UpdateJournalEntry(jentery);
            await _mediator.Send(jentery);
        }
        private async Task AddJEntery(DiscountRequest parameter, UserInformationModel userInformation, int DocCode, int DocId)
        {
            var details = await EntryDetails(parameter, userInformation, DocCode, DocId);
            AddJournalEntryRequest jentery = new AddJournalEntryRequest()
            {
                isAuto = true,
                IsAccredit = true,
                BranchId = userInformation.CurrentbranchId,
                FTDate = parameter.DocDate,
                Notes = details.notes,
                DocType = parameter.DocType,
                JournalEntryDetails = details.journalEntryDetail,
                InvoiceId = DocId
            };

            //await JEntery.AddJournalEntry(jentery);
            await _mediator.Send(jentery);
        }
        // القيود 
        private async Task<journalDetailsDTO> EntryDetails(DiscountRequest parameter, UserInformationModel userInformation, int DocCode, int DocId)
        {
            // get journalEntery


            //المكتسب 
            string noteAr = NotesOfReciepts.AcquiredDiscountAr + InvoicesCode.AcquiredDiscount + " - " + DocCode;
            string noteEn = NotesOfReciepts.AcquiredDiscountEN + InvoicesCode.AcquiredDiscount + " - " + DocCode;

            if (parameter.DocType == (int)DocumentType.PermittedDiscount)
            {
                noteAr = NotesOfReciepts.PermittedDiscountAR + InvoicesCode.PermittedDiscount + " - " + DocCode;
                noteEn = NotesOfReciepts.PermittedDiscountEn + InvoicesCode.PermittedDiscount + " - " + DocCode;
            }


            financialData fa = await getFinantialAcc(parameter.Person);
            var JournalEntryDetail = new List<JournalEntryDetail>();
            var journalentryFA_ID = parameter.DocType == (int)DocumentType.AcquiredDiscount ?
                (int)_PurchasesAndSalesSettingsquery.TableNoTracking.Where(c => c.branchId == userInformation.CurrentbranchId).Where(x => x.RecieptsType == (int)DocumentType.AcquiredDiscount).FirstOrDefault().FinancialAccountId
                : (int)_PurchasesAndSalesSettingsquery.TableNoTracking.Where(c => c.branchId == userInformation.CurrentbranchId).Where(x => x.RecieptsType == (int)DocumentType.PermittedDiscount).FirstOrDefault().FinancialAccountId;
            JournalEntryDetail.AddRange(new[]
            {
                    // setting journalentry
                    new JournalEntryDetail
                    {
                    FinancialAccountId = journalentryFA_ID,
                    Credit = parameter.DocType == (int)DocumentType.AcquiredDiscount ?  parameter.amountMoney  : 0    ,  //دائن
                    Debit = parameter.DocType == (int)DocumentType.PermittedDiscount ?  parameter.amountMoney  : 0 ,     //مديين
                    DescriptionAr=noteAr,
                    DescriptionEn=noteEn,
                    },
                    // person
                    new JournalEntryDetail
                    {
                     FinancialAccountId=fa.financialId,
                     Credit = parameter.DocType == (int)DocumentType.PermittedDiscount ? parameter.amountMoney : 0  ,         //دائن
                     Debit = parameter.DocType == (int)DocumentType.AcquiredDiscount ? parameter.amountMoney : 0 ,         //مديين
                     DescriptionAr=noteAr,
                    DescriptionEn=noteEn,
                    }

            });
            return new journalDetailsDTO { notes = noteAr, journalEntryDetail = JournalEntryDetail };




        }

        public async Task<ResponseResult> UpdateDiscount(UpdateDiscountRequest parameter)
        {

            var userInformation = await _iUserInformation.GetUserInformation();

            var data = await Discount_A_PQuery.GetByAsync(a => a.Id == parameter.Id);
            if (data.IsDeleted)
                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed, Note = "Can not edit deleted element" };

            if (data.BranchId != userInformation.CurrentbranchId)
                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed, Note = Actions.CantEditOtherBranchesElements };

            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };

            if (parameter.amountMoney <= 0)
            {
                return new ResponseResult() { Note = Actions.AcmountCantNotBeZeroOrLess, Result = Result.Failed };
            }
            var findPerson = await _personsQuery.GetByIdAsync(parameter.Person);
            if (findPerson == null)
            {
                return new ResponseResult() { Note = Actions.PersonDoseNotExist, Result = Result.Failed };
            }
            if (findPerson.IsCustomer != parameter.IsCustomer)
                return new ResponseResult() { Note = Actions.PersonDoseNotExist, Result = Result.Failed };
            parameter.DocDate = generalAPIsService.serverDate(parameter.DocDate);

            if (data.DocType == (int)DocumentType.AcquiredDiscount)
            {
                // data.DocNumber = parameter.BranchId + "-" + Aliases.AcquiredDiscount + "-" + NextCode.ToString();
                data.Amount = -1 * parameter.amountMoney;
                data.Creditor = 0;
                data.Debtor = parameter.amountMoney;
            }
            else if (data.DocType == (int)DocumentType.PermittedDiscount)
            {
                // data.DocNumber = parameter.BranchId + "-" + Aliases.PermittedDiscount + "-" + NextCode.ToString();
                data.Amount = parameter.amountMoney;
                data.Creditor = parameter.amountMoney;
                data.Debtor = 0;
            }
            data.Notes = parameter.Notes;
            data.PaperNumber = parameter.PaperNumber;
            data.DocDate = parameter.DocDate;
            data.IsCustomer = parameter.IsCustomer;
            data.PersonId = parameter.Person;
            //data.DocType = parameter.DocType;
            data.IsDeleted = false;
            data.Refrience = data.DocNumber;
            var person = await getPerson(data.PersonId);
            //  var code = await getCode();

            var saved = await Discount_A_PCommand.UpdateAsyn(data);
            if (saved)
            {
                var recTypeId = parameter.DocType;// == (int)DocumentType.AcquiredDiscount ? (int)RecieptsParentType.AcquiredDiscount : (int)RecieptsParentType.PermittedDiscount);
                var findReceipt = _recieptRepositoryQuery.TableNoTracking.Where(x => x.ParentTypeId == recTypeId && x.ParentId == data.Id).FirstOrDefault();
                var recUpdated = await _receiptsService.UpdateReceipt(new Domain.UpdateRecieptsRequest()
                {
                    Id = findReceipt.Id,
                    Amount = data.Amount < 0 ? (data.Amount * -1) : data.Amount,
                    Creditor = data.Creditor,
                    Debtor = data.Debtor,
                    PaperNumber = data.PaperNumber,
                    Notes = data.Notes,

                    RecieptTypeId = data.DocType,
                    BranchId = userInformation.CurrentbranchId,
                    UserId = userInformation.userId,
                    IsIncludeVat = false,
                    Authority = parameter.IsCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers,
                    Code = data.Code,
                    IsAccredit = false,
                    ParentId = data.Id,
                    ParentTypeId = data.DocType, //== (int)DocumentType.AcquiredDiscount ? (int)RecieptsParentType.AcquiredDiscount : (int)RecieptsParentType.PermittedDiscount,
                    ParentType = data.BranchId.ToString() + "-" + data.DocNumber,
                    BenefitId = data.PersonId,
                    PaymentMethodId = 1,
                    ReceiptOnly = true,
                    Deferre = true,
                    RecieptDate = parameter.DocDate,
                    FA_Id = parameter.DocType == (int)DocumentType.AcquiredDiscount ? (int)_PurchasesAndSalesSettingsquery.TableNoTracking.Where(c => c.branchId == userInformation.CurrentbranchId).Where(x => x.RecieptsType == (int)DocumentType.AcquiredDiscount).FirstOrDefault().FinancialAccountId : (int)_PurchasesAndSalesSettingsquery.TableNoTracking.Where(c => c.branchId == userInformation.CurrentbranchId).Where(x => x.RecieptsType == (int)DocumentType.PermittedDiscount).FirstOrDefault().FinancialAccountId
                });
            }
            await UpdateJEntery(parameter, userInformation, data.Code, data.Id);
            var TotalAmount = await _receiptsService.GetReceiptBalanceForBenifit(person.IsCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers, person.Id);

            history.AddHistory(data.Id, "", "", Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            SystemActionEnum systemActionEnum = new SystemActionEnum();
            if (parameter.DocType == (int)DocumentType.AcquiredDiscount)
                systemActionEnum = SystemActionEnum.editAcquiredDiscount;
            else if (parameter.DocType == (int)DocumentType.PermittedDiscount)
                systemActionEnum = SystemActionEnum.editPermittedDiscount;
            await _systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);
            return new ResponseResult() { Data = TotalAmount, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };

        }

        public async Task<ResponseResult> GetListOfDiscounts(DiscountSearch parameter)
        {
            // front Send Doc type (2 or 3)
            var userInformation = await _iUserInformation.GetUserInformation();
            var branches = _gLBranchQuery.TableNoTracking;
            var _discounts = Discount_A_PQuery
                                    .TableNoTracking
                                    .Include(x => x.Person)
                                    .Where(x => x.DocType == parameter.DocType && x.BranchId == userInformation.CurrentbranchId);
            var totalCount = _discounts.Count();

            if (totalCount == 0)
                return new ResponseResult() { DataCount = totalCount, Id = null, Result = Result.NoDataFound };
            var discounts = _discounts;
            if (parameter.DiscountId > 0)
            {
                discounts = _discounts.Where(x => x.Id == parameter.DiscountId);
                if (discounts.Count() == 0)
                    return new ResponseResult() { DataCount = totalCount, Id = null, Result = Result.Success };
            }


            if (string.IsNullOrEmpty(parameter.CodeOrPaperNumber))
                discounts = discounts.OrderByDescending(x => x.Id);
            else
                discounts = discounts.OrderBy(x => x.Code).ThenByDescending(x => x.Person.ArabicName);

            if (!string.IsNullOrEmpty(parameter.CodeOrPaperNumber))
                discounts = discounts.Where(a => (a.BranchId.ToString() + "-" + a.DocNumber) == parameter.CodeOrPaperNumber || a.DocNumber == parameter.CodeOrPaperNumber || a.Code.ToString() == parameter.CodeOrPaperNumber || a.Person.ArabicName.Contains(parameter.CodeOrPaperNumber) || a.Person.LatinName.Contains(parameter.CodeOrPaperNumber) || a.PaperNumber.Contains(parameter.CodeOrPaperNumber));

            var dataCount = discounts.Count();
            if (totalCount > 0 && parameter.PageNumber != 0 && parameter.PageSize != 0)
                discounts = discounts.Skip((parameter.PageNumber - 1) * parameter.PageSize).Take(parameter.PageSize);
            if (!string.IsNullOrEmpty(parameter.DateFrom.ToString()) || !string.IsNullOrEmpty(parameter.DateTo.ToString()))
                discounts = discounts.Where(x => x.DocDate.Date >= parameter.DateFrom.Value.Date && x.DocDate.Date <= parameter.DateTo.Value.Date);

            var _persons = discounts.Select(x => x.Person).Select(x => new { x.Id, x.ArabicName, x.LatinName }).ToList().Distinct();

            var personsTotalAmount = new List<personsTotalMonay>();
            foreach (var item in discounts)
            {
                var totalAmount = await _receiptsService.GetReceiptBalanceForBenifit(item.IsCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers, item.PersonId);
                personsTotalAmount.Add(new personsTotalMonay
                {
                    Id = item.Id,
                    totalMoney = totalAmount.Total
                });

            }

            var _res = discounts.ToList()
            .Select(x => new discountsResponseDTO
            {
                id = x.Id,
                code = x.Code,
                docNumber = branches.Where(c => c.Id == x.BranchId).FirstOrDefault().Code + "-" + x.DocNumber,
                paperNumber = x.PaperNumber,
                docDate = x.DocDate.ToString(defultData.datetimeFormat),
                isCustomer = x.IsCustomer,
                personId = x.PersonId,
                amountMoney = x.Amount < 0 ? (x.Amount * -1) : x.Amount,
                totalAmountMoney = 0.0,
                notes = x.Notes,
                docType = x.DocType,
                isDeleted = x.IsDeleted,
                canDelete= !x.IsDeleted? true : false,
                refrience = x.Refrience,
                branchId = x.BranchId,
                person = _persons.Where(c => c.Id == x.PersonId).FirstOrDefault(),
            }).ToList();
            var responseList = new List<discountsResponseDTO>();
            foreach (var item in _res)
            {
                item.totalAmountMoney = personsTotalAmount.Where(c => c.Id == item.id).FirstOrDefault()?.totalMoney ?? 0;
                responseList.Add(item);
            }

            var count = _res.Count();
            return new ResponseResult()
            {
                TotalCount = totalCount,
                Data = responseList,
                DataCount = dataCount,
                Id = null,
                Result = _res.Any() ? Result.Success : Result.NotFound
            };

        }

        public async Task<WebReport> DiscountsReport(int documentId, int documentType, exportType exportType, bool isArabic, int fileId = 0)
        {

            var branches = _gLBranchQuery.TableNoTracking;



            var userInfo = await _iUserInformation.GetUserInformation();
            var _discount = Discount_A_PQuery.TableNoTracking.Include(x => x.Person).Where(x => x.DocType == documentType && x.BranchId == userInfo.CurrentbranchId && x.Id == documentId).FirstOrDefault();


            var mainData = new discountsResponseDTO()
            {
                docDate = _discount.DocDate.ToString("yyyy/MM/dd"),
                docNumber = branches.Where(c => c.Id == _discount.BranchId).FirstOrDefault().Code + "-" + _discount.DocNumber,

                paperNumber = _discount.PaperNumber,
                amountMoney = _discount.Amount,
                notes = _discount.Notes,
                docType = _discount.DocType,
                personId = _discount.PersonId,



            };
            string Amount = null;
            mainData.amountMoney = Math.Abs(mainData.amountMoney);
            Amount = mainData.amountMoney.ToString();

            ReportOtherData otherData = new ReportOtherData()
            {
                ArabicName = _discount.Person.ArabicName,
                LatinName = _discount.Person.LatinName,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                Currency = ConvertNumberToText.GetText(Amount, isArabic)

            };
            int screenId = 0;
            if (documentType == 14)
            {
                screenId = (int)SubFormsIds.EarnedDiscount_Settelments;



                // Currency= ConvertNumberToText.GetText(actualObject.Amount.ToString())

            }
            else if (documentType == 15)
            {
                screenId = (int)SubFormsIds.PermittedDiscount_Settelments;

            }

            history.AddHistory(_discount.Id, "", "", exportType.ToString(), Aliases.TemporaryRequiredData.UserName);


            var tablesNames = new TablesNames()
            {
                ObjectName = "EarnedPermittedDiscount"
            };

            var report = await _iGeneralPrint.PrintReport<discountsResponseDTO, object, object>(mainData, null, null, tablesNames, otherData
             , screenId, exportType, isArabic,fileId);
            return report;


        }

        public async Task<ResponseResult> DeleteDiscount(SharedRequestDTOs.Delete ListCode)
        {
            var discounts = await Discount_A_PQuery.GetAllAsyn(x => ListCode.Ids.Contains(x.Id));
            //logs
            if (discounts.Where(x => x.DocType == (int)DocumentType.AcquiredDiscount).Any())
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settelments, (int)SubFormsIds.EarnedDiscount_Settelments, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            if (discounts.Where(x => x.DocType == (int)DocumentType.PermittedDiscount).Any())
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settelments, (int)SubFormsIds.PermittedDiscount_Settelments, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            //end logs 

            //delete discount
            discounts.ToList().ForEach(x => x.IsDeleted = true);

            //delete journal entery
            await DeleteJEntry(discounts.Select(c => c.DocType).ToArray(), discounts.Select(c => c.Id).ToArray());

            if (await Discount_A_PCommand.UpdateAsyn(discounts))
            {
                var disId = discounts.Select(x => (int)x.Id).ToArray();
                var recsId = _recieptRepositoryQuery.TableNoTracking.Where(x => (x.ParentTypeId == (int)DocumentType.AcquiredDiscount || x.ParentTypeId == (int)DocumentType.PermittedDiscount) && disId.Contains(x.ParentId ?? 0)).Select(x => x.Id).ToList();
                var deleted = await recieptRepositoryCommand.DeleteAsync(a => recsId.Contains(a.Id));
                if (deleted)
                {
                    SystemActionEnum systemActionEnum = new SystemActionEnum();
                    if (discounts.Where(x => x.DocType == (int)DocumentType.AcquiredDiscount).Any())
                        await _systemHistoryLogsService.SystemHistoryLogsService(SystemActionEnum.deleteAcquiredDiscount, discounts.Count);
                    else if (discounts.Where(x => x.DocType == (int)DocumentType.PermittedDiscount).Any())
                        await _systemHistoryLogsService.SystemHistoryLogsService(SystemActionEnum.deletePermittedDiscount,discounts.Count);
                    foreach (var item in discounts)
                    {
                        history.AddHistory(item.Id, "", "", Aliases.HistoryActions.Delete, Aliases.TemporaryRequiredData.UserName);


                    }
                    return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
                }

            }
            return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };
            #region old
            //List<InvDiscount_A_P> DeletedDiscountList = new List<InvDiscount_A_P>();

            ////for each selected code
            //// new deleted document take the same code , switch amount in acreditor and debtor   
            //// , change the doctype to AcquiredDiscount_Delete
            //// last data is the same .

            //foreach (int discountId in ListCode.Ids)
            //{
            //    var oldData = await Discount_A_PQuery.GetByAsync(a => a.Id == discountId);

            //    oldData.IsDeleted = true;
            //    // make main document as deleted
            //    var data = new InvDiscount_A_P();
            //    await Discount_A_PCommand.UpdateAsyn(oldData);
            //    data.Code = discountId;
            //    //  data.DocNumber = oldData.DocNumber + "-D";
            //    data.Amount = oldData.Amount;
            //    if (oldData.DocType == (int)DocumentType.AcquiredDiscount) // خصم مكتسب دائن
            //    {
            //        data.Creditor = oldData.Debtor;
            //        data.Debtor = 0;
            //        data.DocType = (int)DocumentType.DeleteAcquiredDiscount;
            //        data.DocNumber = oldData.BranchId + "-" + Aliases.InvoicesCode.DeleteAcquiredDiscount + "-" + discountId;
            //    }
            //    else if (oldData.DocType == (int)DocumentType.PermittedDiscount) // خصم مسموح به مدين
            //    {
            //        data.Creditor = 0;
            //        data.Debtor = oldData.Creditor;
            //        data.DocType = (int)DocumentType.DeletePermittedDiscount;
            //        data.DocNumber = oldData.BranchId + "-" + Aliases.InvoicesCode.DeletePermittedDiscount + "-" + discountId;

            //    }
            //    data.Notes = oldData.Notes;
            //    data.PaperNumber = oldData.PaperNumber;
            //    data.DocDate = oldData.DocDate;
            //    data.IsCustomer = oldData.IsCustomer;
            //    data.PersonId = oldData.PersonId;
            //    data.IsDeleted = true;
            //    data.Refrience = oldData.DocNumber;
            //    DeletedDiscountList.Add(data);

            //}
            //Discount_A_PCommand.AddRange(DeletedDiscountList);

            //return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
            #endregion
        }


        public async Task<ResponseResult> GetDiscountHistory(int DiscountId)
        {
            var discount = Discount_A_PQuery.TableNoTracking.Where(x => x.Id == DiscountId).FirstOrDefault();
            if (discount.DocType == (int)DocumentType.AcquiredDiscount)
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settelments, (int)SubFormsIds.EarnedDiscount_Settelments, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            if (discount.DocType == (int)DocumentType.PermittedDiscount)
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settelments, (int)SubFormsIds.PermittedDiscount_Settelments, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            return await history.GetHistory(a => a.EntityId == DiscountId);
        }

        public async Task<ResponseResult> GetDiscountById(int Id)
        {
            var branches = _gLBranchQuery.TableNoTracking;
            var _persons = _personsQuery.TableNoTracking.Select(x => new { x.Id, x.ArabicName, x.LatinName }).ToList().Distinct();

            var elem = Discount_A_PQuery
                .TableNoTracking
                .Where(c => c.Id == Id).FirstOrDefault();


            if (elem == null)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        titleAr = "خطأ",
                        titleEn = "Error",
                        MessageAr = "هذا الخصم غير موجود",
                        MessageEn = "This discount is not exist"
                    }
                };
            var totalAmount = await _receiptsService.GetReceiptBalanceForBenifit(elem.IsCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers, elem.PersonId);
            discountsResponseDTO res = new discountsResponseDTO
            {
                id = elem.Id,
                code = elem.Code,
                docNumber = branches.Where(c => c.Id == elem.BranchId).FirstOrDefault().Code + "-" + elem.DocNumber,
                paperNumber = elem.PaperNumber,
                docDate = elem.DocDate.ToString(defultData.datetimeFormat),
                isCustomer = elem.IsCustomer,
                personId = elem.PersonId,
                amountMoney = elem.Amount < 0 ? (elem.Amount * -1) : elem.Amount,
                totalAmountMoney = totalAmount.Total,
                notes = elem.Notes,
                docType = elem.DocType,
                isDeleted = elem.IsDeleted,
                canDelete = elem.IsDeleted ? true : false,
                refrience = elem.Refrience,
                branchId = elem.BranchId,
                person = _persons.Where(c => c.Id == elem.PersonId).FirstOrDefault(),
            };
            return new ResponseResult
            {
                Result = Result.Success,
                Data = res
            };
        }
    }
    public class personsTotalMonay
    {
        public int Id { get; set; }
        public double totalMoney { get; set; }
    }
}
