using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store.Invoices
{
    public class PosDto
    {
        public int LastCode;
        public int nextCode;
        public int storeId;
        public int storeCode;
        public string storeAr;
        public string storeEn;
        public int customerId;
        public int customerCode;
        public string customerAr;
        public string customerEn;
        public GeneralSettings POSgeneralSettingsint { get; set; }
        public UserSettings POSuserSettings { get; set; }
        public Other OtherSettings { get; set; }
        public VAT VatSettings { get; set; }
    }

    public class POSInvoiceDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceType { get; set; }
        public string BookIndex { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double TotalPrice { get; set; }
        public int InvoiceTypeId { get; set; }
        public int InvoiceSubTypesId { get; set; } // enum of invoice sub Types for main types in InvoiceTypeId
        public int PaymentType { get; set; }
        public bool CanDelete { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int StoreId { get; set; }
        public string StoreNameAr { get; set; }
        public string StoreNameEn { get; set; }
        public int? PersonId { get; set; }
        public string PersonNameAr { get; set; }
        public string PersonNameEn { get; set; }
        public int PersonStatus { get; set; }
        public string ParentInvoiceCode { get; set; }
        public bool IsAccredited { get; set; }
        public int ItemsQuantity { get; set; }
        public double Discount { get; set; }
    }

    public class GeneralSettings
    {
        public bool Pos_ModifyPrices { get; set; }
        public int Pos_ModifyPricesType { get; set; }
        public bool Pos_ExceedDiscountRatio { get; set; }
        public bool Pos_UseLastPrice { get; set; }
        public bool Pos_ActivePricesList { get; set; }
        public bool Pos_ExtractWithoutQuantity { get; set; }
        public bool Pos_PriceIncludeVat { get; set; }
        public bool Pos_ActiveDiscount { get; set; }
        public bool Pos_DeferredSale { get; set; }
        public bool Pos_IndividualCoding { get; set; }
        public bool Pos_PreventEditingRecieptFlag { get; set; }
        public int Pos_PreventEditingRecieptValue { get; set; }
        public bool Pos_ActiveCashierCustody { get; set; }
        public bool Pos_PrintPreview { get; set; }
        public bool Pos_PrintWithEnding { get; set; }
        public bool Pos_EditingOnDate { get; set; }
        public bool CustomerDisplay_Active { get; set; }
        public string CustomerDisplay_PortNumber { get; set; }
        public int CustomerDisplay_Code { get; set; }
        public int CustomerDisplay_LinesNumber { get; set; }
        public int CustomerDisplay_CharNumber { get; set; }
        public string CustomerDisplay_DefaultWord { get; set; }
        public int CustomerDisplay_ScreenType { get; set; }
    }
    public class UserSettings
    {
        //POS
        public bool posAddDiscount { get; set; }
        public bool posAllowCreditSales { get; set; }
        public bool posEditOtherPersonsInv { get; set; }
        public bool posShowOtherPersonsInv { get; set; }
        public bool posShowReportsOfOtherPersons { get; set; }
        //POS Payments
        public bool posCashPayment { get; set; }
        public bool posNetPayment { get; set; }
        public bool posOtherPayment { get; set; }
    }
    public class Other
    {
        public bool Other_MergeItems { get; set; }
        public string otherMergeItemMethod { get; set; }
        public bool Other_ItemsAutoCoding { get; set; }
        public bool Other_ZeroPricesInItems { get; set; }
        public bool Other_PrintSerials { get; set; }
        public bool Other_AutoExtractExpireDate { get; set; }
        public bool Other_ViewStorePlace { get; set; }
        public bool Other_ConfirmeSupplierPhone { get; set; }
        public bool Other_ConfirmeCustomerPhone { get; set; }
        public bool Other_DemandLimitNotification { get; set; }
        public bool Other_ExpireNotificationFlag { get; set; }
        public int Other_ExpireNotificationValue { get; set; }
        public int Other_Decimals { get; set; }
        public bool isActive_CustomerDisplay { get; set; }
        public string CustomerDisplayStartMessage { get; set; }
        public bool Other_ShowBalanceOfPerson { get; set; }
    }
    public class VAT
    {
        public bool Vat_Active { get; set; }
        public double Vat_DefaultValue { get; set; }
    }
}
