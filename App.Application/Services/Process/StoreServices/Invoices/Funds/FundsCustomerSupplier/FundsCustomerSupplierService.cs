using App.Application.Handlers.GeneralLedger;
using App.Application.Handlers.GeneralLedger.JournalEntry;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Domain.Models.Response.Store.Invoices;
using App.Infrastructure.settings;
using MediatR;

namespace App.Application.Services.Process.FundsCustomerSupplier
{
    public class FundsCustomerSupplierService : BaseClass, IFundsCustomerSupplierService
    {
        private readonly IRepositoryQuery<InvFundsCustomerSupplier> FundsCustomerSupplierRepositoryQuery;
        private readonly IRepositoryCommand<InvFundsCustomerSupplier> FundsCustomerSupplierRepositoryCommand;

        private readonly IRepositoryCommand<InvPersons> InvPersonsRepositoryCommand;
        private readonly IRepositoryCommand<InvGeneralSettings> _invGeneralSettingsCommand;
        private readonly IRepositoryCommand<GLJournalEntry> _gLJournalEntryCommand;
        private readonly IRepositoryQuery<InvPersons> InvPersonsRepositoryQuery;
        private readonly IReceiptsService _receiptsService;
        private readonly IGeneralAPIsService generalApiService;
        private readonly IRepositoryQuery<GlReciepts> _receiptQuery;
        private readonly IRepositoryCommand<GlReciepts> receiptCommand;
        private readonly IRepositoryQuery<GLJournalEntry> _gLJournalEntryQuery;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _gLPurchasesAndSalesSettingsQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly iAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IPrintService _iprintService;
        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly IMediator _mediator;

        public FundsCustomerSupplierService(IRepositoryQuery<InvFundsCustomerSupplier> _FundsCustomerSupplierRepositoryQuery,
                                  IRepositoryCommand<InvFundsCustomerSupplier> _FundsCustomerSupplierRepositoryCommand,
                                  IRepositoryCommand<InvPersons> _InvPersonsRepositoryCommand,
                                  IRepositoryCommand<InvGeneralSettings> InvGeneralSettingsCommand,
                                  IRepositoryCommand<GLJournalEntry> _GLJournalEntryCommand,
                                  IRepositoryQuery<InvPersons> _InvPersonsRepositoryQuery,
                                  IReceiptsService receiptsService,
                                  IGeneralAPIsService GeneralApiService,
                                  IRepositoryQuery<GlReciepts> receiptQuery,
                                  IRepositoryCommand<GlReciepts> receiptCommand,
                                  IRepositoryQuery<GLJournalEntry> _GLJournalEntryQuery,
                                  IRepositoryQuery<GLPurchasesAndSalesSettings> gLPurchasesAndSalesSettingsQuery,
                                  IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery,
                                  ISystemHistoryLogsService systemHistoryLogsService,
                                  iAuthorizationService authorizationService,
                                  IHttpContextAccessor _httpContext,
                                  IPrintService iprintService,
                                  IFilesMangerService filesMangerService,
                                  ICompanyDataService companyDataService,
                                  iUserInformation iUserInformation,
                                  IGeneralPrint iGeneralPrint,
                                  IMediator mediator) : base(_httpContext)
        {
            FundsCustomerSupplierRepositoryQuery = _FundsCustomerSupplierRepositoryQuery;
            FundsCustomerSupplierRepositoryCommand = _FundsCustomerSupplierRepositoryCommand;
            InvPersonsRepositoryCommand = _InvPersonsRepositoryCommand;
            _invGeneralSettingsCommand = InvGeneralSettingsCommand;
            _gLJournalEntryCommand = _GLJournalEntryCommand;
            InvPersonsRepositoryQuery = _InvPersonsRepositoryQuery;
            _receiptsService = receiptsService;
            generalApiService = GeneralApiService;
            _receiptQuery = receiptQuery;
            this.receiptCommand = receiptCommand;
            _gLJournalEntryQuery = _GLJournalEntryQuery;
            _gLPurchasesAndSalesSettingsQuery = gLPurchasesAndSalesSettingsQuery;
            _invGeneralSettingsQuery = InvGeneralSettingsQuery;
            _systemHistoryLogsService = systemHistoryLogsService;
            _authorizationService = authorizationService;
            httpContext = _httpContext;
            _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = companyDataService;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _mediator = mediator;
        }
        #region Helper 
        public async Task<bool> CheckFundSettings(bool isCustomer)
        {
            var settings = await _invGeneralSettingsQuery.GetByIdAsync(1);
            if (isCustomer)
                return settings.Funds_Customers;
            else
                return settings.Funds_Supplires;
        }
        public async Task DeletePersonFundFromJournalEntry(int[] ids, bool isCustomer)
        {
            //var tryDelete = await _journalEntryBusiness.removeStoreFundFromJournalDetiales(ids, isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds);
            var tryDelete = await _mediator.Send(new removeStoreFundFromJournalDetialesRequest
            {
                storeFundIds = ids,
                DocType = isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds
            });

        }
        public async Task GLRelation(List<supAndCustUpdateFund> supAndCustUpdateFunds, bool isCustomer, DateTime date, bool isUpdate = false)
        {
            var GLSettings = _gLPurchasesAndSalesSettingsQuery.TableNoTracking.Where(x => isCustomer ? x.RecieptsType == (int)DocumentType.CustomerFunds : x.RecieptsType == (int)DocumentType.SuplierFunds && x.branchId == 1);
            int Fund_FAId = GLSettings.OrderBy(x => x.Id).LastOrDefault().FinancialAccountId ?? 0;

            if (isUpdate)
                await DeletePersonFundFromJournalEntry(supAndCustUpdateFunds.Select(x => x.Id).ToArray(), isCustomer);
            //var persons = InvPersonsRepositoryQuery.TableNoTracking.Where(x=> x.IsCustomer == isCustomer);
            var invFundPersons = await FundsCustomerSupplierRepositoryQuery.GetAllIncludingAsync(0, 0, x => 1 == 1, x => x.Person);
            var EntryFundsList = new List<EntryFunds>();
            int docId = isCustomer ? -2 : -3;
            foreach (var item in supAndCustUpdateFunds)
            {
                if (EntryFundsList.Where(x => x.StoreFundId == item.Id).Any())
                    continue;
                //if (item.Credit == 0  && item.Debit == 0)
                //    continue;
                var TotalAmount = item.Credit - item.Debit;
                var recNoteAr = isCustomer ? "ارصدة اول المدة عملاء" : "ارصدة اول المدة موردين";
                var recNoteEn = isCustomer ? "Customer Entry Fund" : "Supplier Entry Fund";
                var personFA_Id = invFundPersons.Where(x => x.Id == item.Id).FirstOrDefault().Person.First().FinancialAccountId;
                EntryFundsList.AddRange(new[]
                                            {
                                                new EntryFunds
                                                {
                                                    Credit = TotalAmount > 0 ? TotalAmount : 0,
                                                    Debit = TotalAmount < 0 ? TotalAmount * -1 : 0,
                                                    DescriptionAr = recNoteAr,
                                                    DescriptionEn = recNoteEn,
                                                    FinancialAccountId = personFA_Id,
                                                    isStoreFund = true,
                                                    StoreFundId = item.Id,
                                                    JournalEntryId = docId,
                                                    DocType = isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds
                                                }
                                            });
            }
            var journalEntrySaed = await _mediator.Send(new addEntryFundsRequest
            {
                EntryFunds = EntryFundsList,
                date = date,
                note = "",
                isFund = true,
                docId = docId,
                Fund_FAId = Fund_FAId

            });
            //var _journalEntrySaed = await _journalEntryBusiness.addEntryFunds(new addEntryFunds()
            //{
            //    EntryFunds = EntryFundsList,
            //    date = date,
            //    note = "",
            //    isFund = true
            //}, docId, Fund_FAId);
        }
        #endregion//Helper
        public async Task<ResponseResult> GetListOfFundsCustomer(FundsCustomerandSupplierSearch parameters)
        {
            if (CheckFundSettings(true).Result)
                return new ResponseResult()
                {
                    Note = Actions.CustomersFundsIsClosed,
                    Result = Result.Failed
                };
            var result = await GetFundsCustomerandSuppliers(parameters, false);
            var res = new response()
            {
                FundsList = result.Data.FundsList,
                Date = result.Date.ToString(defultData.datetimeFormat)
            };
            return new ResponseResult() { TotalCount = result.totalCount, Data = res, DataCount = result.dataCount, Id = null, Result = Result.Success };
        }
        public async Task<EntryFundCustomerAndSuppliersResponse> GetFundsCustomerandSuppliers(FundsCustomerandSupplierSearch parameters, bool isSupplier, bool isPrint = false, string ids = null, bool isSearchData = true)
        {
            GetFundsResult result = new GetFundsResult();
            IList<InvPersons> resData = new List<InvPersons>();
            if (isSearchData)
            {
                resData =  InvPersonsRepositoryQuery.TableNoTracking.ToList();
            }
            else
            {
                string[] Ids = ids.Split(",");
                foreach (var id in Ids)
                {
                    var item = await InvPersonsRepositoryQuery.GetByIdAsync(Convert.ToInt32(id));
                    resData.Add(item);
                }

            }

            resData = resData.Where(x => x.FundsCustomerSuppliers != null && x.IsSupplier == isSupplier || x.IsCustomer == !isSupplier)
                             .OrderBy(x => x.Code)
                             .ToList();

            if (parameters.Type != 0)
            {
                resData = resData.Where(u => u.Type == parameters.Type).ToList();
            }
            //resData = resData.Where(x => x.FundsCustomerSuppliers.Credit != 0 || x.FundsCustomerSuppliers.Debit != 0).ToList();
            int totalCount = resData.Count;
            int dataCount = resData.Count;
            if (!string.IsNullOrEmpty(parameters.SearchCriteria))
            {
                resData = resData.Where(u =>
                                    u.Code.ToString().Contains(parameters.SearchCriteria.Trim()) ||
                                    (u.CodeT + "-" + u.Code.ToString()).ToString().Contains(parameters.SearchCriteria.Trim()) ||
                                    u.ArabicName.Contains(parameters.SearchCriteria.Trim()) ||
                                    u.LatinName.Contains(parameters.SearchCriteria.Trim()))
                                 .OrderBy(x => x.Code)
                                 .ToList();
                dataCount = resData.Count;
            }

            resData = isPrint ? resData : resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();


            result.FundsList = new List<FundsCustomerandSuppliersDto>();

            Mapping.Mapper.Map(resData, result.FundsList);
            if (!isPrint)
            {
                result.FundsList.ForEach(x =>
                {
                    x.Code = resData.Where(d => d.Id == x.PersonId).FirstOrDefault().CodeT + "-" + x.Code;
                });
            }

            var generalSettings = _invGeneralSettingsQuery.TableNoTracking.Where(x => x.Id == 1).FirstOrDefault();
            var Funds_Supplires_Date = generalSettings.Funds_Supplires_Date != null ? generalSettings.Funds_Supplires_Date : DateTime.Now;
            var Funds_Customers_Date = generalSettings.Funds_Customers_Date != null ? generalSettings.Funds_Customers_Date : DateTime.Now;
            var data = new EntryFundCustomerAndSuppliersResponse()
            {
                Data = result,
                Date = isSupplier ? Funds_Supplires_Date.Value : Funds_Customers_Date.Value,
                dataCount = dataCount,
                totalCount = totalCount
            };
            return data;
        }
        public async Task<WebReport> PersonsReport(FundsCustomerandSupplierSearch parameters, bool isSupplier, string ids, bool isSearchData, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await GetFundsCustomerandSuppliers(parameters, isSupplier, true, ids, isSearchData);

            var userInfo = await _iUserInformation.GetUserInformation();


            var mainData = data.Data.FundsList;

            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, DateTime.Now, DateTime.Now);


            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();




            int screenId = 0;
            if (isSupplier)
            {
                screenId = (int)SubFormsIds.Suppliers_Fund;

            }
            else
            {
                screenId = (int)SubFormsIds.Customres_Fund;

            }
            var tablesNames = new TablesNames()
            {
                FirstListName = "Persons"
            };
            var report = await _iGeneralPrint.PrintReport<object, FundsCustomerandSuppliersDto, object>(null, mainData, null, tablesNames, otherdata
                , screenId, exportType, isArabic, fileId);
            return report;

        }

        public async Task<ResponseResult> GetListOfFundsSuppliers(FundsCustomerandSupplierSearch parameters)
        {
            if (CheckFundSettings(false).Result)
                return new ResponseResult()
                {
                    Note = Actions.SuppliersFundsIsClosed,
                    Result = Result.Failed
                };
            var result = await GetFundsCustomerandSuppliers(parameters, true);
            var res = new response()
            {
                FundsList = result.Data.FundsList,
                Date = result.Date.ToString(defultData.datetimeFormat)
            };
            return new ResponseResult() { TotalCount = result.totalCount, Data = res, DataCount = result.dataCount, Id = null, Result = Result.Success };
        }
        public async Task<ResponseResult> UpdateFundsSuppliersAndCustomers(FundsCustomerandSupplierParameter parameters, bool isCustomer)
        {
            parameters.date = generalApiService.serverDate(parameters.date);

            if (CheckFundSettings(isCustomer).Result)
                return new ResponseResult()
                {
                    Note = isCustomer ? Actions.CustomersFundsIsClosed : Actions.SuppliersFundsIsClosed,
                    Result = Result.Failed
                };

            //update date only of
            await updateDate(isCustomer, parameters.date);

            var funds = await FundsCustomerSupplierRepositoryQuery.GetAllIncludingAsync(0, 0, x => 1 == 1, x => x.Person);
            funds = funds.Where(x => x.Person.First().IsCustomer == isCustomer || x.Person.First().IsSupplier == !isCustomer).ToList();

            if (funds == null)
                return new ResponseResult() { Note = Actions.NotFound, Result = Result.NotFound };

            var arr = parameters.listOfPersonsFunds.Select(d => d.Id).ToArray();
            funds = funds.Where(x => arr.Contains(x.Id)).ToList();

            var listOfPersonsFunds = parameters.listOfPersonsFunds.GroupBy(x => x.Id).Select(y => y.FirstOrDefault());

            funds.ToList().ForEach(x =>
            {
                x.Credit = listOfPersonsFunds.Where(c => c.Id == x.Id).Any() ? parameters.listOfPersonsFunds.Where(c => c.Id == x.Id).FirstOrDefault().Credit : 0;
                x.Debit = listOfPersonsFunds.Where(c => c.Id == x.Id).Any() ? parameters.listOfPersonsFunds.Where(c => c.Id == x.Id).FirstOrDefault().Debit : 0;
            });

            var saved = await FundsCustomerSupplierRepositoryCommand.UpdateAsyn(funds);

            if (saved)
            {
                DateTime dateCreation = parameters.date;
                var settings = await _invGeneralSettingsQuery.GetByIdAsync(1);
                //await updateDate(isCustomer, parameters.date);
                var listOfupdateRecModel = new List<updateRecModel>();

                foreach (var item in funds)//parameters.listOfPersonsFunds)
                {
                    listOfupdateRecModel.Add(new updateRecModel()
                    {
                        creditor = item.Credit,
                        depitor = item.Debit,
                        personId = item.PersonId
                    });
                }

                await updateRec(listOfupdateRecModel, isCustomer, dateCreation);
                await GLRelation(parameters.listOfPersonsFunds, isCustomer, isCustomer ? settings.Funds_Customers_Date.Value : settings.Funds_Supplires_Date.Value, true);
            }
            await _systemHistoryLogsService.SystemHistoryLogsService(isCustomer ? SystemActionEnum.editCustomersFund : SystemActionEnum.editSuppliersFund);
            return new ResponseResult() { Data = null, Result = saved ? Result.Success : Result.Success, Note = saved ? Actions.Success : Actions.SaveFailed };
        }
        public async Task updateDate(bool isCustomer, DateTime date)
        {
            var settings = await _invGeneralSettingsQuery.GetByIdAsync(1);
            if (isCustomer)
                settings.Funds_Customers_Date = date;
            else
                settings.Funds_Supplires_Date = date;

            var SettingsSaved = await _invGeneralSettingsCommand.UpdateAsyn(settings);
            var journalEntry = await _gLJournalEntryQuery.GetByIdAsync(isCustomer ? -2 : -3);
            journalEntry.FTDate = date;
            //updateReceipts 
            var receipts = await _receiptQuery.FindByAsyn(a => isCustomer ? a.ParentTypeId == (int)DocumentType.CustomerFunds : a.ParentTypeId == (int)DocumentType.SuplierFunds);
            receipts.ToList().ForEach(a => a.RecieptDate = a.CreationDate = date);
            await receiptCommand.SaveAsync();
            //end of update reciepts

            _gLJournalEntryCommand.SaveAsync();
            _gLJournalEntryCommand.ClearTracking();
        }
        public async Task updateRec(List<updateRecModel> updateRecModel, bool isCustomer, DateTime date)
        {
            var personsRecs = _receiptQuery
                .TableNoTracking
                .Where(x => updateRecModel.Select(c => c.personId).ToArray().Contains(x.PersonId.Value) && x.ParentTypeId == (isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds));
            foreach (var item in updateRecModel)
            {
                var amount = item.creditor - item.depitor;

                if (personsRecs.Where(x => x.PersonId == item.personId).Any())
                {
                    var updated = await _receiptsService.UpdateReceipt(new Domain.UpdateRecieptsRequest()
                    {
                        Id = personsRecs.Where(x => x.PersonId == item.personId).FirstOrDefault().Id,
                        SafeID = 1,
                        Creditor = amount >= 0 ? amount : 0,
                        Debtor = amount < 0 ? (amount * -1) : 0,
                        Amount = amount >= 0 ? amount : (amount * -1),
                        Authority = isCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers,
                        RecieptDate = date,
                        IsAccredit = false,
                        Notes = isCustomer ? "رصيد اول المدة عملاء" : "رصيد اول المدة موردين",
                        Deferre = true,

                        BenefitId = item.personId,
                        ParentType = InvoicesCode.SupplierFund + " - " + item.personId,
                        ParentId = 0,
                        RecieptTypeId = amount < 0 ? (int)DocumentType.SafeCash : (int)DocumentType.SafePayment,
                        ParentTypeId = isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds,
                        PaymentMethodId = 1,
                        ReceiptOnly = true
                    }, true);
                }
                else
                {
                    var added = await _receiptsService.AddReceipt(new Domain.RecieptsRequest()
                    {
                        SafeID = 1,
                        Creditor = amount >= 0 ? amount : 0,
                        Debtor = amount < 0 ? amount : 0,

                        Amount = amount > 0 ? amount : (amount * -1),
                        Authority = isCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers,
                        RecieptDate = date,
                        ParentId = 0,
                        IsAccredit = false,
                        Notes = isCustomer ? "رصيد اول المدة عملاء" : "رصيد اول المدة موردين",
                        Deferre = true,
                        BenefitId = item.personId,
                        RecieptTypeId = amount < 0 ? (int)DocumentType.SafeCash : (int)DocumentType.SafePayment,
                        ParentTypeId = isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds,
                        ParentType = InvoicesCode.SupplierFund + " - " + item.personId,
                        PaymentMethodId = 1,
                        ReceiptOnly = true


                    });

                }
            }



        }
        public async Task<ResponseResult> GetFundsById(int Id)
        {

            //var funds = await FundsCustomerSupplierRepositoryQuery.GetByAsync(q => q.Id == Id);
            var funds = FundsCustomerSupplierRepositoryQuery.TableNoTracking.Include(x => x.Person).Where(q => q.Id == Id).FirstOrDefault();
            if (CheckFundSettings(funds.Person.First().IsCustomer).Result)
                return new ResponseResult()
                {
                    Note = funds.Person.First().IsCustomer ? Actions.CustomersFundsIsClosed : Actions.SuppliersFundsIsClosed,
                    Result = Result.Failed
                };

            var isAuthorized = await _authorizationService.isAuthorized((int)MainFormsIds.ItemsFund, funds.Person.First().IsCustomer ? (int)SubFormsIds.Customres_Fund : (int)SubFormsIds.Suppliers_Fund, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;

            var person = InvPersonsRepositoryQuery.TableNoTracking.Where(x => x.Id == Id).FirstOrDefault();
            var fundsCustomerDto = new FundsCustomerandSuppliersDto();
            fundsCustomerDto.ArabicName = person.ArabicName;
            fundsCustomerDto.Code = person.CodeT + "-" + person.Code.ToString();
            fundsCustomerDto.Credit = funds.Credit;
            fundsCustomerDto.Debit = funds.Debit;
            fundsCustomerDto.LatinName = person.LatinName;
            fundsCustomerDto.PersonId = funds.PersonId;
            fundsCustomerDto.Type = person.Type;
            fundsCustomerDto.Id = funds.Id;
            //fundsCustomerDto.IsSupplier = funds.Person.IsSupplier;
            fundsCustomerDto.IsCustomer = person.IsCustomer;

            return new ResponseResult() { Data = fundsCustomerDto, Id = Id, Result = Result.Success };
        }

    }
    public class updateRecModel
    {
        public int personId { get; set; }
        public double creditor { get; set; }
        public double depitor { get; set; }
    }
}
