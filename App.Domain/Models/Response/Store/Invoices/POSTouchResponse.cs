using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain
{
    public class POSTouchResponse
    {
        public class CategoriesPOSTouchResponse
        {
            public int Id { get; set; }
            public string ArabicName { get; set; }
            public string LatinName { get; set; }
            public string ImagePath { get; set; }
        }
        public class ItemForIOSResponse
        {
            public int Id { get; set; }
            public int itemTypeId { get; set; }
            public string ArabicName { get; set; }
            public string LatinName { get; set; }
            public string ImagePath { get; set; }
            public bool ApplyVAT { get; set; }
            public double vatRatio { get; set; }
            public string ItemCode { get; set; }
            public object units { get; set; }
            public bool isBalanceBarcode { get; set; }
            public double itemQuantity
            {
                get;
                set;
                
            }
            public string balanceBarcode { get; set; }
            public List<ExpiaryData> expiaryData { get; set; } = new List<ExpiaryData>();
            public List<string> ExtractedSerials { get; set; }
            public List<string> existedSerials { get; set; }
            public List<string> listSerials { get; set; } = new List<string>();// برجعها للفرونت فى حالة عملت بحث بالسيريال 

        }

        public class UnitsForIOS
        {
            public int UnitId { get; set; }
            public string ArabicName { get; set; }
            public string LatinName { get; set; }
            public double ConversionFactor { get; set; }
            public double SalePrice1 { get; set; }

        }
    }
   
}
