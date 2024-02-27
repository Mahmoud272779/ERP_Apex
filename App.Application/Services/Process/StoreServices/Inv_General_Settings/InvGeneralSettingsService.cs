using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Domain.Entities.Setup;
using static App.Application.Helpers.Aliases;
using App.Application.Helpers;
using App.Domain.Models.Request.General;
using Microsoft.CodeAnalysis.Operations;
using App.Application.Services.HelperService.EmailServices;
using App.Domain.Models.Response.General;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;

namespace App.Application.Services.Process.Inv_General_Settings
{
    public class InvGeneralSettingsService : BaseClass, IInvGeneralSettingsService
    {
        private readonly IRepositoryQuery<InvGeneralSettings> GSRepositoryQuery;
        private readonly IRepositoryCommand<InvGeneralSettings> GSRepositoryCommand;
        private readonly IRepositoryCommand<InvCategories> CategoriesCommand;
        private readonly IRepositoryQuery<InvCategories> CategoriesQuery;
        private readonly IRepositoryCommand<InvStpItemCardMaster> itemCardCommand;
        private readonly iUserInformation _iUserInformation;
        private readonly IEmailService _emailService;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardQuery;

        private readonly IHttpContextAccessor httpContext;
        public InvGeneralSettingsService(IRepositoryQuery<InvGeneralSettings> _GSRepositoryQuery,
                                 IRepositoryCommand<InvGeneralSettings> _GSRepositoryCommand,
                                 IRepositoryCommand<InvCategories> CategoriesCommand,
                                 IRepositoryQuery<InvCategories> CategoriesQuery,
                                 IRepositoryCommand<InvStpItemCardMaster> itemCardCommand,
                                 iUserInformation iUserInformation,
                                 IEmailService emailService,
                                 IRepositoryQuery<InvStpItemCardMaster> itemCardQuery,
                                  IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            GSRepositoryQuery = _GSRepositoryQuery;
            GSRepositoryCommand = _GSRepositoryCommand;
            this.CategoriesCommand = CategoriesCommand;
            this.CategoriesQuery = CategoriesQuery;
            this.itemCardCommand = itemCardCommand;
            _iUserInformation = iUserInformation;
            _emailService = emailService;
            this.itemCardQuery = itemCardQuery;
            httpContext = _httpContext;
        }

        public async Task<ResponseResult> UpdatePurchasesSettings(PurchasesRequest request)
        {

            request.Id = 1;
            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == request.Id);

            var table = Mapping.Mapper.Map<PurchasesRequest, InvGeneralSettings>(request, data);
            var res = await GSRepositoryCommand.UpdateAsyn(table);
            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }
        public async Task<ResponseResult> UpdatePOSSettings(POSRequest request)
        {

            request.Id = 1;
            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == request.Id);

            var table = Mapping.Mapper.Map<POSRequest, InvGeneralSettings>(request, data);

            var res = await GSRepositoryCommand.UpdateAsyn(table);

            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }



        public async Task<ResponseResult> UpdateSalesSettings(SalesRequest request)
        {

            request.Id = 1;
            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == request.Id);

            var table = Mapping.Mapper.Map<SalesRequest, InvGeneralSettings>(request, data);
            var res = await GSRepositoryCommand.UpdateAsyn(table);
            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }
        public async Task<ResponseResult> UpdateOtherSettings(OtherRequest request)
        {
            request.Id = 1;
            if(request.other_ActivatePriceLists && 
                (request.PriceListType<(int)PriceListsType.PriceLists || 
                   request.PriceListType > (int)PriceListsType.SalesManPrice))
                return new ResponseResult() { Result = Result.RequiredData , ErrorMessageAr = "يجب اختيار سعر من قائمة الأسعار " ,
                ErrorMessageEn = "Should select price from prices list"};

            if(request.Other_ExpireNotificationFlag && request.Other_ExpireNotificationValue<=0)
                return new ResponseResult()
                { 
                    Result = Result.RequiredData,
                    ErrorMessageAr = "يجب تحديد فترة انتهاء الصلاحية",
                    ErrorMessageEn = "Should select period of expiry"
                };


            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == request.Id);
            var table = Mapping.Mapper.Map<OtherRequest, InvGeneralSettings>(request, data);
            var res = await GSRepositoryCommand.UpdateAsyn(table);
            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }
        public async Task<ResponseResult> UpdateFundsSettings(FundsRequest request)
        {

            request.Id = 1;
            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == request.Id);

            var table = Mapping.Mapper.Map<FundsRequest, InvGeneralSettings>(request, data);
            var res = await GSRepositoryCommand.UpdateAsyn(table);
            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }
        public async Task<ResponseResult> UpdateBarcodeSettings(BarcodeRequest request)
        {
            request.Id = 1;
            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == request.Id);

            var table = Mapping.Mapper.Map<BarcodeRequest, InvGeneralSettings>(request, data);
            var res = await GSRepositoryCommand.UpdateAsyn(table);
            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }
        public async Task<ResponseResult> UpdateVATSettings(VATRequest request)
        {


            var categories = CategoriesQuery.TableNoTracking.ToList();
            categories.Select(a => { a.VatValue = request.Vat_DefaultValue; return a; }).ToList();
            await CategoriesCommand.UpdateAsyn(categories);
            CategoriesCommand.SaveChanges();

            var itemsCard = itemCardQuery.TableNoTracking.ToList();
            itemsCard.Select(a => { a.VAT = request.Vat_DefaultValue; a.ApplyVAT = request.Vat_Active; return a; }).ToList();
            await itemCardCommand.UpdateAsyn(itemsCard);
            itemCardCommand.SaveChanges();

            request.Id = 1;
            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == request.Id);

            var table = Mapping.Mapper.Map<VATRequest, InvGeneralSettings>(request, data);
            var res = await GSRepositoryCommand.UpdateAsyn(table);
            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }
        public async Task<ResponseResult> UpdateAccrediteSettings(AccrediteRequest request)
        {

            request.Id = 1;
            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == request.Id);

            var table = Mapping.Mapper.Map<AccrediteRequest, InvGeneralSettings>(request, data);
            var res = await GSRepositoryCommand.UpdateAsyn(table);
            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }
        public async Task<ResponseResult> UpdateCustomerDisplaySettings(CustomerDisplayRequest request)
        {

            if (int.Parse(request.CustomerDisplay_PortNumber) < 1 || int.Parse(request.CustomerDisplay_PortNumber) > 20)
                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed, Note = "port number must be between 1 to 20" };

            if (request.CustomerDisplay_CharNumber < 10 || request.CustomerDisplay_CharNumber > 50)
                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed, Note = "char number must be between 10 to 50" };

            request.Id = 1;
            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == request.Id);

            var table = Mapping.Mapper.Map<CustomerDisplayRequest, InvGeneralSettings>(request, data);
            var res = await GSRepositoryCommand.UpdateAsyn(table);
            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }
          public async Task<ResponseResult> UpdateElectronicInvoiceSettings(ElectronicInvoiceRequest request)
        {
            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == request.Id);

            var table = Mapping.Mapper.Map<ElectronicInvoiceRequest, InvGeneralSettings>(request, data);
            var res = await GSRepositoryCommand.UpdateAsyn(table);
            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }

      


        public async Task<ResponseResult> GetPurchasesSettings()
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            var resData = await GSRepositoryQuery.Get();

            var responseData = new Purchases()
            {
                Purchases_ActiveDiscount = resData.First().Purchases_ActiveDiscount && userInfo.otherSettings.purchasesAddDiscount ? true : false,
                Purchases_PayTotalNet = resData.First().Purchases_PayTotalNet && !userInfo.otherSettings.purchasesAllowCreditSales ? true : false,



                Purchases_ModifyPrices = resData.First().Purchases_ModifyPrices,
                Purchases_PriceIncludeVat = resData.First().Purchases_PriceIncludeVat,
                Purchases_PrintWithSave = resData.First().Purchases_PrintWithSave,
                Purchases_ReturnWithoutQuantity = resData.First().Purchases_ReturnWithoutQuantity,
                Purchases_UseLastPrice = resData.First().Purchases_UseLastPrice,
                Purchase_UpdateItemsPricesAfterInvoice = resData.First().Purchase_UpdateItemsPricesAfterInvoice,

            };
            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }
        public async Task<ResponseResult> GetPOSSettings()
        {
            var resData = await GSRepositoryQuery.Get();

            var responseData = new POS()
            {
                Pos_ActiveCashierCustody = resData.First().Pos_ActiveCashierCustody,
                Pos_ActiveDiscount = resData.First().Pos_ActiveDiscount,
                Pos_ActivePricesList = resData.First().Pos_ActivePricesList,
                Pos_DeferredSale = resData.First().Pos_DeferredSale,
                Pos_EditingOnDate = resData.First().Pos_EditingOnDate,
                Pos_ExceedDiscountRatio = resData.First().Pos_ExceedDiscountRatio,
                Pos_ModifyPricesType = resData.First().Pos_ModifyPricesType,
                Pos_ExtractWithoutQuantity = resData.First().Pos_ExtractWithoutQuantity,
                Pos_IndividualCoding = resData.First().Pos_IndividualCoding,
                Pos_ModifyPrices = resData.First().Pos_ModifyPrices,
                Pos_PreventEditingRecieptFlag = resData.First().Pos_PreventEditingRecieptFlag,
                Pos_PreventEditingRecieptValue = resData.First().Pos_PreventEditingRecieptValue,
                Pos_PriceIncludeVat = resData.First().Pos_PriceIncludeVat,
                Pos_PrintPreview = resData.First().Pos_PrintPreview,
                Pos_PrintWithEnding = resData.First().Pos_PrintWithEnding,
              //  Pos_UseLastPrice = resData.First().LastPriceForCustomer,
                PosTochSettings = new PosTouch()
                {
                    //categoryImageHeight = resData.First().PosTouch_CategoryImgHeight,
                    //categoryImageWidth = resData.First().PosTouch_CategoryImgWidth,
                    //fontSize = resData.First().PosTouch_FontSize,
                    //itemsImageHeight = resData.First().PosTouch_ItemsImgHeight,
                    //itemsImageWidth = resData.First().PosTouch_ItemsImgWidth,
                    //displayItemPrice = resData.First().PosTouch_DisplayItemPrice,
                }
            };
            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }

        public async Task<ResponseResult> GetSalesSettings()
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var resData = await GSRepositoryQuery.Get();
            var responseData = new Sales()
            {
                Sales_ActiveDiscount = resData.First().Sales_ActiveDiscount && userInfo.otherSettings.salesAddDiscount ? true : false,
                Sales_PayTotalNet = resData.First().Sales_PayTotalNet && !userInfo.otherSettings.salesAllowCreditSales ? true : false,



                Sales_ActivePricesList = resData.First().Sales_ActivePricesList,
                Sales_ExceedDiscountRatio = resData.First().Sales_ExceedDiscountRatio,
                Sales_ModifyPricesType = resData.First().Sales_ModifyPricesType,
                Sales_ExtractWithoutQuantity = resData.First().Sales_ExtractWithoutQuantity,
                Sales_LinkRepresentCustomer = resData.First().Sales_LinkRepresentCustomer,
                Sales_ModifyPrices = resData.First().Sales_ModifyPrices,
                Sales_PriceIncludeVat = resData.First().Sales_PriceIncludeVat,
                Sales_PrintWithSave = resData.First().Sales_PrintWithSave,
              //  Sales_UseLastPrice = resData.First().LastPriceForCustomer
            };
            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }
        public async Task<ResponseResult> GetOtherSettings()
        {
            var resData = await GSRepositoryQuery.Get();
            var userInfo = await _iUserInformation.GetUserInformation();
            var responseData = new Other()
            {
                Other_AutoExtractExpireDate = resData.First().Other_AutoExtractExpireDate,
                Other_ConfirmeCustomerPhone = resData.First().Other_ConfirmeCustomerPhone,
                Other_ConfirmeSupplierPhone = resData.First().Other_ConfirmeSupplierPhone,
                Other_Decimals = resData.First().Other_Decimals,
                Other_DemandLimitNotification = resData.First().Other_DemandLimitNotification,
                Other_ExpireNotificationFlag = resData.First().Other_ExpireNotificationFlag,
                Other_ExpireNotificationValue = resData.First().Other_ExpireNotificationValue,
                Other_ItemsAutoCoding = resData.First().Other_ItemsAutoCoding,
                Other_MergeItems = resData.First().Other_MergeItems,
                otherMergeItemMethod = resData.First().otherMergeItemMethod,
                Other_PrintSerials = resData.First().Other_PrintSerials,
                Other_ViewStorePlace = resData.First().Other_ViewStorePlace,
                Other_ZeroPricesInItems = resData.First().Other_ZeroPricesInItems,
                Other_useRoundNumber = resData.First().Other_UseRoundNumber,
                autoLogoutInMints = resData.First().autoLogoutInMints,
                Other_ShowBalanceOfPerson = resData.First().Other_ShowBalanceOfPerson,
                isCollectionReceipt = userInfo.otherSettings.CollectionReceipts,
                other_ActivatePriceLists = resData.First().other_ActivatePriceLists,
                PriceListType = resData.First().PriceListType,
                isExpenses = resData.First().isExpenses,
                

            };
            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }

        public async Task<ResponseResult> GetBarcodeSettings()
        {
            var resData = await GSRepositoryQuery.Get();

            var responseData = new Barcode()
            {
                barcodeType = resData.First().barcodeType,
                Barcode_ItemCodestart = resData.First().Barcode_ItemCodestart

            };
            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }
        public async Task<ResponseResult> GetCustomerDisplaySettings()
        {
            var resData = await GSRepositoryQuery.Get();
            var responseData = new CustomerDisplay()
            {
                CustomerDisplay_Active = resData.First().CustomerDisplay_Active,
                CustomerDisplay_CharNumber = resData.First().CustomerDisplay_CharNumber,
                CustomerDisplay_Code = resData.First().CustomerDisplay_Code,
                CustomerDisplay_DefaultWord = resData.First().CustomerDisplay_DefaultWord,
                CustomerDisplay_LinesNumber = resData.First().CustomerDisplay_LinesNumber,
                CustomerDisplay_PortNumber = resData.First().CustomerDisplay_PortNumber,
                CustomerDisplay_ScreenType = resData.First().CustomerDisplay_ScreenType

            };
            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }

        public async Task<ResponseResult> GetFundsSettings()
        {
            var resData = await GSRepositoryQuery.Get();
            var responseData = new Funds()
            {
                Funds_Banks = resData.First().Funds_Banks,
                Funds_Customers = resData.First().Funds_Customers,
                Funds_Items = resData.First().Funds_Items,
                Funds_Safes = resData.First().Funds_Safes,
                Funds_Supplires = resData.First().Funds_Supplires
            };
            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }

        public async Task<ResponseResult> GetVATSettings()
        {
            var resData = await GSRepositoryQuery.Get();
            var responseData = new VAT()
            {
                Vat_Active = resData.First().Vat_Active,
                Vat_DefaultValue = resData.First().Vat_DefaultValue
            };

            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }
        public async Task<ResponseResult> GetAccrediteSettings()
        {
            var resData = await GSRepositoryQuery.Get();
            var responseData = new Accredite()
            {
                Accredite_EndPeriod = resData.First().Accredite_EndPeriod,
                Accredite_StartPeriod = resData.First().Accredite_StartPeriod
            };
            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }
        public async Task<Other> GetSettingsForprint()
        {
            var resData = await GSRepositoryQuery.Get();

            var responseData = new InvGeneralSettingsResponse();

            responseData.other = new Other()
            {

                Other_PrintSerials = resData.First().Other_PrintSerials,


            };
            var other = responseData.other;
            return other;
        }

        public async Task<ResponseResult> GetSettings()
        {
            var resData = await GSRepositoryQuery.Get();

            var responseData = new InvGeneralSettingsResponse();
            responseData.purchase = new Purchases()
            {
                Purchases_ActiveDiscount = resData.First().Purchases_ActiveDiscount,
                Purchases_ModifyPrices = resData.First().Purchases_ModifyPrices,
                Purchases_PayTotalNet = resData.First().Purchases_PayTotalNet,
                Purchases_PriceIncludeVat = resData.First().Purchases_PriceIncludeVat,
                Purchases_PrintWithSave = resData.First().Purchases_PrintWithSave,
                Purchases_ReturnWithoutQuantity = resData.First().Purchases_ReturnWithoutQuantity,
                Purchases_UseLastPrice = resData.First().Purchases_UseLastPrice,
                Purchase_UpdateItemsPricesAfterInvoice= resData.First().Purchase_UpdateItemsPricesAfterInvoice
            };
            responseData.pos = new POS()
            {
                Pos_ActiveCashierCustody = resData.First().Pos_ActiveCashierCustody,
                Pos_ActiveDiscount = resData.First().Pos_ActiveDiscount,
                Pos_ActivePricesList = resData.First().Pos_ActivePricesList,
                Pos_DeferredSale = resData.First().Pos_DeferredSale,
                Pos_EditingOnDate = resData.First().Pos_EditingOnDate,
                Pos_ExceedDiscountRatio = resData.First().Pos_ExceedDiscountRatio,
                Pos_ModifyPricesType = resData.First().Pos_ModifyPricesType,
                Pos_ExtractWithoutQuantity = resData.First().Pos_ExtractWithoutQuantity,
                Pos_IndividualCoding = resData.First().Pos_IndividualCoding,
                Pos_ModifyPrices = resData.First().Pos_ModifyPrices,
                Pos_PreventEditingRecieptFlag = resData.First().Pos_PreventEditingRecieptFlag,
                Pos_PreventEditingRecieptValue = resData.First().Pos_PreventEditingRecieptValue,
                Pos_PriceIncludeVat = resData.First().Pos_PriceIncludeVat,
                Pos_PrintPreview = resData.First().Pos_PrintPreview,
                Pos_PrintWithEnding = resData.First().Pos_PrintWithEnding,
                Pos_UseLastPrice = resData.First().other_ActivatePriceLists && resData.First().PriceListType == (int)PriceListsType.LastPriceForCustomer
            };
            responseData.sales = new Sales()
            {
                Sales_ActiveDiscount = resData.First().Sales_ActiveDiscount,
                Sales_ActivePricesList = resData.First().Sales_ActivePricesList,
                Sales_ExceedDiscountRatio = resData.First().Sales_ExceedDiscountRatio,
                Sales_ModifyPricesType = resData.First().Sales_ModifyPricesType,
                Sales_ExtractWithoutQuantity = resData.First().Sales_ExtractWithoutQuantity,
                Sales_LinkRepresentCustomer = resData.First().Sales_LinkRepresentCustomer,
                Sales_ModifyPrices = resData.First().Sales_ModifyPrices,
                Sales_PayTotalNet = resData.First().Sales_PayTotalNet,
                Sales_PriceIncludeVat = resData.First().Sales_PriceIncludeVat,
                Sales_PrintWithSave = resData.First().Sales_PrintWithSave,
                Sales_UseLastPrice = resData.First().other_ActivatePriceLists && resData.First().PriceListType==(int)PriceListsType.LastPriceForCustomer
            };
            responseData.other = new Other()
            {
                Other_AutoExtractExpireDate = resData.First().Other_AutoExtractExpireDate,
                Other_ConfirmeCustomerPhone = resData.First().Other_ConfirmeCustomerPhone,
                Other_ConfirmeSupplierPhone = resData.First().Other_ConfirmeSupplierPhone,
                Other_Decimals = resData.First().Other_Decimals,
                Other_DemandLimitNotification = resData.First().Other_DemandLimitNotification,
                Other_ExpireNotificationFlag = resData.First().Other_ExpireNotificationFlag,
                Other_ExpireNotificationValue = resData.First().Other_ExpireNotificationValue,
                Other_ItemsAutoCoding = resData.First().Other_ItemsAutoCoding,
                Other_MergeItems = resData.First().Other_MergeItems,
                otherMergeItemMethod = resData.First().otherMergeItemMethod,
                Other_PrintSerials = resData.First().Other_PrintSerials,
                Other_ViewStorePlace = resData.First().Other_ViewStorePlace,
                Other_ZeroPricesInItems = resData.First().Other_ZeroPricesInItems,
                Other_ShowBalanceOfPerson = resData.First().Other_ShowBalanceOfPerson,
                PriceListType = resData.First().PriceListType,
                other_ActivatePriceLists = resData.First().other_ActivatePriceLists,
                other_AlertMessageForCustomerSupplierBalance= resData.First().other_AlertMessageForCustomerSupplierBalance,
                other_AutoExtractFromCustomerSupplierBalance= resData.First().other_AutoExtractFromCustomerSupplierBalance,
                other_DoNothingForCustomerSupplierBalance = resData.First().other_DoNothingForCustomerSupplierBalance

            };
            responseData.barcode = new Barcode()
            {
                barcodeType = resData.First().barcodeType,
                Barcode_ItemCodestart = resData.First().Barcode_ItemCodestart

            };
            responseData.customerDisplay = new CustomerDisplay()
            {
                CustomerDisplay_Active = resData.First().CustomerDisplay_Active,
                CustomerDisplay_CharNumber = resData.First().CustomerDisplay_CharNumber,
                CustomerDisplay_Code = resData.First().CustomerDisplay_Code,
                CustomerDisplay_DefaultWord = resData.First().CustomerDisplay_DefaultWord,
                CustomerDisplay_LinesNumber = resData.First().CustomerDisplay_LinesNumber,
                CustomerDisplay_PortNumber = resData.First().CustomerDisplay_PortNumber,
                CustomerDisplay_ScreenType = resData.First().CustomerDisplay_ScreenType

            };
            responseData.funds = new Funds()
            {
                Funds_Banks = resData.First().Funds_Banks,
                Funds_Customers = resData.First().Funds_Customers,
                Funds_Items = resData.First().Funds_Items,
                Funds_Safes = resData.First().Funds_Safes,
                Funds_Supplires = resData.First().Funds_Supplires
            };
            responseData.vat = new VAT()
            {
                Vat_Active = resData.First().Vat_Active,
                Vat_DefaultValue = resData.First().Vat_DefaultValue
            };
            responseData.accredite = new Accredite()
            {
                Accredite_EndPeriod = resData.First().Accredite_EndPeriod,
                Accredite_StartPeriod = resData.First().Accredite_StartPeriod
            };
            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }
        //public async Task<ResponseResult> UpdateSettingsWithSession(UpdateInvGeneralSettingsRequest parameters)
        //{
        //    if (parameters.Id != 1)
        //        return new ResponseResult() { Data = null, Id = parameters.Id, Result = Result.Failed };

        //    var data = await GSRepositoryQuery.GetByAsync(a => a.Id == parameters.Id);
        //    var table = Mapping.Mapper.Map<UpdateInvGeneralSettingsRequest, InvGeneralSettings>(parameters, data);
        //    httpContext.HttpContext.Session.SetString("",JsonConvert.SerializeObject(table));
        //    var res = await GSRepositoryCommand.UpdateAsyn(table);
        //    return new ResponseResult() { Data = null, Id = table.Id, Result = res ? Result.Success : Result.Failed };
        //}
        public async Task<ResponseResult> GetSettingsSession()
        {
            var resData = await GSRepositoryQuery.SingleOrDefault(q => q.Id == 1);

            var res = this.httpContext.HttpContext.Session.GetString("SessionKey");
            return new ResponseResult() { Data = res, Id = null };
            //https://www.c-sharpcorner.com/article/how-to-use-session-in-asp-net-core/
        }

        public async Task<ResponseResult> UpdateOtherGeneralSettings(updateOhterGeneralSettingsRequest parm)
        {
            var data = await GSRepositoryQuery.GetByAsync(a => a.Id == 1);
            data.Other_Decimals = parm.Other_Decimals;
            data.Other_UseRoundNumber = parm.Other_UseRoundNumber;
            data.Other_DemandLimitNotification = parm.other_DemandLimitNotification;
            data.Other_ExpireNotificationFlag = parm.other_ExpireNotificationFlag;
            data.Other_ExpireNotificationValue = parm.other_ExpireNotificationValue;
          
            if (parm.autoLogoutInMints < 0)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "وقت اغلاق الجلسه لا يمكن ان يكون اقل من 0",
                    ErrorMessageEn = "Ending Session time can not be less than 0"
                };
            if (parm.autoLogoutInMints > 1000)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "وقت اغلاق الجلسه لا يمكن ان يتجاوز 1000 دقيقه",
                    ErrorMessageEn = "Ending Session time can not be more than 1000 minutes"
                };
            data.autoLogoutInMints = parm.autoLogoutInMints;

            var res = await GSRepositoryCommand.UpdateAsyn(data);
            return new ResponseResult() { Data = null, Id = null, Result = res ? Result.Success : Result.Failed };
        }

        public async Task<ResponseResult> updateEmailSettings(emailSettingsDTO parm)
        {
            var settings = await GSRepositoryQuery.GetByIdAsync(1);

            settings.Email = parm.email;
            settings.EmailPassword = parm.password;
            settings.EmailPort = parm.port;
            settings.EmailDisplayName = parm.displayName;
            settings.EmailHost = parm.host;
            var saved = await GSRepositoryCommand.UpdateAsyn(settings);
            return new ResponseResult()
            {
                Note = saved ? Actions.SavedSuccessfully : Actions.SaveFailed,
                Result = saved ? Result.Success : Result.Failed
            };
        }

        public async Task<ResponseResult> TestEmailSend(string Email)
        {
            var TestSendingEmail = await _emailService.SendEmail(new emailRequest
            {
                subject = "Test Email Sending",
                body = "Test Email Sending",
                ToEmail = Email
            });
            return new ResponseResult()
            {
                Note = TestSendingEmail,
                Result = TestSendingEmail == "OK" ? Result.Success : Result.Failed
            };
        }

        public async Task<ResponseResult> GetEmailSettings()
        {
            var settings = await GSRepositoryQuery.GetByIdAsync(1);
            var res = new GenralSettingsResponseDTO()
            {
                displayName = settings.EmailDisplayName,
                email = settings.Email,
                host = settings.EmailHost,
                password = settings.EmailPassword,
                port = settings.EmailPort,
                secureSocketOptions = settings.secureSocketOptions
            };
            return new ResponseResult()
            {
                Data = res,
                Note = Actions.Success,
                Result = Result.Success
            };
        }



        public async Task<ResponseResult> GetElectronicInvoiceSettings()
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var resData = await GSRepositoryQuery.Get();
            var responseData = new ElectronicInvoice()
            {

                OTP = resData.First().OTP,
                ActiveElectronicInvoice = resData.First().ActiveElectronicInvoice
            };
            return new ResponseResult() { Data = responseData, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }

       

        //public  async Task<ResponseResult> GetVATSetting()
        //{
        //    var vat =  await GSRepositoryQuery.GetAsync(1);

        //    return new ResponseResult() { Data = vat.Vat_DefaultValue, Result = Result.Success };
        //}
    }
}
