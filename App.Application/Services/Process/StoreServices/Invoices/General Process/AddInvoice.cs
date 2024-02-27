using App.Application.Handlers.GeneralAPIsHandler.PersonBalanceForPaymentMethod;
using App.Application.Handlers.Invoices.OfferPrice.UpdateOfferPriceStatus;
using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Response.GeneralLedger;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Infrastructure;
using Attendleave.Erp.Core.APIUtilities;
using Hangfire;
using MediatR;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Web.Http.Tracing;
using static App.Application.Services.Reports.Items_Prices.Rpt_Store;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.General_Process
{
    public class AddInvoice : BaseClass, IAddInvoice
    {

        //test 22
        private readonly IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositoryCommand;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvPersons> PersonRepositorQuery;
        private readonly IReceiptsFromInvoices ReceiptsInvoice;
        private readonly ISystemHistoryLogsService systemHistoryLogsService;
        private readonly IRepositoryCommand<InvPurchaseAdditionalCostsRelation> PurchaseAdditionalCostsRelationRepositoryCommand;
        private readonly IRepositoryCommand<InvoiceDetails> InvoiceDetailsRepositoryCommand;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;

        private readonly ICalculationSystemService CalcSystem;
        private readonly IRepositoryQuery<InvStpStores> storeQuery;
        private readonly ISecurityIntegrationService _securityIntegrationService;

        //private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly IGeneralAPIsService GeneralAPIsService;
        //  private SettingsOfInvoice SettingsOfInvoice;
        private readonly IHttpContextAccessor httpContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IRepositoryQuery<GlReciepts> recieptQuery;

        private readonly IFilesOfInvoices filesOfInvoices;
        private readonly IPaymentMethodsForInvoiceService paymentMethodsService;
        private readonly ISerialsService serialsService;
        private readonly IAccrediteInvoice accrediteInvoice;
        private readonly iInvoicesIntegrationService _iInvoicesIntegrationService;
        private readonly IGetInvoiceByIdService _getInvoiceByIdService;
        private readonly IRepositoryQuery<InvStoreBranch> invStoreBranchQuery;

        private readonly IRedefineInvoiceRequestService redefineInvoiceRequestService;
        private readonly iUserInformation Userinformation;
        private readonly IRoundNumbers roundNumbers;
        private readonly IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> settingService;
        private readonly IPersonLastPriceService personLastPriceService;
        private readonly IUpdateItemsPrices updateItemsPricesService;
        private readonly IMediator _mediator;
        public AddInvoice(
                              IRepositoryCommand<InvoiceMaster> _InvoiceMasterRepositoryCommand,
                              IRepositoryCommand<InvoiceDetails> _InvoiceDetailsRepositoryCommand,
                              IRepositoryCommand<InvPurchaseAdditionalCostsRelation> _PurchaseAdditionalCostsRelationRepositoryCommand,
                              IHistoryInvoiceService _HistoryInvoiceService,
                              IGeneralAPIsService _GeneralAPIsService,
                              IRepositoryQuery<InvPersons> _PersonRepositorQuery,
                              IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery,
                              IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery,
                               IWebHostEnvironment _hostingEnvironment,
                               ICalculationSystemService CalcSystem,
                               IRepositoryQuery<InvStpStores> storeQuery,
                               ISecurityIntegrationService securityIntegrationService,
                               IFilesOfInvoices filesOfInvoices, IPaymentMethodsForInvoiceService paymentMethodsService,
                                ISerialsService serialsService, IRepositoryQuery<GlReciepts> recieptQuery,
                                iUserInformation Userinformation, IAccrediteInvoice accrediteInvoice,
                                iInvoicesIntegrationService iInvoicesIntegrationService,
                                IReceiptsFromInvoices receiptsInvoice, ISystemHistoryLogsService systemHistoryLogsService,
                                IGetInvoiceByIdService getInvoiceByIdService, IRepositoryQuery<InvStoreBranch> invStoreBranchQuery,
                                IRedefineInvoiceRequestService redefineInvoiceRequestService, IRoundNumbers roundNumbers,
                                IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery,
        IHttpContextAccessor _httpContext, IRepositoryQuery<InvGeneralSettings> settingService, IPersonLastPriceService personLastPriceService, IMediator mediator, IUpdateItemsPrices updateItemsPricesService) : base(_httpContext)
        {
            InvoiceMasterRepositoryCommand = _InvoiceMasterRepositoryCommand;
            InvoiceDetailsRepositoryCommand = _InvoiceDetailsRepositoryCommand;
            PersonRepositorQuery = _PersonRepositorQuery;
            PurchaseAdditionalCostsRelationRepositoryCommand = _PurchaseAdditionalCostsRelationRepositoryCommand;
            httpContext = _httpContext;
            HistoryInvoiceService = _HistoryInvoiceService;
            GeneralAPIsService = _GeneralAPIsService;
            this.InvoiceDetailsRepositoryQuery = InvoiceDetailsRepositoryQuery;
            this.InvoiceMasterRepositoryQuery = InvoiceMasterRepositoryQuery;
            this._hostingEnvironment = _hostingEnvironment;
            this.CalcSystem = CalcSystem;
            this.storeQuery = storeQuery;
            _securityIntegrationService = securityIntegrationService;
            this.filesOfInvoices = filesOfInvoices;
            this.paymentMethodsService = paymentMethodsService;
            this.serialsService = serialsService;
            this.recieptQuery = recieptQuery;
            this.Userinformation = Userinformation;
            this.accrediteInvoice = accrediteInvoice;
            ReceiptsInvoice = receiptsInvoice;
            this.systemHistoryLogsService = systemHistoryLogsService;
            _iInvoicesIntegrationService = iInvoicesIntegrationService;
            _getInvoiceByIdService = getInvoiceByIdService;
            this.invStoreBranchQuery = invStoreBranchQuery;
            this.redefineInvoiceRequestService = redefineInvoiceRequestService;
            this.roundNumbers = roundNumbers;
            this.itemCardPartsQuery = itemCardPartsQuery;
            this.settingService = settingService;
            this.personLastPriceService = personLastPriceService;
            _mediator = mediator;
            this.updateItemsPricesService = updateItemsPricesService;
        }
        public int Autocode(int BranchId, int invoiceType)
        {
            var Code = 1;
            Code = InvoiceMasterRepositoryQuery.GetMaxCode(e => e.Code, a => a.InvoiceTypeId == invoiceType && a.BranchId == BranchId);

            if (Code != null)
                Code++;

            return Code;
        } 
        // alaa
        public void setDefaultData(ref InvoiceMasterRequest parameter, int invoiceType, double recalcTotalDiscoubtRatio)
        {

            if (Lists.storesInvoicesList.Contains(invoiceType))
            {
                parameter.PersonId = null;
                parameter.Paid = parameter.Net;
                parameter.Remain = 0;
            }
            parameter.VirualPaid = parameter.Paid;
            if (Lists.POSInvoicesList.Contains(invoiceType))
            {
                //NewV
                //if (parameter.Paid > NetAfterRecalculate)
                //    parameter.Paid = NetAfterRecalculate;
                var paidvalue = parameter.Paid;
                if (parameter.PaymentsMethods.Count() == 1)
                {
                    parameter.PaymentsMethods.Select(a => a.Value = paidvalue).ToList();
                }
            }
            if (parameter.BookIndex == null)
                parameter.BookIndex = "";
            if (parameter.ParentInvoiceCode == null)
                parameter.ParentInvoiceCode = "";

            if (parameter.Notes == null)
                parameter.Notes = "";

            //  double paid = 0;
            //
            if (parameter.TotalDiscountRatio >= 100|| parameter.TotalDiscountValue >= 100|| recalcTotalDiscoubtRatio>=100)
            {
                 parameter.Paid = 0;
                parameter.PaymentsMethods.RemoveAll(a=>true);
            }
               


            //else
            //{
            //    paid = parameter.Paid;
            //}

            //  return new ResponseResult() { Result = Result.Success };

        }

        public async Task<ResponseResult> SaveInvoice(InvoiceMasterRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice, int invoiceTypeId, string InvoiceTypeName, int MainInvoiceId, string fileDirectory, bool rejectedTransfer)
        {
            return await SaveInvoice(parameter, setting, SettingsOfInvoice, invoiceTypeId, InvoiceTypeName, MainInvoiceId, fileDirectory, rejectedTransfer, null, null,0);

        }
        public async Task<ResponseResult> SaveInvoice(InvoiceMasterRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice, int invoiceTypeId, string InvoiceTypeName, int MainInvoiceId, string fileDirectory, int transStatusOfSerial)
        {
            return await SaveInvoice(parameter, setting, SettingsOfInvoice, invoiceTypeId, InvoiceTypeName, MainInvoiceId, fileDirectory, false, null, null,transStatusOfSerial);


        }
        public async Task<ResponseResult> SaveInvoice(InvoiceMasterRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice, int invoiceTypeId, string InvoiceTypeName, int MainInvoiceId, string fileDirectory)
        {
            return await SaveInvoice(parameter, setting, SettingsOfInvoice, invoiceTypeId, InvoiceTypeName, MainInvoiceId, fileDirectory, false, null, null, 0);


        }
        public async Task<ResponseResult> SaveInvoice(InvoiceMasterRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice, int invoiceTypeId, string InvoiceTypeName, int MainInvoiceId, string fileDirectory, bool rejectedTransfer, int? transferStore, int? inCommingTransferCode, int transStatusOfSerial)
        {
            var security = await _securityIntegrationService.getCompanyInformation();
            if (!security.isInfinityNumbersOfInvoices)
            {
                var invoicesCount = InvoiceMasterRepositoryQuery.TableNoTracking.Where(x => x.InvoiceTypeId == invoiceTypeId).Count();
                if (invoicesCount >= security.AllowedNumberOfInvoices)
                    return new ResponseResult()
                    {
                        Note = Actions.YouHaveTheMaxmumOfInvoices,
                        Result = Result.MaximumLength,
                        ErrorMessageAr = "تجاوزت الحد الاقصي من عدد الفواتير",
                        ErrorMessageEn = "You Cant add a new invoice because you have the maximum of invoices for your bunlde",
                    };
            }

            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            if(Lists.POSInvoicesList.Where(x=> x != (int)DocumentType.ReturnPOS).Contains( invoiceTypeId))
            {
                var session = GeneralAPIsService.userHasSession(userInfo.POSSessionId);
                if (!session.Item1)
                    return session.Item2;
            }

            var valid = GeneralAPIsService.ValidationOfInvoices(parameter, invoiceTypeId, setting, userInfo.CurrentbranchId, rejectedTransfer, userInfo.userStors, DateTime.Now , false);
            if (valid.Result.Result != Result.Success)
                return valid.Result;
            var currentBranch = invStoreBranchQuery.TableNoTracking
                    .Where(a => a.StoreId == parameter.StoreId).First().BranchId;

            // redefine Invoice Request to avoid any error eccured while creating invoice
            if (invoiceTypeId != (int)DocumentType.IncomingTransfer)
            {
                var redefineInvoiceRequest = await redefineInvoiceRequestService.setInvoiceRequest(parameter, setting, invoiceTypeId,userInfo, parameter.ParentInvoiceCode,null, MainInvoiceId);
                if (redefineInvoiceRequest.Item2 != "")
                    return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = redefineInvoiceRequest.Item2, ErrorMessageEn = redefineInvoiceRequest.Item3 };
                parameter = redefineInvoiceRequest.Item1;
            }
            parameter.InvoiceDate = GeneralAPIsService.serverDate(parameter.InvoiceDate);
            // int NextCode = Autocode(currentBranch, invoiceTypeId);
            InvoiceMasterRepositoryCommand.StartTransaction();


            try
            {
                var signal = GeneralAPIsService.GetSignal(invoiceTypeId);
                if (!Lists.QuantityNotCheckedInvoicesList.Contains(invoiceTypeId))
                {
                    var invoiceId = 0;
                    if (Lists.returnInvoiceList.Contains(invoiceTypeId) || invoiceTypeId == (int)DocumentType.IncomingTransfer)
                        invoiceId = MainInvoiceId;

                    if (Lists.InvoicesTypesOfExtractFromStore.Contains(invoiceTypeId))
                    {
         // check quantity for items before save for extract from store
                       
                            var quantityAvailable = await GeneralAPIsService.checkQuantityBeforeSaveInvoiceForExtract(parameter.InvoiceDetails, parameter.StoreId, parameter.InvoiceDate, invoiceId, setting, invoiceTypeId, signal, parameter.StoreId, false,parameter.ParentInvoiceCode);
                            if (!quantityAvailable.Item1)
                                return new ResponseResult()
                                { Result = Result.QuantityNotavailable, ErrorMessageAr = quantityAvailable.Item2, ErrorMessageEn = quantityAvailable.Item3 };

                    }
                    else if (Lists.InvoicesTypeOfAddingToStore.Contains(invoiceTypeId))
                    {
                        if(!(Lists.returnInvoiceList.Contains(invoiceTypeId) && string.IsNullOrEmpty(parameter.ParentInvoiceCode)))
                        {
                            var quantityAvailable = GeneralAPIsService.checkQuantityBeforeSaveInvoiceForAdd(parameter.InvoiceDetails, parameter.StoreId, parameter.InvoiceDate, invoiceId, setting, invoiceTypeId, signal, parameter.StoreId, rejectedTransfer).Result;
                            if (!quantityAvailable.Item1)
                                return new ResponseResult()
                                { Result = Result.QuantityNotavailable, ErrorMessageAr = quantityAvailable.Item2, ErrorMessageEn = quantityAvailable.Item3 };
                        }
                    }
                }

                var serialExist = serialsService.checkSerialBeforeSave(false, null, parameter.InvoiceDetails, invoiceTypeId, parameter.StoreId,parameter.StoreId);
                // check serials 
                if (serialExist.Result.Result != Result.Success)
                    return new ResponseResult()
                    {
                        Data = serialExist.Result.Data,
                        Id = null,
                        Result = serialExist.Result.Result,
                        Note = serialExist.Result.ErrorMessageEn,
                        ErrorMessageAr = serialExist.Result.ErrorMessageAr,
                        ErrorMessageEn = serialExist.Result.ErrorMessageEn
                    };

                //setting validation
                if (invoiceTypeId == (int)DocumentType.Purchase || invoiceTypeId == (int)DocumentType.ReturnPurchase 
                    || invoiceTypeId == (int)DocumentType.wov_purchase || invoiceTypeId == (int)DocumentType.ReturnWov_purchase)
                {

                    if (setting.Purchases_PayTotalNet == true)
                    {
                        //if (!userInfo.otherSettings.purchasesAllowCreditSales)
                        if (parameter.Paid < parameter.Net)
                        {
                            return new ResponseResult()
                            {
                                Data = null,
                                Id = null,
                                Result = Result.NotTotalPaid,
                                Note = "you should send total net equal to total payments"
                                                        ,
                                ErrorMessageAr = ErrorMessagesAr.PayTotalNet,
                                ErrorMessageEn = ErrorMessagesEn.PayTotalNet
                            };
                        }
                    }
                }

                parameter.SalesManId = parameter.SalesManId == 0 ? parameter.SalesManId = null : parameter.SalesManId;

                // recalculate results to avoid any changings in data from user
                CalculationOfInvoiceParameter calculationOfInvoiceParameter = new CalculationOfInvoiceParameter()
                {
                    DiscountType = parameter.DiscountType,
                    InvoiceTypeId = invoiceTypeId,
                    ParentInvoice = parameter.ParentInvoiceCode,
                    TotalDiscountRatio = parameter.TotalDiscountRatio,
                    TotalDiscountValue = parameter.TotalDiscountValue,
                    PersonId = parameter.PersonId,
                    isDiscountRatio = parameter.isDiscountRatio
                };
                Mapping.Mapper.Map<List<InvoiceDetailsRequest>, List<InvoiceDetailsAttributes>>(parameter.InvoiceDetails, calculationOfInvoiceParameter.itemDetails);
                var recalculate = await CalcSystem.StartCalculation(calculationOfInvoiceParameter);
                // Mapping.Mapper.Map<InvoiceResultCalculateDto, InvoiceMasterRequest>(  recalculate,parameter);
                int count = 0;
                foreach (var item in parameter.InvoiceDetails)//.Where(a => a.parentItemId == null || a.parentItemId == 0).ToList())
                {
                    item.Quantity = recalculate.itemsTotalList[count].Quantity;
                    item.Price = recalculate.itemsTotalList[count].Price;
                    item.SplitedDiscountValue = recalculate.itemsTotalList[count].SplitedDiscountValue;
                    item.SplitedDiscountRatio = recalculate.itemsTotalList[count].SplitedDiscountRatio;
                    item.DiscountValue = recalculate.itemsTotalList[count].DiscountValue;
                    item.DiscountRatio = recalculate.itemsTotalList[count].DiscountRatio;
                    item.VatValue = recalculate.itemsTotalList[count].VatValue;
                    item.Total = recalculate.itemsTotalList[count].ItemTotal;
                    item.TotalWithSplitedDiscount = recalculate.itemsTotalList[count].TotalWithSplitedDiscount;
                    item.VatValueForPrint = recalculate.itemsTotalList[count].VatValueForPrint;
                 
                    count++;
                }
              
                setDefaultData(ref parameter, invoiceTypeId, recalculate.TotalDiscountRatio);

                //check auto extract person balance
                if ((invoiceTypeId == (int)DocumentType.Sales || invoiceTypeId == (int)DocumentType.Purchase || invoiceTypeId == (int)DocumentType.wov_purchase)&& setting.other_AutoExtractFromCustomerSupplierBalance)
                {
                    
                    parameter.PaymentsMethods = await paymentMethodsService.AutoExtractFromPersonBalanceValidation(parameter, invoiceTypeId, recalculate.Net, setting.other_AutoExtractFromCustomerSupplierBalance);

                   
                }


                // القيمه المحدده تجاوزت السحب من رصيد المورد 


                var paidExceedNet = await paymentMethodsService.PaymentValidation( parameter, invoiceTypeId,recalculate.Net,userInfo.CurrentbranchId,0);
                if (paidExceedNet.Result != Result.Success)
                    return paidExceedNet;

                //transfer
                var invoice = new InvoiceMaster()
                {
                    POSSessionId=userInfo.POSSessionId,
                    EmployeeId = userInfo.employeeId,
                    BranchId = currentBranch,
                    InvoiceTypeId = invoiceTypeId,
                    BookIndex = (parameter.BookIndex != null ? parameter.BookIndex.Trim() : parameter.BookIndex),
                    InvoiceDate = parameter.InvoiceDate,
                    StoreId = parameter.StoreId,
                    StoreIdTo = parameter.StoreIdTo,
                    PersonId = parameter.PersonId,
                    Notes = (parameter.Notes != null ? parameter.Notes.Trim() : parameter.Notes),
                    TransferNotesAR = "",//TransferNotes.descAr,
                    TransferNotesEN = "",//TransferNotes.descEn,
                    DiscountType = parameter.DiscountType,
                    BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString(),
                    Net = recalculate.Net,// parameter.Net,
                    Paid = parameter.Paid,
                    VirualPaid = parameter.VirualPaid,// parameter.Paid,
                    Remain = roundNumbers.GetRoundNumber(recalculate.Net - parameter.Paid, setting.Other_Decimals),//recalculate.Net - parameter.Paid,
                    TotalDiscountRatio = recalculate.TotalDiscountRatio,// parameter.TotalDiscountRatio,
                    TotalVat = recalculate.TotalVat,// parameter.TotalVat,
                    TotalDiscountValue = recalculate.TotalDiscountValue, // parameter.TotalDiscountValue,
                    TotalPrice = recalculate.TotalPrice, // parameter.TotalPrice,
                    TotalAfterDiscount = recalculate.TotalAfterDiscount, // parameter.TotalAfterDiscount,
                    ParentInvoiceCode = parameter.ParentInvoiceCode,
                    SalesManId = parameter.SalesManId,
                    PriceListId = parameter.PriceListId,
                    transferStatus = parameter.transferStatus,
                    ActualNet=recalculate.ActualNet,
                    isDiscountRatio=parameter.isDiscountRatio,
                    isExpenses=parameter.isExpenses.Value , //مصروفات
                    //InvoiceSubTypesId = (!parameter.isExpenses.Value? 0 :
                    //                      (  invoiceTypeId==(int)DocumentType.Purchase?  (int)SubType.Expenses :
                    //                        ( invoiceTypeId == (int)DocumentType.ReturnPurchase ? (int)SubType.ReturnExpenses :0)))
                };
                // بتحدد طرق السدادا اجل او جزئى او مسدد
                if (!Lists.storesInvoicesList.Contains(invoiceTypeId))
                {
                    // determine type of payment
                    if (invoice.Paid == 0)
                    {
                        invoice.PaymentType = (int)PaymentType.Delay;
                    }
                    if (invoice.Paid < invoice.Net && invoice.Paid != 0)
                    {
                        invoice.PaymentType = (int)PaymentType.Partial;
                    }
                    if (invoice.Paid >= invoice.Net)
                    {
                        invoice.PaymentType = (int)PaymentType.Complete;
                    }
                }
                // payment of sales

                if (Lists.salesInvoicesList.Contains(invoiceTypeId)|| Lists.POSInvoicesList.Contains(invoiceTypeId))
                {
                    var Credit = PersonRepositorQuery.TableNoTracking.Where(a => a.Id == parameter.PersonId)
                            .Select(a => new { a.CreditLimit, a.CreditPeriod }).ToList();

                    if (Lists.salesInvoicesList.Contains(invoiceTypeId) && setting.Sales_PayTotalNet)
                    {
                        //if (!userInfo.otherSettings.salesAllowCreditSales)
                        if (parameter.Paid < invoice.Net)
                            return new ResponseResult()
                            {
                                Data = null,
                                Id = null,
                                Result = Result.NotTotalPaid,
                                Note = "you should send total net equal to total payments"
                                     ,
                                ErrorMessageAr = ErrorMessagesAr.PayTotalNet,
                                ErrorMessageEn = ErrorMessagesEn.PayTotalNet
                            };

                    }
                    else if (Lists.POSInvoicesList.Contains(invoiceTypeId)&& !setting.Pos_DeferredSale)
                    {
                        if ( parameter.Paid < invoice.Net)
                            return new ResponseResult()
                            {
                                Data = null,
                                Id = null,
                                Result = Result.DeferredSale,
                                Note = "Deferred sale is not permitted",
                                ErrorMessageAr = ErrorMessagesAr.DeferredSale,
                                ErrorMessageEn = ErrorMessagesEn.DeferredSale
                            };

                    }
                    else
                    {
                        if (!Lists.returnInvoiceList.Contains(invoiceTypeId))
                            if (invoice.PaymentType != (int)PaymentType.Complete)
                            {
                             
                                if (Credit.Count()>0)
                                {
                                    if(Credit.First().CreditLimit!=null)
                                    {
                                        var debitValueForCustomer = recieptQuery.TableNoTracking.Where(a =>a.BranchId==userInfo.CurrentbranchId && a.PersonId == parameter.PersonId).Sum(a => a.Creditor - a.Debtor);

                                        debitValueForCustomer += Credit.First().CreditLimit.Value;

                                        if (invoice.Remain > debitValueForCustomer)
                                            return new ResponseResult() { Data = null, Id = null, Result = Result.NotTotalPaid,
                                                Note = "You have exceeded your credit limit", 
                                                ErrorMessageAr =string.Concat( ErrorMessagesAr.CreditLimit," ", Credit.First().CreditLimit.Value," ريال "),
                                                ErrorMessageEn = string.Concat(ErrorMessagesEn.CreditLimit, " ", Credit.First().CreditLimit.Value , " SAR ")
                                            };

                                    }

                                   
                                }
                            }

                    }

                    if (invoice.PaymentType != (int)PaymentType.Complete && Credit.Count() > 0&& (invoiceTypeId == (int)DocumentType.Sales || invoiceTypeId == (int)DocumentType.POS))
                    {
                        if (Credit.First().CreditPeriod != null)
                        {
                            var FirstInvoice = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => a.BranchId == userInfo.CurrentbranchId 
                            && (a.InvoiceTypeId == (int)DocumentType.Sales || a.InvoiceTypeId == (int)DocumentType.POS )
                             && (a.IsReturn ? a.InvoiceSubTypesId == (int)SubType.PartialReturn : true) && !a.IsDeleted
                                                  && a.PersonId == parameter.PersonId && a.Paid < a.Net).OrderBy(a => a.InvoiceDate);
                            if (FirstInvoice.Count() > 0)
                            {
                                var CreditPeriodDate = FirstInvoice.First().InvoiceDate.AddDays(Credit.First().CreditPeriod.Value );
                                if (parameter.InvoiceDate >= CreditPeriodDate)
                                {
                                    return new ResponseResult()
                                    {
                                        Data = null,
                                        Id = null,
                                        Result = Result.NotTotalPaid,
                                        Note = "You have exceeded your credit period",
                                        ErrorMessageAr = string.Concat(ErrorMessagesAr.CreditPeriod, " ", Credit.First().CreditPeriod.Value, " يوم ")
                                            ,
                                        ErrorMessageEn = string.Concat(ErrorMessagesEn.CreditPeriod, " ", Credit.First().CreditPeriod.Value, " day ")
                                    };
                                }
                            }

                        }
                    }

                     

                }

              

                if (!Lists.storesInvoicesList.Contains(invoiceTypeId))
                    if ((parameter.Paid > 0 && parameter.PaymentsMethods.Count() == 0) || ((invoiceTypeId==(int)DocumentType.POS? !setting.Pos_DeferredSale:true) && parameter.PaymentsMethods.Select(a => a.Value).Sum() != invoice.Paid))
                    {
                        return new ResponseResult()
                        {
                            Data = null,
                            Id = null,
                            Result = Result.NotTotalPaid,
                            Note = "you should send total paid equal to total payments"
                           ,
                            ErrorMessageAr = ErrorMessagesAr.paymentMethodEqualPaid,
                            ErrorMessageEn = ErrorMessagesEn.paymentMethodEqualPaid
                        };

                    }

                invoice.RoundNumber = setting.Other_Decimals;
                if (SettingsOfInvoice != null)
                {
                    invoice.ActiveDiscount = SettingsOfInvoice.ActiveDiscount;
                    invoice.ApplyVat = SettingsOfInvoice.ActiveVat;
                    invoice.PriceWithVat = SettingsOfInvoice.PriceIncludeVat;
                    invoice.RoundNumber = SettingsOfInvoice.setDecimal;

                }

                if (Lists.returnInvoiceList.Contains(invoiceTypeId)) invoice.IsReturn = true;
                //    invoice.ParentInvoiceCode = invoice.InvoiceType;
                invoice.Serialize = Convert.ToDouble(GeneralAPIsService.CreateSerializeOfInvoice(invoiceTypeId, MainInvoiceId, invoice.BranchId).ToString());

                int NextCode = Autocode(currentBranch, invoiceTypeId);

                invoice.InvoiceType = userInfo.CurrentbranchCode.ToString() + "-" + InvoiceTypeName + "-" + NextCode;

                int nextTransferCode = NextCode;

                //  لو تحويل وارد مقبول
                if (invoiceTypeId == (int)DocumentType.IncomingTransfer)
                    invoice.InvoiceSubTypesId = (int)SubType.AcceptedTransfer;
                // تحويل وارد مرفوض 
                if (inCommingTransferCode != null)
                {
                    nextTransferCode = inCommingTransferCode.Value;
                    invoice.InvoiceType = userInfo.CurrentbranchCode.ToString() + "-" + InvoiceTypeName + "-" + nextTransferCode + "_";
                    invoice.InvoiceSubTypesId = (int)SubType.RejectedTransfer;
                    // make 0 to avoid bug in coding this branch el bug ->  ل الكميه المرفوضه بترجع للمخزن المحول منه وبتاخد كود جديد ومش بتظهر مع التحويلات 
                    nextTransferCode = 0;
                }
                invoice.InvoiceTransferType = invoice.InvoiceType.Replace("_", "");// userInfo.CurrentbranchCode.ToString() + "-" + InvoiceTypeName + "-" + nextTransferCode;
                invoice.Code = nextTransferCode;
                //invoice.InvoicePaymentsMethods.ToList();

                InvoiceMasterRepositoryCommand.AddWithoutSaveChanges(invoice);


                var saved = await InvoiceMasterRepositoryCommand.SaveAsync();

                if (!saved)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice master" };
                // this code will run in Approval of invoices



                if (MainInvoiceId == 0)
                    MainInvoiceId = invoice.InvoiceId;
                //    saved = false;
                saved = GeneralAPIsService.addSerialize(invoice.Serialize, MainInvoiceId, invoiceTypeId, invoice.BranchId);
                if (!saved)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild in add serialize" };


                // Add Purchase additional costs List
                if (Lists.purchasesWithOutDeleteInvoicesList.Contains(invoiceTypeId))
                {
                    if (parameter.OtherAdditionList != null)
                    {
                        if (parameter.OtherAdditionList.Count() > 0)
                        {
                            var purchaseAdditionalCostList = new List<InvPurchaseAdditionalCostsRelation>();
                            double totalOfAdditions = 0;
                            foreach (var item in parameter.OtherAdditionList)
                            {
                                var purchaseAdditionalCost = new InvPurchaseAdditionalCostsRelation();
                                purchaseAdditionalCost.InvoiceId = invoice.InvoiceId;
                                purchaseAdditionalCost.AddtionalCostId = item.AddtionalCostId;
                                purchaseAdditionalCost.Amount = item.Amount;
                                if (item.AdditionsType == (int)PurchasesAdditionalType.RatioOfInvoiceTotal)
                                    purchaseAdditionalCost.Total =Math.Round( (item.Amount / 100) * invoice.TotalPrice, invoice.RoundNumber );
                                else if (item.AdditionsType == (int)PurchasesAdditionalType.RatioOfInvoiceNet)
                                    purchaseAdditionalCost.Total = Math.Round((item.Amount / 100) * invoice.Net, invoice.RoundNumber );
                                else
                                    purchaseAdditionalCost.Total = Math.Round(item.Amount, invoice.RoundNumber );

                                totalOfAdditions += purchaseAdditionalCost.Total;
                                purchaseAdditionalCostList.Add(purchaseAdditionalCost);
                            }
                            invoice.TotalOtherAdditions = Math.Round(totalOfAdditions,invoice.RoundNumber);
                            PurchaseAdditionalCostsRelationRepositoryCommand.AddRange(purchaseAdditionalCostList);

                            var currentInvoiceMaster = await InvoiceMasterRepositoryQuery.GetByAsync(a => a.InvoiceId == invoice.InvoiceId);
                            currentInvoiceMaster.TotalOtherAdditions = Math.Round(totalOfAdditions, invoice.RoundNumber);
                            await InvoiceMasterRepositoryCommand.UpdateAsyn(currentInvoiceMaster);

                        }
                    }
                }

                // save details of items in the invoice
                var invoiceDetailsList = new List<InvoiceDetails>();
                if (parameter.InvoiceDetails != null)
                {
                    if (parameter.InvoiceDetails.Count() > 0)
                    {
                        if(invoiceTypeId != (int)DocumentType.IncomingTransfer)
                        if ((((invoiceTypeId != (int)DocumentType.POS && setting.Other_MergeItems == true)  || (invoiceTypeId == (int)DocumentType.POS  && setting.Other_MergeItems && setting.otherMergeItemMethod == "withSave")) || invoiceTypeId == (int)DocumentType.OutgoingTransfer) && !Lists.returnInvoiceList.Contains(invoiceTypeId)) //setting with marge
                        {
                            var invoiceDetailsListMerg = await GeneralAPIsService.MergeItems(parameter.InvoiceDetails.ToList(), invoiceTypeId);
                            parameter.InvoiceDetails = invoiceDetailsListMerg;
                        }

                        int index = 0;
                        if (invoiceTypeId == (int)DocumentType.OutgoingTransfer)
                            parameter.InvoiceDetails.Select(a => a.TransStatus = invoice.transferStatus).ToList();


                        foreach (var item in parameter.InvoiceDetails)
                        {

                            index++;
                            var invoiceDetails = new InvoiceDetails();

                            invoiceDetails = ItemDetails(invoice, item);

                            // invoiceDetails.indexOfItem = index;
                            if (Lists.returnInvoiceList.Contains(invoiceTypeId))
                            {
                                invoiceDetails.indexOfItem = item.IndexOfItem;
                            }
                            else
                            {
                                if (item.parentItemId != null && item.parentItemId != 0)
                                {
                                    item.IndexOfItem = 0;
                                    invoiceDetails.indexOfItem = 0;
                                    index--;
                                }
                                else
                                {
                                    if( item.IndexOfItem>0)
                                    { 
                                        item.IndexOfItem=item.IndexOfItem;
                                        invoiceDetails.indexOfItem = item.IndexOfItem;
                                    }
                                    else
                                    {
                                        item.IndexOfItem = index;
                                        invoiceDetails.indexOfItem = index;
                                    }
                               
                                }

                            }

                            // other additions 
                            if (parameter.OtherAdditionList.Count() > 0)
                            {
                                invoiceDetails.OtherAdditions = Math.Round((((item.Quantity * item.Price) / invoice.TotalPrice) * invoice.TotalOtherAdditions),10);
                            }
                                //if (item.ItemTypeId==(int)ItemTypes.Serial)
                                //  item.IndexOfItem = index;
                                invoiceDetailsList.Add(invoiceDetails);


                            
                        }

                        InvoiceDetailsRepositoryCommand.AddRange(invoiceDetailsList);

                        saved = await InvoiceDetailsRepositoryCommand.SaveAsync();
                        if (!saved)
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice details" };

                        // serials
                        var itemsWithSerialsFromRequest = parameter.InvoiceDetails.Where(a => a.ItemTypeId == (int)ItemTypes.Serial).ToList();
                        if (itemsWithSerialsFromRequest.Count() > 0)
                        {

                            if (Lists.InvoicesTypeOfAddingToStore.Contains(invoice.InvoiceTypeId))
                            {
                                var invoiceType_ = (invoiceTypeId == (int)DocumentType.IncomingTransfer ? invoice.InvoiceTransferType : invoice.InvoiceType);

                                var res = await serialsService.AddSerialsForAddedInvoice(invoiceType_, itemsWithSerialsFromRequest, null, parameter.StoreId, invoiceTypeId,transStatusOfSerial);
                                if (!string.IsNullOrEmpty(res))
                                    return new ResponseResult { Result = Result.Failed, ErrorMessageAr = res, ErrorMessageEn = res };

                            }
                            else if (Lists.InvoicesTypesOfExtractFromStore.Contains(invoice.InvoiceTypeId))
                            {
                                var res = await serialsService.AddSerialsForExtractInvoice(false, itemsWithSerialsFromRequest, invoice.InvoiceType, invoice.transferStatus);
                                if (!string.IsNullOrEmpty(res.Item2))
                                    return new ResponseResult { Result = Result.Failed, ErrorMessageAr = res.Item2, ErrorMessageEn = res.Item3 };

                            }

                        }
                    }
                }
                // Add new files

                saved = await filesOfInvoices.saveFilesOfInvoices(parameter.AttachedFile, currentBranch, fileDirectory, invoice.InvoiceId, false, null, false);
                if (!saved)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice files" };


               
                    
                
               

                var SavePaymentMethod_ = await paymentMethodsService.SavePaymentMethods(invoiceTypeId, parameter.PaymentsMethods, invoice.InvoiceId, invoice.BranchId, invoice.Paid, false, invoice.RoundNumber);
                if (!SavePaymentMethod_.Item1)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild in payment methods" ,ErrorMessageAr= SavePaymentMethod_ .Item2 , ErrorMessageEn= SavePaymentMethod_ .Item3};

               
                // updates in the main invoice
                if (Lists.returnInvoiceList.Contains(invoiceTypeId) && !string.IsNullOrEmpty(parameter.ParentInvoiceCode))
                {
                    var MainInvoiceDetails = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => a.InvoiceId == MainInvoiceId).ToList();
                    foreach (var item in parameter.InvoiceDetails)
                    {
                        // update returned quantity with the same factor of quantity
                        MainInvoiceDetails.Where(a => a.ItemId == item.ItemId && a.indexOfItem == item.IndexOfItem)
                                          .Select(a => { a.ReturnQuantity = (a.ReturnQuantity + ((item.Quantity * item.ConversionFactor) / a.ConversionFactor)); return a; }).ToList();

                    }
                    await InvoiceDetailsRepositoryCommand.UpdateAsyn(MainInvoiceDetails);
                    // calculate remain quantity to determine type of return  
                    var RemainQty = MainInvoiceDetails.ToList().Sum(a => (a.Quantity) - a.ReturnQuantity);

                    var MainInvoiceMaster = await InvoiceMasterRepositoryQuery.GetByAsync(a => a.InvoiceId == MainInvoiceId);

                    var currentInvoiceMaster = await InvoiceMasterRepositoryQuery.GetByAsync(a => a.InvoiceId == invoice.InvoiceId);

                    if (RemainQty > 0)
                    {
                        MainInvoiceMaster.InvoiceSubTypesId = (int)SubType.PartialReturn;
                        MainInvoiceMaster.IsReturn = true;
                        currentInvoiceMaster.InvoiceSubTypesId = (int)SubType.PartialReturn;
                        currentInvoiceMaster.IsReturn = true;

                    }
                    else
                    {
                        MainInvoiceMaster.IsReturn = true;
                        MainInvoiceMaster.InvoiceSubTypesId = (int)SubType.TotalReturn;
                        currentInvoiceMaster.IsReturn = true;
                        currentInvoiceMaster.InvoiceSubTypesId = (int)SubType.TotalReturn;
                    }
                    await InvoiceMasterRepositoryCommand.UpdateAsyn(MainInvoiceMaster);
                    await InvoiceMasterRepositoryCommand.UpdateAsyn(currentInvoiceMaster);

                    invoice.InvoiceSubTypesId = MainInvoiceMaster.InvoiceSubTypesId;
                }

                var parentForHistory = invoice.InvoiceType;
                //var print = settingService.TableNoTracking.FirstOrDefault().Purchases_PrintWithSave;

                //if (Lists.returnInvoiceList.Contains(invoiceTypeId) && print)
                //    parentForHistory ="";
                if (Lists.returnInvoiceList.Contains(invoiceTypeId))
                    parentForHistory = invoice.ParentInvoiceCode;

                HistoryInvoiceService.HistoryInvoiceMaster(invoice.BranchId, invoice.Notes, invoice.BrowserName, "A", null, invoice.BookIndex, invoice.Code
                 , invoice.InvoiceDate, invoice.InvoiceId, invoice.InvoiceType, invoice.InvoiceTypeId, invoice.InvoiceSubTypesId, invoice.IsDeleted, parentForHistory, invoice.Serialize, invoice.StoreId, invoice.TotalPrice);
                //if(invoice.InvoiceTypeId==(int)DocumentType.Purchase)
                //   await   accrediteInvoice.accrediteAllInvoice(null, invoice.InvoiceTypeId, invoice.InvoiceId);


                //InvoiceMasterRepositoryCommand.CommitTransaction();
                // Saving items in edited items entity for profit
                saved = await GeneralAPIsService.generateEditedItems(invoiceDetailsList, invoice.Serialize, false, invoice.InvoiceId,invoice.BranchId);
                if(!saved)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild in Edited Items" };

                if (Lists.purchasesWithoutVatInvoicesList.Contains(invoiceTypeId) || Lists.purchasesInvoicesList.Contains(invoiceTypeId) || Lists.salesInvoicesList.Contains(invoiceTypeId) || Lists.POSInvoicesList.Contains(invoiceTypeId) || Lists.storesInvoicesList.Where(x => !Lists.InvoiceCannotAddJournalEntery.Contains(x)).Contains(invoiceTypeId))
                {
                    if(!parameter.isExpenses.Value)
                    {
                        var setJournalEntry = await _iInvoicesIntegrationService.InvoiceJournalEntryIntegration(new PurchasesJournalEntryIntegrationDTO()
                        {
                            total = invoice.TotalPrice,
                            discount = invoice.TotalDiscountValue,
                            VAT = invoice.TotalVat,
                            net = invoice.ActualNet,
                            personId = invoice.PersonId,
                            invoiceId = invoice.InvoiceId,
                            InvoiceCode = invoice.InvoiceType,
                            isIncludeVAT = invoice.PriceWithVat,
                            isAllowedVAT = invoice.ApplyVat,
                            invDate = invoice.InvoiceDate,
                            DocType = (DocumentType)invoice.InvoiceTypeId,
                            InvDetails = invoiceDetailsList,
                            serialize = invoice.Serialize,
                            branchId = invoice.BranchId,
                            note=invoice.Notes
                        });
                        if (setJournalEntry.Result == Result.Failed)
                        {
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in Journal Entry" };
                        }
                    }
                 
                    //else
                    //{
                    //    IRepositoryActionResult res = (IRepositoryActionResult)setJournalEntry.Data;
                    //    if(res.Status != RepositoryActionStatus.Created )
                    //        return new ResponseResult { Result = Result.Failed, Note = "Faild in Journal Entry" };
                    //}

                    if (!Lists.storesInvoicesList.Contains(invoiceTypeId))
                    {
                        var setReceipt = await ReceiptsInvoice.AddReceiptsFromInvoices(invoice, invoice.InvoicePaymentsMethods != null ? invoice.InvoicePaymentsMethods.ToList() : null);
                        if(setReceipt.Result==Result.Failed)
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in Receipts" };

                    }
                }
                InvoiceMasterRepositoryCommand.CommitTransaction();

                SystemActionEnum systemActionEnum = new SystemActionEnum();
                if (invoice.InvoiceTypeId == (int)DocumentType.Sales)
                    systemActionEnum = SystemActionEnum.addSalesInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.Purchase)
                    systemActionEnum = SystemActionEnum.addPurchaseInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.ReturnSales)
                    systemActionEnum = SystemActionEnum.addReturnSalesInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.ReturnPurchase)
                    systemActionEnum = SystemActionEnum.addReturnPurchaseInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.ExtractPermission)
                    systemActionEnum = SystemActionEnum.addExtractPermission;
                else if (invoice.InvoiceTypeId == (int)DocumentType.AddPermission)
                    systemActionEnum = SystemActionEnum.addAddPermisson;
                else if (invoice.InvoiceTypeId == (int)DocumentType.itemsFund)
                    systemActionEnum = SystemActionEnum.addItemsEntryFund;
                else if (invoice.InvoiceTypeId == (int)DocumentType.POS)
                    systemActionEnum = SystemActionEnum.addPOSInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.ReturnPOS)
                    systemActionEnum = SystemActionEnum.addReturnPOSInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.OutgoingTransfer)
                    systemActionEnum = SystemActionEnum.OutGoingTransferInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.IncomingTransfer)
                    systemActionEnum = SystemActionEnum.IncomingTranferInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.OfferPrice)
                    systemActionEnum = SystemActionEnum.addOfferPrice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.wov_purchase)
                    systemActionEnum = SystemActionEnum.addWOVPurchase;
                else if (invoice.InvoiceTypeId == (int)DocumentType.ReturnWov_purchase)
                    systemActionEnum = SystemActionEnum.addReturnWOVPurchase;

                if (systemActionEnum != null)
                    await systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);


                //var setJournalEntry = iInvoicesIntegrationService.InvoiceJournalEntryIntegration()


                if(parameter.OfferPriceId>0)
                {
                    await _mediator.Send(new UpdateOfferPriceStatusRequest() { offerPriceId=parameter.OfferPriceId });
                }

                if (invoice.InvoiceTypeId == (int)DocumentType.POS || invoice.InvoiceTypeId == (int)DocumentType.Sales
                    || invoice.InvoiceTypeId == (int)DocumentType.Purchase || invoice.InvoiceTypeId == (int)DocumentType.wov_purchase)
                {
                    var personLastPriceRequest = new PersonLastPriceRequest()
                    {
                        personId = parameter.PersonId.Value,
                        invoiceDetails = parameter.InvoiceDetails,
                        invoiceTypeId = invoiceTypeId
                    };

                 //  BackgroundJob.Schedule(() => personLastPriceService.setLastPrice(personLastPriceRequest), TimeSpan.FromSeconds(30));
                await Task.Run(() => personLastPriceService.setLastPrice(personLastPriceRequest));
           
                    if(invoice.InvoiceTypeId == (int)DocumentType.Purchase && setting.Purchase_UpdateItemsPricesAfterInvoice)
                        await Task.Run(() => updateItemsPricesService.setItemsPrices(invoiceDetailsList));

                    // await personLastPriceService.setLastPrice(personLastPriceRequest);
                }
                if ((int)DocumentType.POS == invoiceTypeId || (int)DocumentType.ReturnPOS == invoiceTypeId)
                {
                    InvoiceDto data = await _getInvoiceByIdService.GetInvoiceDto(invoice.InvoiceId, false);

                    return new ResponseResult { employyeNameAr=userInfo.employeeNameAr.ToString(), employyeNameEn = userInfo.employeeNameAr.ToString(), permissionListId = userInfo.permissionListId, Result = Result.Success, Data = data, Id = invoice.InvoiceId, Code = invoice.Code };
                }
                
                return new ResponseResult { employyeNameAr = userInfo.employeeNameAr.ToString(), employyeNameEn = userInfo.employeeNameAr.ToString(), permissionListId = userInfo.permissionListId , Result = Result.Success, Id = invoice.InvoiceId, Code = invoice.Code };
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public InvoiceDetails ItemDetails(InvoiceMaster invoice, InvoiceDetailsRequest item)//, InvoiceDetails invoiceDetails)
        {
            var invoiceDetails = new InvoiceDetails();
            invoiceDetails.InvoiceId = invoice.InvoiceId;
            invoiceDetails.ItemId = item.ItemId;
            invoiceDetails.Price = item.Price;// roundNumbers.GetRoundNumber(item.Price, setDecimal);
            invoiceDetails.Quantity = item.Quantity;// roundNumbers.GetRoundNumber( item.Quantity ,setDecimal);
            invoiceDetails.TotalWithSplitedDiscount = item.TotalWithSplitedDiscount;
            invoiceDetails.TotalWithOutSplitedDiscount = item.Total;

            if (item.ItemTypeId == (int)ItemTypes.Note)
            {
                invoiceDetails.UnitId = null;
                invoiceDetails.Units = null;
            }
            else
                invoiceDetails.UnitId = item.UnitId;
            invoiceDetails.Signal = GeneralAPIsService.GetSignal(invoice.InvoiceTypeId);
            invoiceDetails.ConversionFactor = item.ConversionFactor;
            invoiceDetails.AutoDiscount = 0;// disconts of items   (later)
            invoiceDetails.AvgPrice = 0;// sales (later)
            invoiceDetails.Cost = 0;//calc (later)
            invoiceDetails.DiscountRatio =Math.Round( item.DiscountRatio,invoice.RoundNumber);
            invoiceDetails.DiscountValue = Math.Round(item.DiscountValue, invoice.RoundNumber);
            invoiceDetails.ItemTypeId = item.ItemTypeId;
            invoiceDetails.MinimumPrice = 0;//sales (later)
            invoiceDetails.PriceList = 0;//sales (later)
            invoiceDetails.ReturnQuantity = 0;//reurns (later)
            invoiceDetails.SplitedDiscountRatio = item.SplitedDiscountRatio;
            invoiceDetails.SplitedDiscountValue = item.SplitedDiscountValue;
            invoiceDetails.StatusOfTrans = item.TransStatus;//transfare (later)
            //invoiceDetails.TransQuantity = 0;//transfare (later)
            invoiceDetails.VatRatio = item.VatRatio;
            invoiceDetails.VatValue = item.VatValue;
            invoiceDetails.TransQuantity = item.TransQuantity;
            invoiceDetails.parentItemId = item.parentItemId;


            if (item.ItemTypeId == (int)ItemTypes.Expiary)
                invoiceDetails.ExpireDate = item.ExpireDate;
            else
                invoiceDetails.ExpireDate = null;

            invoiceDetails.balanceBarcode = item.balanceBarcode;
            invoiceDetails.isDiscountRatioItem = item.isDiscountRatioItem;
            invoiceDetails.VatValueForPrint= item.VatValueForPrint;
            return invoiceDetails;
        }

        public List<InvoiceDetailsRequest> setCompositItem(InvoiceMaster invoice, int itemId, int unitId, int indexOfItem, double qty)
        {
            var componentItems = new List<InvoiceDetailsRequest>();
            var itemData = itemCardPartsQuery.TableNoTracking.Where(a => a.ItemId == itemId);
            var componentItem = new InvoiceDetailsRequest();
            foreach (var item in itemData)
            {
                componentItem.ItemId = item.PartId;
                componentItem.UnitId = item.UnitId;
                componentItem.IndexOfItem = indexOfItem;
                componentItem.Quantity = qty * item.Quantity;
                componentItem.ItemTypeId = item.CardMaster.TypeId;
                componentItem.ItemCode = item.CardMaster.ItemCode;
                componentItem.ConversionFactor = item.CardMaster.Units.Where(a => a.UnitId == item.UnitId).First().ConversionFactor;
                //  var itemDetails = ItemDetails(invoice, componentItem);

                componentItems.Add(componentItem);
            }
            return componentItems;
        }


    }
}
