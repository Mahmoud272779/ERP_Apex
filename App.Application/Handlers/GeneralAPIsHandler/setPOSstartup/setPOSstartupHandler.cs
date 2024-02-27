using App.Application.Helpers.POSHelper;
using App.Domain.Models.Response.Store.Invoices;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralAPIsHandler.setPOSstartup
{
    public class setPOSstartupHandler : IRequestHandler<setPOSstartupRequest, ResponseResult>
    {
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvPersons> PersonRepositorQuery;
        private readonly IRepositoryQuery<InvStpStores> storeQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;

        public setPOSstartupHandler(iUserInformation userinformation, IRepositoryQuery<InvoiceMaster> invoiceMasterRepositoryQuery, IRepositoryQuery<InvPersons> personRepositorQuery, IRepositoryQuery<InvStpStores> storeQuery, IRepositoryQuery<InvGeneralSettings> generalSettings)
        {
            Userinformation = userinformation;
            InvoiceMasterRepositoryQuery = invoiceMasterRepositoryQuery;
            PersonRepositorQuery = personRepositorQuery;
            this.storeQuery = storeQuery;
            GeneralSettings = generalSettings;
        }

        public async Task<ResponseResult> Handle(setPOSstartupRequest request, CancellationToken cancellationToken)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            bool showOthoerInvoice = POSHelper.showOthorInv(request.invoiceTypeId, userInfo);
            int invoiceTypeDel = (request.invoiceTypeId == (int)DocumentType.Purchase ? (int)DocumentType.DeletePurchase : request.invoiceTypeId == (int)DocumentType.Sales ? (int)DocumentType.DeleteSales : request.invoiceTypeId == (int)DocumentType.POS ? (int)DocumentType.POS : 0);

            int LastCode = InvoiceMasterRepositoryQuery.GetMaxCode(a => a.InvoiceId, q =>
                q.BranchId == userInfo.CurrentbranchId && (showOthoerInvoice ? 1 == 1 : q.EmployeeId == userInfo.employeeId)
                && ((q.InvoiceTypeId == request.invoiceTypeId && q.IsDeleted == false) || q.InvoiceTypeId == invoiceTypeDel));


            var nextCode = POSHelper.Autocode(userInfo.CurrentbranchId, request.invoiceTypeId, InvoiceMasterRepositoryQuery);

            var defaultCustomer = PersonRepositorQuery.TableNoTracking.Where(a => a.Id == 2)
                .Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName }).ToList().FirstOrDefault();
            var defaultStore = storeQuery.TableNoTracking.Where(a => a.StoreBranches.First().BranchId == userInfo.CurrentbranchId)
                .Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName }).ToList().FirstOrDefault();

            //GeneralSetting
            var generalSettings = GeneralSettings.TableNoTracking.FirstOrDefault();



            var res = new PosDto()
            {
                LastCode = LastCode,
                nextCode = nextCode,
                customerId = defaultCustomer.Id,
                customerCode = defaultCustomer.Code,
                customerAr = defaultCustomer.ArabicName,
                customerEn = defaultCustomer.LatinName,
                POSgeneralSettingsint = new Domain.Models.Response.Store.Invoices.GeneralSettings()
                {
                    Pos_ActiveCashierCustody = generalSettings.Pos_ActiveCashierCustody,
                    Pos_ActiveDiscount = generalSettings.Pos_ActiveDiscount,
                    Pos_ActivePricesList = generalSettings.Pos_ActivePricesList,
                    Pos_DeferredSale = generalSettings.Pos_DeferredSale,
                    Pos_EditingOnDate = generalSettings.Pos_EditingOnDate,
                    Pos_ExceedDiscountRatio = generalSettings.Pos_ExceedDiscountRatio,
                    Pos_ModifyPricesType = generalSettings.Pos_ModifyPricesType,
                    Pos_ExtractWithoutQuantity = generalSettings.Pos_ExtractWithoutQuantity,
                    Pos_IndividualCoding = generalSettings.Pos_IndividualCoding,
                    Pos_ModifyPrices = generalSettings.Pos_ModifyPrices,
                    Pos_PreventEditingRecieptFlag = generalSettings.Pos_PreventEditingRecieptFlag,
                    Pos_PreventEditingRecieptValue = generalSettings.Pos_PreventEditingRecieptValue,
                    Pos_PriceIncludeVat = generalSettings.Pos_PriceIncludeVat,
                    Pos_PrintPreview = generalSettings.Pos_PrintPreview,
                    Pos_PrintWithEnding = generalSettings.Pos_PrintWithEnding,
                    Pos_UseLastPrice =(generalSettings.other_ActivatePriceLists&& generalSettings.PriceListType==(int)PriceListsType.LastPriceForCustomer),
                    CustomerDisplay_Active = generalSettings.CustomerDisplay_Active,
                    CustomerDisplay_CharNumber = generalSettings.CustomerDisplay_CharNumber,
                    CustomerDisplay_Code = generalSettings.CustomerDisplay_Code,
                    CustomerDisplay_DefaultWord = generalSettings.CustomerDisplay_DefaultWord,
                    CustomerDisplay_LinesNumber = generalSettings.CustomerDisplay_LinesNumber,
                    CustomerDisplay_PortNumber = generalSettings.CustomerDisplay_PortNumber,
                    CustomerDisplay_ScreenType = generalSettings.CustomerDisplay_ScreenType,
                },
                POSuserSettings = new UserSettings()
                {
                    posAddDiscount = userInfo.otherSettings.posAddDiscount,
                    posAllowCreditSales = userInfo.otherSettings.posAllowCreditSales,
                    posCashPayment = userInfo.otherSettings.posCashPayment,
                    posEditOtherPersonsInv = userInfo.otherSettings.posEditOtherPersonsInv,
                    posNetPayment = userInfo.otherSettings.posNetPayment,
                    posOtherPayment = userInfo.otherSettings.posOtherPayment,
                    posShowOtherPersonsInv = userInfo.otherSettings.posShowOtherPersonsInv,
                    posShowReportsOfOtherPersons = userInfo.otherSettings.salesShowReportsOfOtherPersons,

                },

                OtherSettings = new Other()
                {
                    Other_MergeItems = generalSettings.Other_MergeItems,
                    otherMergeItemMethod = generalSettings.otherMergeItemMethod,
                    Other_ItemsAutoCoding = generalSettings.Other_ItemsAutoCoding,
                    Other_ZeroPricesInItems = generalSettings.Other_ZeroPricesInItems,
                    Other_PrintSerials = generalSettings.Other_PrintSerials,
                    Other_AutoExtractExpireDate = generalSettings.Other_AutoExtractExpireDate,
                    Other_ViewStorePlace = generalSettings.Other_ViewStorePlace,
                    Other_ConfirmeSupplierPhone = generalSettings.Other_ConfirmeSupplierPhone,
                    Other_ConfirmeCustomerPhone = generalSettings.Other_ConfirmeCustomerPhone,
                    Other_DemandLimitNotification = generalSettings.Other_DemandLimitNotification,
                    Other_ExpireNotificationFlag = generalSettings.Other_ExpireNotificationFlag,
                    Other_ExpireNotificationValue = generalSettings.Other_ExpireNotificationValue,
                    Other_Decimals = generalSettings.Other_Decimals,
                    isActive_CustomerDisplay = generalSettings.CustomerDisplay_Active,
                    CustomerDisplayStartMessage = generalSettings.CustomerDisplay_DefaultWord,
                    Other_ShowBalanceOfPerson = generalSettings.Other_ShowBalanceOfPerson
                },

                VatSettings = new VAT()
                {
                    Vat_Active = generalSettings.Vat_Active,
                    Vat_DefaultValue = generalSettings.Vat_DefaultValue,
                }

            };
            if (defaultStore != null)
            {
                res.storeId = defaultStore.Id;
                res.storeCode = defaultStore.Code;
                res.storeAr = defaultStore.ArabicName;
                res.storeEn = defaultStore.LatinName;
            }

            return new ResponseResult { Data = res, Result = Result.Success };
        }
    }
}
