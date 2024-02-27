using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
   public class ItemCardDataOfBarcodeResponse
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemNameAr { get; set; }
        public string ItemNameEn { get; set; }

        //public int UnitId { get; set; }
        //public string UnitNameAr { get; set; }
        //public string UnitNameEn { get; set; }

        public List<unitsData> unitsList { get; set; } = new List<unitsData>();
        public int ItemType { get; set; }
        public DateTime ExpireDate { get; set; }
      //  public double Price { get; set; }
        public double Vat { get; set; }
     //   public string Barcode { get; set; }

        public int CategoryId { get; set; }
        public string CategoryNameAr { get; set; }
        public string CategoryNameEn { get; set; }
        public string CompanyNameAr { get; set; }
        public string CompanyNameEn { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
       

    }

    public class unitsData
    {
        public int unitId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public double Price { get; set; }
        public string Barcode { get; set; }

    }
}
