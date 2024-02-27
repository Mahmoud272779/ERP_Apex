using App.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class InvGeneralSettings : AuditableEntity
    {
        public int Id { get; set; }
        public bool Purchases_ModifyPrices { get; set; }
        public bool Purchases_PayTotalNet { get; set; }
        public bool Purchases_UseLastPrice { get; set; }
        public bool Purchases_PriceIncludeVat { get; set; }
        public bool Purchases_PrintWithSave { get; set; }
        public bool Purchases_ReturnWithoutQuantity { get; set; }
        public bool Purchases_ActiveDiscount { get; set; }
        public bool Purchase_UpdateItemsPricesAfterInvoice { get; set; } = false;
        public bool Pos_ModifyPrices { get; set; }
        public int Pos_ModifyPricesType { get; set; } // get from enum-> ModifySalePricesType
        public bool Pos_ExceedDiscountRatio { get; set; }
       // public bool Pos_UseLastPrice { get; set; }
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

        public bool Sales_ModifyPrices { get; set; }
        public int Sales_ModifyPricesType { get; set; }// get from enum-> ModifySalePricesType
        public bool Sales_ExceedDiscountRatio { get; set; }
        public bool Sales_PayTotalNet { get; set; }
     //   public bool Sales_UseLastPrice { get; set; }
        public bool Sales_ExtractWithoutQuantity { get; set; }
        public bool Sales_PriceIncludeVat { get; set; }
        public bool Sales_PrintWithSave { get; set; }
        public bool Sales_ActiveDiscount { get; set; }
        public bool Sales_LinkRepresentCustomer { get; set; }
        public bool Sales_ActivePricesList { get; set; }
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
         //Extract From person Balance
        public bool other_AlertMessageForCustomerSupplierBalance { get; set; }
        public bool other_AutoExtractFromCustomerSupplierBalance { get; set; }
        public bool other_DoNothingForCustomerSupplierBalance { get; set; } = true;


        public bool Other_ShowBalanceOfPerson { get; set; } 
        public bool Other_UseRoundNumber { get; set; } = false;//up
        public bool Funds_Items { get; set; }
        public bool Funds_Supplires { get; set; }
        public DateTime? Funds_Supplires_Date { get; set; }
        public bool Funds_Customers { get; set; }
        public DateTime? Funds_Customers_Date { get; set; }

        public bool other_ActivatePriceLists { get; set; }
        public int PriceListType { get; set; }
        //alaa khaled
        public bool Funds_Safes { get; set; }
        public bool Funds_Banks { get; set; }
        public string barcodeType { get; set; }
        public bool Barcode_ItemCodestart { get; set; }
        public bool Vat_Active { get; set; }
        public double Vat_DefaultValue { get; set; }
        public DateTime Accredite_StartPeriod { get; set; }
        public DateTime Accredite_EndPeriod { get; set; }

        public bool CustomerDisplay_Active { get; set; }

        public string CustomerDisplay_PortNumber { get; set; }
        public int CustomerDisplay_Code { get; set; }
        public int CustomerDisplay_LinesNumber { get; set; }
        public int CustomerDisplay_CharNumber { get; set; }
        public string CustomerDisplay_DefaultWord { get; set; }
        public int CustomerDisplay_ScreenType { get; set; }

        //Email Settings 
        public string Email { get; set; } = "";
        public string EmailPassword { get; set; } = "";
        public string EmailDisplayName { get; set; } = "";
        public string EmailHost { get; set; } = "";
        public int EmailPort { get; set; } = 0;
        public int secureSocketOptions { get; set; } = 1;

        //public bool autoLogout { get; set; } = false;
        public int autoLogoutInMints { get; set; } = 15;
        public int UpdateFilesNumber { get; set; }
        public int SystemUpdateNumber { get; set; }
        // Electronic Invoice 
        public string OTP { get; set; }  // OnTime Passowrd
        public bool ActiveElectronicInvoice { get; set; }=false;
        public bool? isExpenses { get; set; } = false;// مصروفات


    }
}
