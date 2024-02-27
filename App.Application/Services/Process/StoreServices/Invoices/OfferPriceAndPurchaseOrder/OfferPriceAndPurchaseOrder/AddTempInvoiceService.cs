using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Process.Invoices;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Infrastructure;
using Hangfire;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using static App.Application.Services.Reports.Items_Prices.Rpt_Store;

namespace App.Application
{
    public class AddTempInvoiceService : BaseClass, IAddTempInvoiceService
    {
        private readonly IRepositoryCommand<OfferPriceMaster> OfferPriceMasterRepositoryCommand;
        private readonly IRepositoryQuery<OfferPriceMaster> OfferPriceMasterRepositoryQuery;
        private readonly IRepositoryCommand<OfferPriceDetails> OfferPriceDetailsRepositoryCommand;
        private readonly ICalculationSystemService CalcSystem;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryQuery<InvStoreBranch> invStoreBranchQuery;
        private readonly IRedefineInvoiceRequestService redefineInvoiceRequestService;
        private readonly iUserInformation Userinformation;
        private readonly IRoundNumbers roundNumbers;
        private readonly IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private SettingsOfInvoice SettingsOfInvoice;
        private readonly ISystemHistoryLogsService systemHistoryLogsService;

        public AddTempInvoiceService(
                              IRepositoryCommand<OfferPriceMaster> _OfferPriceMasterRepositoryCommand,
                              IRepositoryCommand<OfferPriceDetails> _OfferPriceDetailsRepositoryCommand,
                              IHistoryInvoiceService _HistoryInvoiceService,
                              IGeneralAPIsService _GeneralAPIsService,
                              IRepositoryQuery<OfferPriceMaster> OfferPriceMasterRepositoryQuery,
                                ICalculationSystemService CalcSystem,
                               ISecurityIntegrationService securityIntegrationService,
                                iUserInformation Userinformation, IRepositoryQuery<InvStoreBranch> invStoreBranchQuery,
                                IRedefineInvoiceRequestService redefineInvoiceRequestService, IRoundNumbers roundNumbers,
                                IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery,
        IHttpContextAccessor _httpContext, IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery, ISystemHistoryLogsService systemHistoryLogsService) : base(_httpContext)
        {
            OfferPriceMasterRepositoryCommand = _OfferPriceMasterRepositoryCommand;
            OfferPriceDetailsRepositoryCommand = _OfferPriceDetailsRepositoryCommand;
            httpContext = _httpContext;
            HistoryInvoiceService = _HistoryInvoiceService;
            GeneralAPIsService = _GeneralAPIsService;
            this.OfferPriceMasterRepositoryQuery = OfferPriceMasterRepositoryQuery;
            this.CalcSystem = CalcSystem;
            _securityIntegrationService = securityIntegrationService;
            this.Userinformation = Userinformation;
            this.invStoreBranchQuery = invStoreBranchQuery;
            this.redefineInvoiceRequestService = redefineInvoiceRequestService;
            this.roundNumbers = roundNumbers;
            this.itemCardPartsQuery = itemCardPartsQuery;
            InvGeneralSettingsRepositoryQuery = invGeneralSettingsRepositoryQuery;
            this.systemHistoryLogsService = systemHistoryLogsService;
        }
        public int Autocode(int BranchId, int invoiceType)
        {
            var Code = 1;
            Code = OfferPriceMasterRepositoryQuery.GetMaxCode(e => e.Code, a => a.InvoiceTypeId == invoiceType && a.BranchId == BranchId);

            if (Code != null)
                Code++;

            return Code;
        }

      

        // add offer price or purchase order
        public async Task<ResponseResult> addTempInvoice(InvoiceMasterRequest parameter, int invoiceTypeId , string invoiceType)
        {
            parameter.ParentInvoiceCode = "";
          
            var security = await _securityIntegrationService.getCompanyInformation();
            if (!security.isInfinityNumbersOfInvoices)
            {
                var invoicesCount = OfferPriceMasterRepositoryQuery.TableNoTracking.Where(x => x.InvoiceTypeId == invoiceTypeId).Count();
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
            var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);

            if(invoiceTypeId==(int)DocumentType.OfferPrice)
            {
                SettingsOfInvoice = new SettingsOfInvoice
                {
                    ActiveDiscount = setting.Sales_ActiveDiscount,
                    ActiveVat = setting.Vat_Active,
                    PriceIncludeVat = setting.Sales_PriceIncludeVat,
                    setDecimal = setting.Other_Decimals,
                };

            }
            else if(invoiceTypeId==(int)DocumentType.PurchaseOrder)
            {
                SettingsOfInvoice = new SettingsOfInvoice
                {
                    ActiveDiscount = setting.Purchases_ActiveDiscount,
                    ActiveVat = setting.Vat_Active,
                    PriceIncludeVat = setting.Purchases_PriceIncludeVat,
                    setDecimal = setting.Other_Decimals,
                };

            }
           
            var valid = GeneralAPIsService.ValidationOfInvoices(parameter, invoiceTypeId, setting, userInfo.CurrentbranchId, false, userInfo.userStors, DateTime.Now,false);
            if (valid.Result.Result != Result.Success)
                return valid.Result;
            var currentBranch = invStoreBranchQuery.TableNoTracking
                    .Where(a => a.StoreId == parameter.StoreId).First().BranchId;

            // redefine Invoice Request to avoid any error eccured while creating invoice

            var redefineInvoiceRequest = await redefineInvoiceRequestService.setInvoiceRequest(parameter, setting, invoiceTypeId, userInfo, parameter.ParentInvoiceCode,null,0);
            if (redefineInvoiceRequest.Item2 != "")
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = redefineInvoiceRequest.Item2, ErrorMessageEn = redefineInvoiceRequest.Item3 };
            parameter = redefineInvoiceRequest.Item1;

            parameter.InvoiceDate = GeneralAPIsService.serverDate(parameter.InvoiceDate);
            // int NextCode = Autocode(currentBranch, invoiceTypeId);
            OfferPriceMasterRepositoryCommand.StartTransaction();


            try
            {
                var signal = -1;

                //setting validation

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
                Mapping.Mapper.Map(parameter.InvoiceDetails, calculationOfInvoiceParameter.itemDetails);
                var recalculate = await CalcSystem.StartCalculation(calculationOfInvoiceParameter);
                // Mapping.Mapper.Map<InvoiceResultCalculateDto, InvoiceMasterRequest>(  recalculate,parameter);
                int count = 0;
                foreach (var item in parameter.InvoiceDetails)
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

                if (parameter.BookIndex == null)
                    parameter.BookIndex = "";
                if (parameter.ParentInvoiceCode == null)
                    parameter.ParentInvoiceCode = "";

                if (parameter.Notes == null)
                    parameter.Notes = "";

                var invoice = new OfferPriceMaster()
                {
                    EmployeeId = userInfo.employeeId,
                    BranchId = currentBranch,
                    InvoiceTypeId = invoiceTypeId,
                    BookIndex = parameter.BookIndex != null ? parameter.BookIndex.Trim() : parameter.BookIndex,
                    InvoiceDate = parameter.InvoiceDate,
                    StoreId = parameter.StoreId,
                    PersonId = parameter.PersonId,
                    Notes = parameter.Notes != null ? parameter.Notes.Trim() : parameter.Notes,
                    TransferNotesAR = "",
                    TransferNotesEN = "",
                    DiscountType = parameter.DiscountType,
                    BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString(),
                    Net = recalculate.Net,
                    Paid = parameter.Paid,
                    VirualPaid = parameter.Paid,
                    Remain = roundNumbers.GetRoundNumber(recalculate.Net - parameter.Paid, setting.Other_Decimals),
                    TotalDiscountRatio = recalculate.TotalDiscountRatio,// parameter.TotalDiscountRatio,
                    TotalVat = recalculate.TotalVat,// parameter.TotalVat,
                    TotalDiscountValue = recalculate.TotalDiscountValue, // parameter.TotalDiscountValue,
                    TotalPrice = recalculate.TotalPrice, // parameter.TotalPrice,
                    TotalAfterDiscount = recalculate.TotalAfterDiscount, // parameter.TotalAfterDiscount,
                    ParentInvoiceCode = parameter.ParentInvoiceCode,
                    SalesManId = parameter.SalesManId,
                    PriceListId = parameter.PriceListId,
                    transferStatus = parameter.transferStatus,
                    PaymentType = (int)PaymentType.Delay,
                    isDiscountRatio = parameter.isDiscountRatio


                };

                invoice.RoundNumber = setting.Other_Decimals;
                if (SettingsOfInvoice != null)
                {
                    invoice.ActiveDiscount = SettingsOfInvoice.ActiveDiscount;
                    invoice.ApplyVat = SettingsOfInvoice.ActiveVat;
                    invoice.PriceWithVat = SettingsOfInvoice.PriceIncludeVat;
                    invoice.RoundNumber = SettingsOfInvoice.setDecimal;

                }

                //    invoice.ParentInvoiceCode = invoice.InvoiceType;
                invoice.Serialize = Convert.ToDouble(GeneralAPIsService.CreateSerializeOfInvoice(invoiceTypeId, 0, invoice.BranchId).ToString());

                int NextCode = Autocode(currentBranch, invoiceTypeId);

               
                   invoice.InvoiceType = userInfo.CurrentbranchCode.ToString() + "-" + invoiceType + "-" + NextCode;
                   invoice.Code = NextCode;
                invoice.InvoiceSubTypesId = (int)SubType.OfferPriceUnAccepted;
                OfferPriceMasterRepositoryCommand.AddWithoutSaveChanges(invoice);


                var saved = await OfferPriceMasterRepositoryCommand.SaveAsync();

                if (!saved)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice master" };
              

                saved = GeneralAPIsService.addSerialize(invoice.Serialize, invoice.InvoiceId, invoiceTypeId, invoice.BranchId);
                if (!saved)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild in add serialize" };

                // save details of items in the invoice
                var invoiceDetailsList = new List<OfferPriceDetails>();
                if (parameter.InvoiceDetails != null)
                {
                    if (parameter.InvoiceDetails.Count() > 0)
                    {

                        if (setting.Other_MergeItems == true && setting.otherMergeItemMethod == "withSave") //setting with marge
                        {
                            var invoiceDetailsListMerg = await GeneralAPIsService.MergeItems(parameter.InvoiceDetails.ToList(), invoiceTypeId);
                            parameter.InvoiceDetails = invoiceDetailsListMerg;
                        }

                        int index = 0;
                        foreach (var item in parameter.InvoiceDetails)
                        {

                            index++;
                            var invoiceDetails = new OfferPriceDetails();

                            invoiceDetails = ItemDetails(invoice, item);
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
                                    item.IndexOfItem = index;
                                    invoiceDetails.indexOfItem = index;
                                }

                            }

                            invoiceDetailsList.Add(invoiceDetails);

                        }

                        OfferPriceDetailsRepositoryCommand.AddRange(invoiceDetailsList);

                        saved = await OfferPriceDetailsRepositoryCommand.SaveAsync();
                        if (!saved)
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice details" };


                    }
                }

                var parentForHistory = invoice.InvoiceType;

                if (Lists.returnInvoiceList.Contains(invoiceTypeId))
                    parentForHistory = invoice.ParentInvoiceCode;

                HistoryInvoiceService.HistoryInvoiceMaster(invoice.BranchId, invoice.Notes, invoice.BrowserName, "A", null, invoice.BookIndex, invoice.Code
                 , invoice.InvoiceDate, invoice.InvoiceId, invoice.InvoiceType, invoice.InvoiceTypeId, invoice.InvoiceSubTypesId, invoice.IsDeleted, parentForHistory, invoice.Serialize, invoice.StoreId, invoice.TotalPrice);
                //if(invoice.InvoiceTypeId==(int)DocumentType.Purchase)
                //   await   accrediteInvoice.accrediteAllInvoice(null, invoice.InvoiceTypeId, invoice.InvoiceId);


                OfferPriceMasterRepositoryCommand.CommitTransaction();

                SystemActionEnum systemActionEnum = new SystemActionEnum();

                if (invoiceTypeId == (int)DocumentType.OfferPrice)
                    systemActionEnum = SystemActionEnum.addOfferPrice;
                else if (invoiceTypeId == (int)DocumentType.PurchaseOrder)
                    systemActionEnum = SystemActionEnum.addPurchaseOrder;
                if (systemActionEnum != null)
                    await systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);
                return new ResponseResult { Result = Result.Success, Id = invoice.InvoiceId, Code = invoice.Code };
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public OfferPriceDetails ItemDetails(OfferPriceMaster invoice, InvoiceDetailsRequest item)//, InvoiceDetails invoiceDetails)
        {
            var invoiceDetails = new OfferPriceDetails();
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
            invoiceDetails.DiscountRatio = item.DiscountRatio;
            invoiceDetails.DiscountValue = item.DiscountValue;
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
            invoiceDetails.isDiscountRatioItem = item.isDiscountRatioItem;

            invoiceDetails.VatValueForPrint = item.VatValueForPrint;

            if (item.ItemTypeId == (int)ItemTypes.Expiary)
                invoiceDetails.ExpireDate = item.ExpireDate;
            else
                invoiceDetails.ExpireDate = null;

            invoiceDetails.balanceBarcode = item.balanceBarcode;
            return invoiceDetails;
        }

        public List<InvoiceDetailsRequest> setCompositItem(OfferPriceMaster invoice, int itemId, int unitId, int indexOfItem, double qty)
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
