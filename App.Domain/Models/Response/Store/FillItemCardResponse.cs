using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class FillItemCardResponse
    {
        public FillItemCardResponse()
        {
            Item = new itemMaster();
            expiaryData = new List<ExpiaryData>();
            Unit = new unitData();
        }
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public double ConversionFactor { get; set; }
        public double Price { get; set; }
    //    public double PurchasePrice { get; set; }
   //     public double SalePrice { get; set; }
        public bool ApplyVat { get; set; }
        public double  Vat { get; set; }
        public double AutoDiscount { get; set; } // خصومات ع الاصناف فى اللمبيعات
        //  public decimal SalePrice1 { get; set; }
        //  public decimal SalePrice2 { get; set; }
        //  public decimal SalePrice3 { get; set; }
        //  public decimal SalePrice4 { get; set; }
        //    public string Barcode { get; set; }
        public virtual itemMaster Item { get; set; }
        public bool QuantityAvailable { get; set; }
        public bool isBalanceBarcode { get; set; } = false;// هل الصنف باركود ميزان او لا 
        public double  QuantityInStore { get; set; }
        public double  itemQuantity { get; set; } //فى حاله الباركود الميزان هنضيف الكميه للصنف
        public double  itemCost { get; set; } //فى حاله الباركود الميزان هنضيف التكلفه للصنف
        public double StoreQuantityWithOutInvoice { get; set; }
        public List<string> ExtractedSerials { get; set; }
        public List<string> existedSerials { get; set; }
        public List<string> listSerials { get; set; } = new List<string>();// برجعها للفرونت فى حالة عملت بحث بالسيريال 
        public List<ExpiaryData> expiaryData { get; set; } = new List<ExpiaryData>();
        public string balanceBarcode { get; set; }

        public virtual unitData Unit { get; set; }
        public string ImagePath { get; set; }


    }
    public class ExpiaryData
    {
        public string expiaryOfInvoice { get; set; }
        public double QuantityOfDate { get; set; }
        public double price { get; set; }
        public int conversionFactor { get; set; }
        public double discountValue { get; set; }
        public double totalPrice { get; set; }

    }

    public class itemMaster
    {
        public string ItemCode { get; set; }
        public int TypeId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }

    }
    public class unitData
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
    }
    public class ItemAllData
    {
        public IQueryable<InvStpItemCardMaster> itemMaster { get; set; }
        public List<InvStpItemCardUnit> itemData { get; set; }
    }
}
