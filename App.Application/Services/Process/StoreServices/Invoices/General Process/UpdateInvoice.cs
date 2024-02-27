using App.Application.Handlers.GeneralAPIsHandler.PersonBalanceForPaymentMethod;
using App.Application.Handlers.Invoices.POS.UpdatePOSInvoice;
using App.Application.Helpers;
using App.Application.Helpers.Service_helper.FileHandler;
using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Application.Services.Process.Invoices.General_APIs;

using App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using App.Infrastructure.settings;
using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.Office2010.Excel;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.General_Process
{
    public class UpdateInvoice : BaseClass, IUpdateInvoice
    {
        private readonly IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositoryCommand;
        private readonly IRepositoryCommand<InvoiceFiles> InvoiceFilesRepositoryCommand;
        private readonly IRepositoryCommand<InvoicePaymentsMethods> PaymentsMethodsRepositoryCommand;
        private readonly IRepositoryCommand<InvPurchaseAdditionalCostsRelation> InvoiceAdditionalCostsRelationRepositoryCommand;
        private readonly IRepositoryCommand<InvoiceDetails> InvoiceDetailsRepositoryCommand;
        private readonly IRepositoryCommand<InvSerialTransaction> InvSerialTransactionRepositoryCommand;
        private readonly IRepositoryQuery<InvStpItemCardMaster> InvStpItemCardMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceFiles> InvoiceFilesRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvoicePaymentsMethods> InvoicePaymentsMethodsRepositoryQuery;
        private readonly IRepositoryQuery<InvPurchaseAdditionalCostsRelation> InvoiceAdditionalCostsRelationRepositoryQuery;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly IAddInvoice addInvoice;
        private readonly ISystemHistoryLogsService systemHistoryLogsService;
        private readonly iInvoicesIntegrationService _iInvoicesIntegrationService;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRepositoryCommand<InvSerialTransaction> SerialTransactionCommand;
        private readonly IFileHandler fileHandler;
        private readonly ICalculationSystemService CalcSystem;
        private readonly IFilesOfInvoices filesOfInvoices;
        private readonly IPaymentMethodsForInvoiceService paymentMethodsService;
        private readonly ISerialsService serialsService;
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvPersons> PersonRepositorQuery;
        private readonly IRepositoryQuery<GlReciepts> recieptQuery;
        private readonly IReceiptsFromInvoices ReceiptsInvoice;
        private readonly IRepositoryQuery<InvStoreBranch> invStoreBranchQuery;
        private readonly IRoundNumbers roundNumbers;
        private readonly IRedefineInvoiceRequestService redefineInvoiceRequestService;
        private readonly IPersonLastPriceService personLastPriceService;
        private readonly IUpdateItemsPrices updateItemsPricesService;
        private readonly IMediator _mediator;

        public UpdateInvoice(
                              IRepositoryCommand<InvoiceMaster> _InvoiceMasterRepositoryCommand,
                              IRepositoryCommand<InvoiceDetails> _InvoiceDetailsRepositoryCommand,
                              IRepositoryCommand<InvoiceFiles> _InvoiceFilesRepositoryCommand,
                              IRepositoryQuery<InvoiceDetails> _InvoiceDetailsRepositoryQuery,
                              IRepositoryQuery<InvoiceFiles> _InvoiceFilesRepositoryQuery,
                              IRepositoryCommand<InvoicePaymentsMethods> _PurchasePaymentsMethodsRepositoryCommand,
                              IRepositoryCommand<InvPurchaseAdditionalCostsRelation> _InvoiceAdditionalCostsRelationRepositoryCommand,
                              IRepositoryCommand<InvSerialTransaction> _InvSerialTransactionRepositoryCommand,
                              IRepositoryQuery<InvStpItemCardMaster> _InvStpItemCardMasterRepositoryQuery,
                              IHistoryInvoiceService _HistoryInvoiceService,
                              IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                              IAddInvoice generalProcess,
                              ISystemHistoryLogsService systemHistoryLogsService,
                              iInvoicesIntegrationService iInvoicesIntegrationService,
                              IGeneralAPIsService generalAPIsService,
                              IRepositoryQuery<InvoicePaymentsMethods> _InvoicePaymentsMethodsRepositoryQuery,
                              IRepositoryQuery<InvPurchaseAdditionalCostsRelation> _InvoiceAdditionalCostsRelationRepositoryQuery,
                               IHostingEnvironment _hostingEnvironment,
                               IRepositoryCommand<InvSerialTransaction> SerialTransactionCommand,
                               IFileHandler fileHandler, ICalculationSystemService CalcSystem,
                              IHttpContextAccessor _httpContext, IFilesOfInvoices filesOfInvoices,
                              IPaymentMethodsForInvoiceService paymentMethodsService, IRedefineInvoiceRequestService redefineInvoiceRequestService,
                              iUserInformation Userinformation, ISerialsService serialsService, IRepositoryQuery<InvPersons> personRepositorQuery
                            , IRepositoryQuery<GlReciepts> recieptQuery, IReceiptsFromInvoices ReceiptsInvoice,
                              IRepositoryQuery<InvStoreBranch> invStoreBranchQuery, IPersonLastPriceService personLastPriceService
            , IRoundNumbers roundNumbers, IUpdateItemsPrices updateItemsPricesService, IMediator mediator) : base(_httpContext)
        {
            InvoiceMasterRepositoryCommand = _InvoiceMasterRepositoryCommand;
            InvoiceDetailsRepositoryCommand = _InvoiceDetailsRepositoryCommand;
            PaymentsMethodsRepositoryCommand = _PurchasePaymentsMethodsRepositoryCommand;
            InvSerialTransactionRepositoryCommand = _InvSerialTransactionRepositoryCommand;
            InvStpItemCardMasterRepositoryQuery = _InvStpItemCardMasterRepositoryQuery;
            InvoiceAdditionalCostsRelationRepositoryCommand = _InvoiceAdditionalCostsRelationRepositoryCommand;
            httpContext = _httpContext;
            addInvoice = generalProcess;
            this.systemHistoryLogsService = systemHistoryLogsService;
            _iInvoicesIntegrationService = iInvoicesIntegrationService;
            InvoiceDetailsRepositoryQuery = _InvoiceDetailsRepositoryQuery;
            InvoicePaymentsMethodsRepositoryQuery = _InvoicePaymentsMethodsRepositoryQuery;
            InvoiceAdditionalCostsRelationRepositoryQuery = _InvoiceAdditionalCostsRelationRepositoryQuery;
            InvoiceFilesRepositoryQuery = _InvoiceFilesRepositoryQuery;
            HistoryInvoiceService = _HistoryInvoiceService;
            InvoiceFilesRepositoryCommand = _InvoiceFilesRepositoryCommand;
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            GeneralAPIsService = generalAPIsService;
            this._hostingEnvironment = _hostingEnvironment;
            this.SerialTransactionCommand = SerialTransactionCommand;
            this.fileHandler = fileHandler;
            this.CalcSystem = CalcSystem;
            this.filesOfInvoices = filesOfInvoices;
            this.paymentMethodsService = paymentMethodsService;
            this.serialsService = serialsService;
            this.Userinformation = Userinformation;
            PersonRepositorQuery = personRepositorQuery;
            this.recieptQuery = recieptQuery;
            this.ReceiptsInvoice = ReceiptsInvoice;
            this.invStoreBranchQuery = invStoreBranchQuery;
            this.roundNumbers = roundNumbers;
            this.redefineInvoiceRequestService = redefineInvoiceRequestService;
            this.personLastPriceService = personLastPriceService;
            this.updateItemsPricesService = updateItemsPricesService;
            _mediator = mediator;
        }

        public void setDefaultData(ref UpdateInvoiceMasterRequest parameter, int invoiceType, double TotalDiscountRatio)
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
            
            if (parameter.Notes == null)
                parameter.Notes = "";
            if (parameter.TotalDiscountRatio >= 100 || TotalDiscountRatio>=100)
            {
                parameter.Paid = 0;
                parameter.PaymentsMethods.RemoveAll(a => true);
            }


            //  double paid = 0;

            /*   if (parameter.isOtherPayment.Value && invoiceType == (int)DocumentType.POS && parameter.Paid > NetAfterRecalculate)
                   return new ResponseResult() { Result = Result.NotTotalPaid, ErrorMessageAr = ErrorMessagesAr.PaidExceedsTheNet, ErrorMessageEn = ErrorMessagesEn.PaidExceedsTheNet };
               //newV
               if (Lists.purchasesInvoicesList.Contains(invoiceType) || Lists.salesInvoicesList.Contains(invoiceType))
               {
                   if (parameter.Paid > NetAfterRecalculate && parameter.PaymentsMethods.Count() > 1)
                   {
                       //  paid = NetAfterRecalculate;
                       return new ResponseResult() { Result = Result.NotTotalPaid, ErrorMessageAr = ErrorMessagesAr.CantPaidExceedNetWithmoreThan1mwthod, ErrorMessageEn = ErrorMessagesEn.CantPaidExceedNetWithmoreThan1mwthod };
                   }
                   else if (parameter.Paid > NetAfterRecalculate && parameter.PaymentsMethods.Select(a => a.PaymentMethodId).Contains((int)PaymentMethod.PersonBalance))
                   {
                       if (invoiceType == (int)DocumentType.Sales)
                           return new ResponseResult() { Result = Result.NotTotalPaid, ErrorMessageAr = ErrorMessagesAr.CustomerBalanceGreaterThanNet, ErrorMessageEn = ErrorMessagesEn.CustomerBalanceGreaterThanNet };
                       else if (invoiceType == (int)DocumentType.Sales)
                           return new ResponseResult() { Result = Result.NotTotalPaid, ErrorMessageAr = ErrorMessagesAr.SupplierBalanceGreaterThanNet, ErrorMessageEn = ErrorMessagesEn.SupplierBalanceGreaterThanNet };

                   }
               }

               //else
               //{
               //    paid = parameter.Paid;
               //}


               return new ResponseResult() { Result = Result.Success};*/

        }

        public async Task<ResponseResult> UpdateInvoices(UpdateInvoiceMasterRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice, int invoiceTypeId, string InvoiceTypeName, string fileDirectory)
        {

            if (Lists.MainInvoiceForReturn.Contains(invoiceTypeId) && setting.ActiveElectronicInvoice)
            {
                return new ResponseResult { Result = Result.CanNotBeUpdated, ErrorMessageAr = ErrorMessagesAr.CantUpdateSystemInElectronicInvoice, ErrorMessageEn = ErrorMessagesEn.CantUpdateSystemInElectronicInvoice };
            }
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            

            if (Lists.POSInvoicesList.Where(x=> x != (int)DocumentType.ReturnPOS).Contains(invoiceTypeId))
            {
                //if (parameter.posSessionId != 0 && parameter.posSessionId != null)
                //    userInfo.POSSessionId = parameter.posSessionId;
                var session = GeneralAPIsService.userHasSession(userInfo.POSSessionId);
                if (!session.Item1)
                    return session.Item2;
            }
           


            var invoicesListWOtype = InvoiceMasterRepositoryQuery.TableNoTracking.Include(a => a.InvoicesDetails)//.ThenInclude(a => a.Items.Serials)
                  .Where(e => parameter.InvoiceId == e.InvoiceId).ToList();
            if (invoiceTypeId == (int)DocumentType.POS)
                parameter.StoreId = invoicesListWOtype.First().StoreId;
            var valid = GeneralAPIsService.ValidationOfInvoices(parameter, invoiceTypeId,setting, userInfo.CurrentbranchId,
                                 false,userInfo.userStors,invoicesListWOtype.First().InvoiceDate,true);
            if (valid.Result.Result != Result.Success)
                return valid.Result;

            var currentBranch = invStoreBranchQuery.TableNoTracking.Where(a => a.StoreId == parameter.StoreId).First().BranchId;

            //h


            var invoicePreventUpdated = invoicesListWOtype.Where(h => h.InvoiceTypeId == invoiceTypeId);
            if (invoicesListWOtype.Any() && invoicePreventUpdated.Count() <= 0)
            {
                return new ResponseResult { Result = Result.CanNotBeDeleted, ErrorMessageAr = " لا يمكن التعديل المستند نظرا لاختلاف ال API ", ErrorMessageEn = "you can not update this rec as  Api is different " };
            }

            if (userInfo.employeeId != invoicesListWOtype.First().EmployeeId && ( 
                ((invoiceTypeId == (int)DocumentType.Purchase || invoiceTypeId == (int)DocumentType.wov_purchase) && (!userInfo.otherSettings.purchasesShowOtherPersonsInv 
                   || (userInfo.otherSettings.purchasesShowOtherPersonsInv &&  !userInfo.otherSettings.purchasesEditOtherPersonsInv)) )
             || (invoiceTypeId == (int)DocumentType.Sales && (!userInfo.otherSettings.salesShowOtherPersonsInv
                   || (userInfo.otherSettings.salesShowOtherPersonsInv && !userInfo.otherSettings.salesEditOtherPersonsInv)))
             || (invoiceTypeId == (int)DocumentType.POS && (!userInfo.otherSettings.posShowOtherPersonsInv
                   || (userInfo.otherSettings.posShowOtherPersonsInv && !userInfo.otherSettings.posEditOtherPersonsInv)))))
            {
                return new ResponseResult { Result = Result.CanNotBeUpdated, ErrorMessageAr = " لا يمكن التعديل علي فواتير المستخدين الاخرين", ErrorMessageEn = "you can not update invoices of another users " };

            }
            // invoices that deleted before or returned or accredited > prevent deleting them again
            //var invoicePreventUpdated = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => a.InvoiceId== parameter.InvoiceId &&a.InvoiceTypeId==invoiceTypeId  
            //&& (a.IsDeleted || a.IsAccredite || a.IsReturn)).Select(a => new { a.InvoiceId, a.IsDeleted, a.IsAccredite, a.IsReturn ,a.transferStatus}).ToList();


            var reasonOfNotDeletedAr = "";
            var reasonOfNotDeletedEn = "";

            if (invoicePreventUpdated.Count()>0)
            {
               
                if (invoicePreventUpdated.First().IsAccredite)
                {
                    reasonOfNotDeletedAr = ErrorMessagesAr.CantUpdateInvAccredited;
                    reasonOfNotDeletedEn = ErrorMessagesEn.CantUpdateInvAccredited;
                }
                else if (invoicePreventUpdated.First().IsDeleted)
                {
                    reasonOfNotDeletedAr = ErrorMessagesAr.CantUpdateInvDeleted;
                    reasonOfNotDeletedEn = ErrorMessagesEn.CantUpdateInvDeleted;
                }
                else if (invoicePreventUpdated.First().IsReturn)
                {
                    reasonOfNotDeletedAr = ErrorMessagesAr.CantUpdateInvReturned;
                    reasonOfNotDeletedEn = ErrorMessagesEn.CantUpdateInvReturned;
                }
                else if (invoicePreventUpdated.First().transferStatus != Aliases.TransferStatus.Binded && Lists.transferStore.Contains(invoicePreventUpdated.First().InvoiceTypeId))
                {
                    return new ResponseResult() { Result = Result.CanNotBeUpdated, ErrorMessageAr = "لا يمكن التعديل على المستند نظراً لاستلامه ", ErrorMessageEn = "Can not edit in this document due to its receipt" };
                }
                else if (invoicePreventUpdated.First().IsCollectionReceipt)
                {
                    reasonOfNotDeletedAr = ErrorMessagesAr.CantUpdateInvCollectionReceipt;
                    reasonOfNotDeletedEn = ErrorMessagesEn.CantUpdateInvCollectionReceipt;
                }
                if (!string.IsNullOrEmpty(reasonOfNotDeletedAr))
                    return new ResponseResult()
                {
                    Result = Result.CanNotBeUpdated,
                    ErrorMessageAr = reasonOfNotDeletedAr,
                    ErrorMessageEn = reasonOfNotDeletedEn
                };
            }

            //var dataFaild = await redefineInvoiceRequestService.setInvoiceRequest(parameter, setting, invoiceTypeId);
            //if (dataFaild.Item2 != "")
            //    return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = dataFaild.Item2, ErrorMessageEn = dataFaild.Item3 };


            // redefine Invoice Request to avoid any error eccured while creating invoice
            List<int> FileId = parameter.FileId;
            int InvoiceId = parameter.InvoiceId;
            
            var redefineInvoiceRequest = await redefineInvoiceRequestService.setInvoiceRequest(parameter, setting, invoiceTypeId,userInfo,"", invoicesListWOtype.First().PriceListId,0);
            if (redefineInvoiceRequest.Item2 != "")
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = redefineInvoiceRequest.Item2, ErrorMessageEn = redefineInvoiceRequest.Item3 };
            parameter = Mapping.Mapper.Map<InvoiceMasterRequest,UpdateInvoiceMasterRequest>(redefineInvoiceRequest.Item1);
            parameter.InvoiceId = InvoiceId;
            parameter.FileId = FileId;


            

            //parameter.BranchId = 4;
            InvoiceMasterRepositoryCommand.StartTransaction();
            try
            {
                var signal = GeneralAPIsService.GetSignal(invoiceTypeId);

                var invoice = await InvoiceMasterRepositoryQuery.GetByAsync(q => q.InvoiceId == parameter.InvoiceId);
                if(invoice.BranchId != currentBranch)
                {
                    return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "لا يمكن تغيير المخزن. المخزن تابع لفرع اخر " };
                }
                if(parameter.isExpenses.Value && !invoice.isExpenses)
                {
                    return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "لا يمكن التحويل من مشتريات الى مصروفات " ,
                             ErrorMessageEn = "Can not convert from purchases to expenses"};

                }

                parameter.InvoiceDate = parameter.InvoiceDate.Date == invoice.InvoiceDate.Date ? invoice.InvoiceDate : GeneralAPIsService.serverDate(parameter.InvoiceDate);

                invoice.InvoiceType = userInfo.CurrentbranchCode.ToString() + "-" + InvoiceTypeName + "-" + invoice.Code;

               if(Lists.InvoicesTypesOfExtractFromStore.Contains(invoiceTypeId))
                {
                    var quantityAvailable = await GeneralAPIsService.checkQuantityBeforeSaveInvoiceForExtract(parameter.InvoiceDetails, parameter.StoreId, parameter.InvoiceDate, parameter.InvoiceId, setting, invoiceTypeId, signal,invoice.StoreId,false,parameter.ParentInvoiceCode);
                    if (!quantityAvailable.Item1)
                    {
                        return new ResponseResult()
                        { Result = Result.QuantityNotavailable, ErrorMessageAr = quantityAvailable.Item2, ErrorMessageEn = quantityAvailable.Item3 };
                    }
                }
                else if (Lists.InvoicesTypeOfAddingToStore.Contains(invoiceTypeId))
                {
                    var quantityAvailable = await GeneralAPIsService.checkQuantityBeforeSaveInvoiceForAdd(parameter.InvoiceDetails, invoice.StoreId, parameter.InvoiceDate, parameter.InvoiceId, setting, invoiceTypeId, signal,parameter.StoreId,false);
                    if (!quantityAvailable.Item1) 
                        return new ResponseResult()
                          { Result = Result.QuantityNotavailable, ErrorMessageAr =  quantityAvailable.Item2 , ErrorMessageEn =  quantityAvailable.Item3};

                }


                var serialExist = serialsService.checkSerialBeforeSave(true, invoice.InvoiceType, parameter.InvoiceDetails, invoiceTypeId,parameter.StoreId,invoice.StoreId);
                // check serials 
                if (serialExist.Result.Result != Result.Success)
                    return new ResponseResult() { Data = serialExist.Result.Data, Id = null,Result = serialExist.Result.Result,
                        Note = serialExist.Result.ErrorMessageEn, ErrorMessageAr = serialExist.Result.ErrorMessageAr, ErrorMessageEn = serialExist.Result.ErrorMessageEn };


                //setting validation
                if (invoiceTypeId == (int)DocumentType.Purchase || invoiceTypeId == (int)DocumentType.ReturnPurchase
                    || invoiceTypeId == (int)DocumentType.wov_purchase || invoiceTypeId == (int)DocumentType.ReturnWov_purchase)
                {
              
                    if (setting.Purchases_PayTotalNet == true)
                    {
                      //if (!userInfo.otherSettings.purchasesAllowCreditSales)
                        if (parameter.Paid != parameter.Net)
                        {
                            return new ResponseResult() { Data = null, Id = null, Result = Result.NotTotalPaid, Note = "you should send total net equal to total payments" 
                            ,ErrorMessageAr=ErrorMessagesAr.PayTotalNet,ErrorMessageEn=ErrorMessagesEn.PayTotalNet};
                        }
                    }
                }

              

                parameter.SalesManId = parameter.SalesManId == 0 ? parameter.SalesManId = null : parameter.SalesManId;

                // recalculate results to avoid any changings in data from user
                CalculationOfInvoiceParameter calculationOfInvoiceParameter = new CalculationOfInvoiceParameter()
                {
                    DiscountType = parameter.DiscountType,
                    InvoiceTypeId = invoiceTypeId,
                    // ParentInvoice = parameter.ParentInvoiceCode,
                    TotalDiscountRatio = parameter.TotalDiscountRatio,
                    TotalDiscountValue = parameter.TotalDiscountValue,
                    PersonId=parameter.PersonId,
                    InvoiceId=parameter.InvoiceId,
                    isDiscountRatio = parameter.isDiscountRatio
                }; 
                Mapping.Mapper.Map<List<InvoiceDetailsRequest>, List<InvoiceDetailsAttributes>>(parameter.InvoiceDetails, calculationOfInvoiceParameter.itemDetails);
                var recalculate = await CalcSystem.StartCalculation(calculationOfInvoiceParameter);
               
                int count = 0; 
                foreach (var item in parameter.InvoiceDetails)//.Where(a => a.parentItemId == null||a.parentItemId==0).ToList())
                {
                    item.Price = recalculate.itemsTotalList[count].Price;
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
                // set default data
              //  double paid = 0;

                 setDefaultData(ref parameter, invoiceTypeId, recalculate.TotalDiscountRatio);
                //check auto extract person balance


                // check if paid is equal in edit with old invoice
                bool isEqualPaidInUpdate = false;
               
                if ((invoiceTypeId == (int)DocumentType.Sales || invoiceTypeId == (int)DocumentType.Purchase || invoiceTypeId == (int)DocumentType.wov_purchase) && setting.other_AutoExtractFromCustomerSupplierBalance)
                {
                   

                    // check if paid is equal in edit with old invoice
                    bool IsRemainChangedForUpdate = false;
                    if (parameter.PersonId.Value == invoice.PersonId.Value)
                    {
                        
                        isEqualPaidInUpdate = true;
                        if (parameter.Remain != invoice.Remain)
                            IsRemainChangedForUpdate = true;

                    }
                    
                    else
                    {
                        
                        if (parameter.PaymentsMethods.Count() > 0 && parameter.PaymentsMethods.Select(a => a.PaymentMethodId).Contains((int)PaymentMethod.PersonBalance)) 
                            {

                                //if(parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).ToList().Count()>0 )
                                if(parameter.TotalDiscountValue==0&& parameter.TotalDiscountRatio==0)
                                parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).First().Value += parameter.Remain;

                            }

                        if(parameter.PaymentsMethods.Count() > 0 && parameter.PaymentsMethods.Where(a=>a.Value==0).FirstOrDefault() ==null&&parameter.TotalDiscountValue==0&& parameter.TotalDiscountRatio==0)
                        parameter.Paid += parameter.Remain;
                    }

                    parameter.PaymentsMethods = await paymentMethodsService.AutoExtractFromPersonBalanceValidation(parameter, invoiceTypeId, recalculate.Net, setting.other_AutoExtractFromCustomerSupplierBalance, isEqualPaidInUpdate, IsRemainChangedForUpdate);
 
                }

                var paidExceedNet = await paymentMethodsService.PaymentValidation( parameter, invoiceTypeId, recalculate.Net, userInfo.CurrentbranchId,parameter.InvoiceId, isEqualPaidInUpdate);

                if (paidExceedNet.Result != Result.Success)
                    return paidExceedNet;
                //  paid = parameter.Paid;

                invoice.isDiscountRatio = parameter.isDiscountRatio;

                //invoice.EmployeeId = userInfo.employeeId;
                invoice.POSSessionId = userInfo.POSSessionId;
                invoice.BranchId = currentBranch;
                invoice.Code = invoice.Code;
                invoice.InvoiceTypeId = invoiceTypeId;
                invoice.BookIndex = (parameter.BookIndex != null ? parameter.BookIndex.Trim() : parameter.BookIndex);
                invoice.InvoiceDate = parameter.InvoiceDate;
                invoice.StoreId = parameter.StoreId;
                invoice.StoreIdTo = parameter.StoreIdTo;

                invoice.PersonId = parameter.PersonId;
                invoice.Notes = (parameter.Notes != null ? parameter.Notes.Trim() : parameter.Notes);
                invoice.DiscountType = parameter.DiscountType;
                invoice.SalesManId = parameter.SalesManId;
                invoice.Net = recalculate.Net;// parameter.Net;
                if (SettingsOfInvoice != null)
                {
                    invoice.ActiveDiscount = SettingsOfInvoice.ActiveDiscount;
                    invoice.ApplyVat = invoice.ApplyVat;
                    invoice.PriceWithVat = invoice.PriceWithVat;
                }

                invoice.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
                invoice.ParentInvoiceCode = invoice.InvoiceType;
                invoice.Paid = parameter.Paid;
                invoice.VirualPaid = parameter.VirualPaid;//parameter.Paid;
                invoice.RoundNumber = (setting.Other_Decimals > invoice.RoundNumber ? setting.Other_Decimals : invoice.RoundNumber);

                invoice.Remain = roundNumbers.GetRoundNumber(recalculate.Net- parameter.Paid, invoice.RoundNumber);
                invoice.TotalDiscountRatio = recalculate.TotalDiscountRatio; // parameter.TotalDiscountRatio;
                invoice.TotalVat = recalculate.TotalVat;// parameter.TotalVat;
                invoice.TotalDiscountValue = recalculate.TotalDiscountValue;// parameter.TotalDiscountValue;
                invoice.TotalPrice = recalculate.TotalPrice;// parameter.TotalPrice;
                invoice.TotalAfterDiscount = recalculate.TotalAfterDiscount;// parameter.TotalAfterDiscount;
               // invoice.Serialize = invoice.Code;
                invoice.IsDeleted = false;
                invoice.ActualNet = recalculate.ActualNet;
                invoice.isExpenses = parameter.isExpenses.Value;
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
                if (Lists.salesInvoicesList.Contains(invoiceTypeId) || Lists.POSInvoicesList.Contains(invoiceTypeId))
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
                    else if (Lists.POSInvoicesList.Contains(invoiceTypeId) && !setting.Pos_DeferredSale)
                    {
                        if (  parameter.Paid < invoice.Net)
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
                        if (invoice.PaymentType != (int)PaymentType.Complete)
                        {
                           // var CreditLimit = PersonRepositorQuery.TableNoTracking.Where(a => a.Id == parameter.PersonId)
                           //.Select(a => a.CreditLimit).ToList();
                           // if (CreditLimit.First() != null)
                           // {
                           //     var debitValueForCustomer = recieptQuery.TableNoTracking.Where(a => a.ParentId!=invoice.InvoiceId && a.PersonId == parameter.PersonId).Sum(a => a.Debtor - a.Creditor);
                           //     debitValueForCustomer += invoice.Remain;
                           //     if (debitValueForCustomer > CreditLimit.First())
                           //         return new ResponseResult() { Data = null, Id = null, Result = Result.NotTotalPaid, Note = "You have exceeded your credit limit", ErrorMessageAr = ErrorMessagesAr.CreditLimit, ErrorMessageEn = ErrorMessagesEn.CreditLimit };
                           // }

                           
                            if (Credit.Count() > 0)
                            {
                                if (Credit.First().CreditLimit != null)
                                {
                                    var debitValueForCustomer = recieptQuery.TableNoTracking.Where(a => a.BranchId == userInfo.CurrentbranchId && a.ParentId != invoice.InvoiceId && a.PersonId == parameter.PersonId).Sum(a => a.Debtor - a.Creditor);

                                    debitValueForCustomer += invoice.Remain;

                                    if (debitValueForCustomer > Credit.First().CreditLimit)
                                        return new ResponseResult() { Data = null, Id = null, Result = Result.NotTotalPaid,
                                            Note = "You have exceeded your credit limit",
                                            ErrorMessageAr = string.Concat(ErrorMessagesAr.CreditLimit, " ", Credit.First().CreditLimit.Value ," ريال "), 
                                            ErrorMessageEn = string.Concat(ErrorMessagesEn.CreditLimit, " ", Credit.First().CreditLimit.Value, " SAR ")
                                        };

                                }

                            }
                        }
                    }

                    if (invoice.PaymentType != (int)PaymentType.Complete && Credit.Count() > 0 && (invoiceTypeId == (int)DocumentType.Sales  || invoiceTypeId == (int)DocumentType.POS))
                    {
                        if (Credit.First().CreditPeriod != null)
                        {
                            var FirstInvoice = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => a.BranchId == userInfo.CurrentbranchId
                                     && a.InvoiceId != invoice.InvoiceId
                                     && (a.InvoiceTypeId == (int)DocumentType.Sales|| a.InvoiceTypeId == (int)DocumentType.POS )
                                     && (a.IsReturn?  a.InvoiceSubTypesId == (int)SubType.PartialReturn : true)
                                      && !a.IsDeleted  && a.PersonId == parameter.PersonId && a.Paid < a.Net)
                                                   .OrderBy(a => a.InvoiceDate);
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
                                        Note = "You have exceeded your credit period"
                                        ,
                                        ErrorMessageAr = string.Concat(ErrorMessagesAr.CreditPeriod, " ", Credit.First().CreditPeriod.Value, " يوم "),
                                        ErrorMessageEn = string.Concat(ErrorMessagesEn.CreditPeriod, " ", Credit.First().CreditPeriod.Value, " day ")
                                    };
                                }
                            }

                        }
                    }
                        
                }
              
                if (!Lists.storesInvoicesList.Contains(invoiceTypeId))
                    if ((parameter.Paid > 0 && parameter.PaymentsMethods.Count() == 0) || parameter.PaymentsMethods.Select(a => a.Value).Sum() != invoice.Paid)
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
                 

                invoice.ParentInvoiceCode = invoice.InvoiceType;
                //invoice.InvoiceSubTypesId = (!parameter.isExpenses.Value ? 0 :
                //                          (invoiceTypeId == (int)DocumentType.Purchase ? (int)SubType.Expenses :
                //                            (invoiceTypeId == (int)DocumentType.ReturnPurchase ? (int)SubType.ReturnExpenses : 0)));


                var saved = await InvoiceMasterRepositoryCommand.UpdateAsyn(invoice);
                if (!saved)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice master" };

                // Delete Other Addition List 
                if (Lists.purchasesWithOutDeleteInvoicesList.Contains(invoiceTypeId))
                {
                    var invooiceOtherAdditionList = InvoiceAdditionalCostsRelationRepositoryQuery.FindAll(q => q.InvoiceId == parameter.InvoiceId).ToList();

                    if (invooiceOtherAdditionList.Count > 0)
                        InvoiceAdditionalCostsRelationRepositoryCommand.RemoveRange(invooiceOtherAdditionList);

                    //Add Purchase additional costs List .
                    if (parameter.OtherAdditionList != null)
                    {
                        if (parameter.OtherAdditionList.Count() > 0)
                        {
                            var InvoiceAdditionalCostList = new List<InvPurchaseAdditionalCostsRelation>();
                            double totalOfAdditions = 0;

                            foreach (var item in parameter.OtherAdditionList)
                            {
                                var InvoiceAdditionalCost = new InvPurchaseAdditionalCostsRelation();
                                InvoiceAdditionalCost.InvoiceId = invoice.InvoiceId;
                                InvoiceAdditionalCost.AddtionalCostId = item.AddtionalCostId;
                                InvoiceAdditionalCost.Amount = item.Amount;
                                if (item.AdditionsType == (int)PurchasesAdditionalType.RatioOfInvoiceTotal)
                                    InvoiceAdditionalCost.Total =Math.Round( (item.Amount / 100) * invoice.TotalPrice,invoice.RoundNumber);
                                else if (item.AdditionsType == (int)PurchasesAdditionalType.RatioOfInvoiceNet)
                                    InvoiceAdditionalCost.Total = Math.Round((item.Amount / 100) * invoice.Net,invoice.RoundNumber);
                                else
                                    InvoiceAdditionalCost.Total = Math.Round(item.Amount,invoice.RoundNumber);

                                totalOfAdditions += InvoiceAdditionalCost.Total;

                                InvoiceAdditionalCostList.Add(InvoiceAdditionalCost);
                            }
                            invoice.TotalOtherAdditions = Math.Round(totalOfAdditions,invoice.RoundNumber);

                            InvoiceAdditionalCostsRelationRepositoryCommand.AddRange(InvoiceAdditionalCostList);
                            await InvoiceAdditionalCostsRelationRepositoryCommand.SaveAsync();
                            var currentInvoiceMaster = await InvoiceMasterRepositoryQuery.GetByAsync(a => a.InvoiceId == invoice.InvoiceId);
                            currentInvoiceMaster.TotalOtherAdditions = Math.Round(totalOfAdditions,invoice.RoundNumber);
                            await InvoiceMasterRepositoryCommand.UpdateAsyn(currentInvoiceMaster);
                        }

                    }

                }


                // Delete Details 
                var invoiceDetails = InvoiceDetailsRepositoryQuery.FindAll(q => q.InvoiceId == parameter.InvoiceId).ToList();
               
                // save details of items in the invoice
                var invoiceDetailsList = new List<InvoiceDetails>();
                if (parameter.InvoiceDetails != null)
                {
                    if (parameter.InvoiceDetails.Count() > 0)
                    {
                        
                        if (((invoiceTypeId != (int)DocumentType.POS && setting.Other_MergeItems == true)|| (invoiceTypeId == (int)DocumentType.POS&&  setting.Other_MergeItems && setting.otherMergeItemMethod == "withSave")) || invoiceTypeId == (int)DocumentType.OutgoingTransfer) //setting with marge
                        {
                            var invoiceDetailsListMerg = await GeneralAPIsService.MergeItems(parameter.InvoiceDetails.ToList(),invoiceTypeId);
                            parameter.InvoiceDetails = invoiceDetailsListMerg;
                        }

                        if (invoice.InvoiceId != 0)
                        {
                            // will refactored
                         //   await SerialTransactionCommand.DeleteAsync(a => a.InvoiceId == invoice.InvoiceId);
                          //  SerialTransactionCommand.SaveAsync();
                        }
                        int index = 0;
                        if (invoiceTypeId == (int)DocumentType.OutgoingTransfer)
                            parameter.InvoiceDetails.Select(a => a.TransStatus = invoice.transferStatus).ToList();
                        foreach (var item in parameter.InvoiceDetails)
                        {
                            index++;
                            var invoiceDetails_ = new InvoiceDetails();
                            invoiceDetails_ =  addInvoice.ItemDetails(invoice, item);
                            if(item.parentItemId!=null && item.parentItemId!=0)
                            {
                                item.IndexOfItem = 0;
                                invoiceDetails_.indexOfItem = 0;
                            }
                            else
                            {
                                item.IndexOfItem = index;
                                invoiceDetails_.indexOfItem =index;
                            }

                            // other additions 
                            if (parameter.OtherAdditionList.Count() > 0)
                            {
                                invoiceDetails_.OtherAdditions = Math.Round((((item.Quantity * item.Price) / invoice.TotalPrice) * invoice.TotalOtherAdditions), 10);
                            }

                            invoiceDetailsList.Add(invoiceDetails_);
                            //if (item.ItemTypeId == (int)ItemTypes.Composite)
                            //{
                            //    var componnentITems =addInvoice.setCompositItem(invoice, item.ItemId, item.UnitId.Value, index, item.Quantity);
                            //    invoiceDetailsList.AddRange(componnentITems);

                            //}
                        }

                      

                        // Saving items in edited items entity for profit
                      saved = await   GeneralAPIsService.generateEditedItems(invoiceDetailsList, invoice.Serialize ,true,invoice.InvoiceId,invoice.BranchId);


                         InvoiceDetailsRepositoryCommand.AddRange(invoiceDetailsList);
          //      saved = await InvoiceDetailsRepositoryCommand.SaveAsync();   
                  
                              InvoiceDetailsRepositoryCommand.RemoveRange(invoiceDetails);
                              saved= await   InvoiceDetailsRepositoryCommand.SaveAsync();
                      
                   
                        if (!saved)
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice details" };

                        // serials
                        var itemsWithSerialsFromRequest = parameter.InvoiceDetails.Where(a => a.ItemTypeId == (int)ItemTypes.Serial).ToList();
                      
                        if (Lists.InvoicesTypeOfAddingToStore.Contains(invoice.InvoiceTypeId))
                        {
                              var res=  await serialsService.AddSerialsForAddedInvoice(invoice.InvoiceType, itemsWithSerialsFromRequest, null, parameter.StoreId, invoice.InvoiceTypeId,0);
                            if (!string.IsNullOrEmpty(res))
                                return new ResponseResult { Result = Result.Failed, ErrorMessageAr = res, ErrorMessageEn = res };

                        }
                        else if (Lists.InvoicesTypesOfExtractFromStore.Contains(invoice.InvoiceTypeId))
                            {
                              var res = await  serialsService.AddSerialsForExtractInvoice(true , itemsWithSerialsFromRequest, invoice.InvoiceType,invoice.transferStatus);
                                if (!string.IsNullOrEmpty(res.Item2))
                                    return new ResponseResult { Result = Result.Failed, ErrorMessageAr = res.Item2, ErrorMessageEn = res.Item3 };
                            }
                    }
                }


                
                // delete old files and add new
               
               saved =await filesOfInvoices.saveFilesOfInvoices(parameter.AttachedFile, currentBranch, fileDirectory, invoice.InvoiceId,true,parameter.FileId,false);

               if (!saved)
                  return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice files" };


                //Add invoice Payment List
              var SavePaymentMethods_ = await  paymentMethodsService.SavePaymentMethods(invoiceTypeId, parameter.PaymentsMethods, invoice.InvoiceId, invoice.BranchId, invoice.Paid,true, invoice.RoundNumber);
                if(!SavePaymentMethods_.Item1)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild in payment methods" , ErrorMessageAr= SavePaymentMethods_ .Item2,ErrorMessageEn= SavePaymentMethods_ .Item3};


         

                // to update TotalPrice after calculations
                await InvoiceMasterRepositoryCommand.UpdateAsyn(invoice);
                HistoryInvoiceService.HistoryInvoiceMaster(invoice.BranchId, invoice.Notes, invoice.BrowserName, lastTransactionAction: "U", null, invoice.BookIndex, invoice.Code
              , invoice.InvoiceDate, invoice.InvoiceId, invoice.InvoiceType, invoice.InvoiceTypeId,(int)SubType.Nothing, invoice.IsDeleted, invoice.ParentInvoiceCode, invoice.Serialize, invoice.StoreId, invoice.TotalPrice);

               // InvoiceMasterRepositoryCommand.CommitTransaction();

                if(Lists.purchasesInvoicesList.Contains(invoiceTypeId)|| Lists.salesInvoicesList.Contains(invoiceTypeId) || Lists.POSInvoicesList.Contains(invoiceTypeId) || Lists.storesInvoicesList.Where( x => !Lists.InvoiceCannotAddJournalEntery.Contains(x)).Contains(invoiceTypeId))
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
                        DocType = (DocumentType)invoice.InvoiceTypeId,
                        isAllowedVAT = invoice.ApplyVat,
                        isUpdate = true,
                        invDate=invoice.InvoiceDate,
                        branchId = invoice.BranchId,
                        note = invoice.Notes

                    });
                    if (setJournalEntry.Result == Result.Failed)
                    {
                        return new ResponseResult { Result = Result.Failed, Note = "Faild in Journal Entry" };
                    }
                    //else
                    //{
                    //    IRepositoryActionResult res = (IRepositoryActionResult)setJournalEntry.Data;
                    //    if (res.Status != RepositoryActionStatus.Updated)
                    //        return new ResponseResult { Result = Result.Failed, Note = "Faild in Journal Entry" };
                    //}

                    if (! Lists.storesInvoicesList.Contains(invoiceTypeId))
                    {
                        var setReceipt = await ReceiptsInvoice.updateReceiptsFromInvoices(invoice, invoice.InvoicePaymentsMethods != null ? invoice.InvoicePaymentsMethods.ToList() : null);
                        if (setReceipt.Result == Result.Failed)
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in Receipts" };

                    }

                }
                InvoiceMasterRepositoryCommand.CommitTransaction();

                SystemActionEnum systemActionEnum = new SystemActionEnum();
                if (invoice.InvoiceTypeId == (int)DocumentType.Sales)
                    systemActionEnum = SystemActionEnum.editSalesInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.Purchase)
                    systemActionEnum = SystemActionEnum.editPurchaseInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.ReturnSales)
                    systemActionEnum = SystemActionEnum.editReturnSalesInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.ReturnPurchase)
                    systemActionEnum = SystemActionEnum.editReturnPurchaseInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.ReturnPurchase)
                    systemActionEnum = SystemActionEnum.editReturnPurchaseInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.AddPermission)
                    systemActionEnum = SystemActionEnum.editAddPermisson;
                else if (invoice.InvoiceTypeId == (int)DocumentType.itemsFund)
                    systemActionEnum = SystemActionEnum.editItemsEntryFund;
                else if (invoice.InvoiceTypeId == (int)DocumentType.POS)
                    systemActionEnum = SystemActionEnum.editPOSInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.ExtractPermission)
                    systemActionEnum = SystemActionEnum.editExtractPermission;
                else if (invoice.InvoiceTypeId == (int)DocumentType.OutgoingTransfer)
                    systemActionEnum = SystemActionEnum.OutGoingTransferInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.IncomingTransfer)
                    systemActionEnum = SystemActionEnum.IncomingTranferInvoice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.OfferPrice)
                    systemActionEnum = SystemActionEnum.editOfferPrice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.wov_purchase)
                    systemActionEnum = SystemActionEnum.addWOVPurchase;
                else if (invoice.InvoiceTypeId == (int)DocumentType.ReturnWov_purchase)
                    systemActionEnum = SystemActionEnum.addReturnWOVPurchase;

                if (systemActionEnum != null)
                    await systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);

                if (invoice.InvoiceTypeId == (int)DocumentType.POS || invoice.InvoiceTypeId == (int)DocumentType.Sales||
                    invoice.InvoiceTypeId == (int)DocumentType.Purchase || invoice.InvoiceTypeId == (int)DocumentType.wov_purchase)
                 {
                    var personLastPriceRequest = new PersonLastPriceRequest()
                    {
                        personId = parameter.PersonId.Value,
                        invoiceDetails = parameter.InvoiceDetails,
                        invoiceTypeId = invoiceTypeId
                    };
                    //BackgroundJob.Enqueue(() => personLastPriceService.setLastPrice(personLastPriceRequest).Wait());
                    await personLastPriceService.setLastPrice(personLastPriceRequest);
                    if (invoice.InvoiceTypeId == (int)DocumentType.Purchase && setting.Purchase_UpdateItemsPricesAfterInvoice)
                        await Task.Run(() => updateItemsPricesService.setItemsPrices(invoiceDetailsList));

                }

                return new ResponseResult() {permissionListId = userInfo.permissionListId, Data = null, Id = parameter.InvoiceId,Code= invoice.Code, Result = Result.Success   };
            }
            catch (Exception e )
            {
                return new ResponseResult { Data = e };
            }
           
        }


    }
}
