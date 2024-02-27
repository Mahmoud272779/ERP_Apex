using App.Application.Helpers;
using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.Invoices;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.Payment_methods;
using App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process
{
    public class  UpdateTempInvoiceService : BaseClass, IUpdateTempInvoiceService
    {
         private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IHttpContextAccessor httpContext; 
        private SettingsOfInvoice SettingsOfInvoice;
        private readonly iUserInformation iUserInformation;
        private readonly IRepositoryQuery<InvStoreBranch> SotreQuery;
        private readonly IRepositoryCommand<OfferPriceMaster> OfferPriceMasterCommand;
        private readonly IRepositoryQuery<OfferPriceMaster> OfferPriceMasterQuery;
        private readonly IRepositoryQuery<OfferPriceDetails> OfferPriceDetailsQuery;
        private readonly IRepositoryCommand<OfferPriceDetails> OfferPriceDetailsCommand;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IRedefineInvoiceRequestService redefineInvoiceRequestService;
        private readonly ICalculationSystemService CalcSystem;
        private readonly IRoundNumbers roundNumbers;
        private readonly IAddTempInvoiceService addTempInvoice;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly ISystemHistoryLogsService systemHistoryLogsService;

        public UpdateTempInvoiceService(IRepositoryQuery<InvGeneralSettings> generalSettings,
                              IHttpContextAccessor _httpContext,
                              iUserInformation iUserInformation,
                              IRepositoryQuery<InvStoreBranch> sotreQuery,
                              IRepositoryCommand<OfferPriceMaster> offerPriceMasterCommand,
                              IRepositoryQuery<OfferPriceMaster> offerPriceMasterQuery,
                              IRepositoryQuery<OfferPriceDetails> offerPriceDetailsQuery,
                              IRepositoryCommand<OfferPriceDetails> offerPriceDetailsCommand,
                              IGeneralAPIsService generalAPIsService,
                              IRedefineInvoiceRequestService redefineInvoiceRequestService,
                              ICalculationSystemService calcSystem,
                              IRoundNumbers roundNumbers,
                             IAddTempInvoiceService addTempInvoice,
                              IHistoryInvoiceService historyInvoiceService,
                              ISystemHistoryLogsService systemHistoryLogsService) : base(_httpContext)
        {
            httpContext = _httpContext;
            GeneralSettings = generalSettings;
            this.iUserInformation = iUserInformation;
            SotreQuery = sotreQuery;
            OfferPriceMasterCommand = offerPriceMasterCommand;
            OfferPriceMasterQuery = offerPriceMasterQuery;
            OfferPriceDetailsQuery = offerPriceDetailsQuery;
            OfferPriceDetailsCommand = offerPriceDetailsCommand;
            GeneralAPIsService = generalAPIsService;
            this.redefineInvoiceRequestService = redefineInvoiceRequestService;
            CalcSystem = calcSystem;
            this.roundNumbers = roundNumbers;
            this.addTempInvoice = addTempInvoice;
            HistoryInvoiceService = historyInvoiceService;
            this.systemHistoryLogsService = systemHistoryLogsService;
        }

        public async Task<ResponseResult> UpdateTempInvoice(UpdateInvoiceMasterRequest parameter, int invoiceTypeId,string invoiceType)
        {
            if (parameter.InvoiceId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var setting = await GeneralSettings.SingleOrDefault(a => a.Id == 1);


            // validation of total result
            SettingsOfInvoice = new SettingsOfInvoice();

            if(invoiceTypeId==(int)DocumentType.OfferPrice)
               SettingsOfInvoice.ActiveDiscount = setting.Sales_ActiveDiscount;
            else if (invoiceTypeId == (int)DocumentType.PurchaseOrder)
                SettingsOfInvoice.ActiveDiscount = setting.Purchases_ActiveDiscount;

            UserInformationModel userInfo = await iUserInformation.GetUserInformation();

            var currentBranch = SotreQuery.TableNoTracking.Where(a => a.StoreId == parameter.StoreId).First().BranchId;

            var invoicesListWOtype = OfferPriceMasterQuery.TableNoTracking.Include(a => a.OfferPriceDetails)//.ThenInclude(a => a.Items.Serials)
                  .Where(e => parameter.InvoiceId == e.InvoiceId).ToList();

            var valid = GeneralAPIsService.ValidationOfInvoices(parameter, invoiceTypeId, setting, userInfo.CurrentbranchId,
                                 false, userInfo.userStors, invoicesListWOtype.First().InvoiceDate,true);
            if (valid.Result.Result != Result.Success)
                return valid.Result;

            var invoicePreventUpdated = invoicesListWOtype.Where(h => h.InvoiceTypeId == invoiceTypeId);
            if (invoicesListWOtype.Any() && invoicePreventUpdated.Count() <= 0)
            {
                return new ResponseResult { Result = Result.CanNotBeDeleted, ErrorMessageAr = " لا يمكن التعديل المستند نظرا لاختلاف ال API ", ErrorMessageEn = "you can not update this rec as  Api is different " };
            }

            var reasonOfNotDeletedAr = "";
            var reasonOfNotDeletedEn = "";

            if (invoicePreventUpdated.Count() > 0)
            {
                if (invoicePreventUpdated.First().InvoiceSubTypesId == (int)SubType.OfferPriceAccridited)
                {
                    reasonOfNotDeletedAr = ErrorMessagesAr.CantUpdateInvAccredited;
                    reasonOfNotDeletedEn = ErrorMessagesEn.CantUpdateInvAccredited;
                }
                else if (invoicePreventUpdated.First().InvoiceSubTypesId==(int)SubType.OfferPriceDeleted)
                {
                    reasonOfNotDeletedAr = ErrorMessagesAr.CantUpdateInvDeleted;
                    reasonOfNotDeletedEn = ErrorMessagesEn.CantUpdateInvDeleted;
                }
              
                if (!string.IsNullOrEmpty(reasonOfNotDeletedAr))
                    return new ResponseResult()
                    {
                        Result = Result.CanNotBeUpdated,
                        ErrorMessageAr = reasonOfNotDeletedAr,
                        ErrorMessageEn = reasonOfNotDeletedEn
                    };
            }

         

            // redefine Invoice Request to avoid any error eccured while creating invoice
            List<int> FileId = parameter.FileId;
            int InvoiceId = parameter.InvoiceId;
            var redefineInvoiceRequest = await redefineInvoiceRequestService.setInvoiceRequest(parameter, setting, invoiceTypeId, userInfo, "", invoicesListWOtype.First().PriceListId, 0);
            if (redefineInvoiceRequest.Item2 != "")
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = redefineInvoiceRequest.Item2, ErrorMessageEn = redefineInvoiceRequest.Item3 };
            parameter = Mapping.Mapper.Map<InvoiceMasterRequest, UpdateInvoiceMasterRequest>(redefineInvoiceRequest.Item1);
            parameter.InvoiceId = InvoiceId;
            parameter.FileId = FileId;
             
            //parameter.BranchId = 4;
            OfferPriceMasterCommand.StartTransaction();
            try
            {
                var signal = GeneralAPIsService.GetSignal(invoiceTypeId);

                var invoice = await OfferPriceMasterQuery.GetByAsync(q => q.InvoiceId == parameter.InvoiceId);
                if (invoice.BranchId != currentBranch)
                {
                    return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "لا يمكن تغيير المخزن. المخزن تابع لفرع اخر " };
                }

                parameter.InvoiceDate = parameter.InvoiceDate.Date == invoice.InvoiceDate.Date ? invoice.InvoiceDate : GeneralAPIsService.serverDate(parameter.InvoiceDate);

                invoice.InvoiceType = currentBranch + "-" + invoiceType + "-" + invoice.Code;

                parameter.SalesManId = parameter.SalesManId == 0 ? parameter.SalesManId = null : parameter.SalesManId;

                // recalculate results to avoid any changings in data from user
                CalculationOfInvoiceParameter calculationOfInvoiceParameter = new CalculationOfInvoiceParameter()
                {
                    DiscountType = parameter.DiscountType,
                    InvoiceTypeId = invoiceTypeId,
                    TotalDiscountRatio = parameter.TotalDiscountRatio,
                    TotalDiscountValue = parameter.TotalDiscountValue,
                    PersonId = parameter.PersonId,
                    InvoiceId = parameter.InvoiceId,
                    isDiscountRatio= parameter.isDiscountRatio
                };
                Mapping.Mapper.Map<List<InvoiceDetailsRequest>, List<InvoiceDetailsAttributes>>(parameter.InvoiceDetails, calculationOfInvoiceParameter.itemDetails);
                var recalculate = await CalcSystem.StartCalculation(calculationOfInvoiceParameter);

                int count = 0;
                foreach (var item in parameter.InvoiceDetails)//.Where(a => a.parentItemId == null||a.parentItemId==0).ToList())
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
                // set default data
                //  double paid = 0;
                if (parameter.BookIndex == null)
                    parameter.BookIndex = "";
                if (parameter.ParentInvoiceCode == null)
                    parameter.ParentInvoiceCode = "";

                if (parameter.Notes == null)
                    parameter.Notes = "";

               

                //invoice.EmployeeId = userInfo.employeeId;

                invoice.BranchId = currentBranch;
                invoice.Code = invoice.Code;
                invoice.InvoiceTypeId = invoiceTypeId;
                invoice.BookIndex = (parameter.BookIndex != null ? parameter.BookIndex.Trim() : parameter.BookIndex);
                invoice.InvoiceDate = parameter.InvoiceDate;
                invoice.StoreId = parameter.StoreId;

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
                invoice.VirualPaid = parameter.Paid;//parameter.Paid;
                invoice.RoundNumber = (setting.Other_Decimals > invoice.RoundNumber ? setting.Other_Decimals : invoice.RoundNumber);

                invoice.Remain = roundNumbers.GetRoundNumber(recalculate.Net - parameter.Paid, invoice.RoundNumber);
                invoice.TotalDiscountRatio = recalculate.TotalDiscountRatio; // parameter.TotalDiscountRatio;
                invoice.TotalVat = recalculate.TotalVat;// parameter.TotalVat;
                invoice.TotalDiscountValue = recalculate.TotalDiscountValue;// parameter.TotalDiscountValue;
                invoice.TotalPrice = recalculate.TotalPrice;// parameter.TotalPrice;
                invoice.TotalAfterDiscount = recalculate.TotalAfterDiscount;// parameter.TotalAfterDiscount;
                                                                            // invoice.Serialize = invoice.Code;
                invoice.IsDeleted = false;
                invoice.PaymentType = (int)PaymentType.Delay;
                invoice.ParentInvoiceCode = invoice.InvoiceType;
                invoice.InvoiceSubTypesId = (int)SubType.OfferPriceUnAccepted;
                invoice.isDiscountRatio = parameter.isDiscountRatio;

                var saved = await OfferPriceMasterCommand.UpdateAsyn(invoice);
                if (!saved)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice master" };
                // Delete Details 
                var invoiceDetails = OfferPriceDetailsQuery.FindAll(q => q.InvoiceId == parameter.InvoiceId).ToList();

                // save details of items in the invoice
                var invoiceDetailsList = new List<OfferPriceDetails>();
                if (parameter.InvoiceDetails != null)
                {
                    if (parameter.InvoiceDetails.Count() > 0)
                    {

                        if ((setting.Other_MergeItems == true && setting.otherMergeItemMethod == "withSave") ) //setting with marge
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
                            var invoiceDetails_ = new OfferPriceDetails();
                            invoiceDetails_ = addTempInvoice.ItemDetails(invoice, item);
                            if (item.parentItemId != null && item.parentItemId != 0)
                            {
                                item.IndexOfItem = 0;
                                invoiceDetails_.indexOfItem = 0;
                            }
                            else
                            {
                                item.IndexOfItem = index;
                                invoiceDetails_.indexOfItem = index;
                            }
                            invoiceDetailsList.Add(invoiceDetails_);
                          
                        }

                        OfferPriceDetailsCommand.AddRange(invoiceDetailsList);

                        OfferPriceDetailsCommand.RemoveRange(invoiceDetails);
                        saved = await OfferPriceDetailsCommand.SaveAsync();


                        if (!saved)
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice details" };

                    }
                }

               
        
                // to update TotalPrice after calculations
                await OfferPriceMasterCommand.UpdateAsyn(invoice);
                HistoryInvoiceService.HistoryInvoiceMaster(invoice.BranchId, invoice.Notes, invoice.BrowserName, lastTransactionAction: "U", null, invoice.BookIndex, invoice.Code
              , invoice.InvoiceDate, invoice.InvoiceId, invoice.InvoiceType, invoice.InvoiceTypeId, (int)SubType.Nothing, invoice.IsDeleted, invoice.ParentInvoiceCode, invoice.Serialize, invoice.StoreId, invoice.TotalPrice);

                OfferPriceMasterCommand.CommitTransaction();

             

                SystemActionEnum systemActionEnum = new SystemActionEnum();
               
                if (invoice.InvoiceTypeId == (int)DocumentType.OfferPrice)
                    systemActionEnum = SystemActionEnum.editOfferPrice;
                else if (invoice.InvoiceTypeId == (int)DocumentType.PurchaseOrder)
                    systemActionEnum = SystemActionEnum.editPurchaseOrder;
                await systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);

               
                return new ResponseResult() { Data = null, Id = parameter.InvoiceId, Code = invoice.Code, Result = Result.Success };
            }
            catch (Exception e)
            {
                return new ResponseResult { Data = e };
            }
        }

  
    }
}
