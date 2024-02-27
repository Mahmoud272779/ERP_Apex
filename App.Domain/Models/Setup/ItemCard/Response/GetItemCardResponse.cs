using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Response
{
    public class GetItemCardResponse
    {
        public GetItemCardResponse()
        {
            StorePlace = new StorePlaceVM();
            Category = new CategoriesVM();
            Units = new List<UnitsVm>();
            Stores = new List<StoresVm>();
            ItemParts = new List<PartsVm>();
        }

        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string NationalBarcode { get; set; }
        public int TypeId { get; set; }
        public bool UsedInSales { get; set; }
        public int? DepositeUnit { get; set; }
        public int? WithdrawUnit { get; set; }
        public int? ReportUnit { get; set; }
        public double VAT { get; set; }
        public bool ApplyVAT { get; set; }
        public string Model { get; set; }
       
     
        public string Description { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int GroupId { get; set; }
       
        public int Status { get; set; }
        public byte[] Image { get; set; }
        public string ImagePath { get; set; }
        public bool CanEdit { get; set; }
        public virtual StorePlaceVM StorePlace { get; set; }

        public virtual CategoriesVM Category { get; set; }
        public virtual List<UnitsVm> Units { get; set; }
        public virtual List<StoresVm > Stores { get; set; }
        public virtual List<PartsVm> ItemParts { get; set; }

    }

    public class StorePlaceVM
    {
        public int? DefaultStoreId { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int StorePlaceActive { get; set; }

    }
    public class CategoriesVM {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int CategoryActive { get; set; }

    }
    public class UnitsVm
    {
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }

        public double ConversionFactor { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice1 { get; set; }
        public double SalePrice2 { get; set; }
        public double SalePrice3 { get; set; }
        public double SalePrice4 { get; set; }
        public string Barcode { get; set; }
        public bool CanEditUnit { get; set; }
        public int UnitActive { get; set; }


    }
    public class StoresVm
    {
        public int ItemId { get; set; }
        public int StoreId { get; set; }
        public int StoreCode { get; set; }
        public decimal DemandLimit { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int StoreActive { get; set; }

    }

    public class PartsVm
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        //  public int PartId { get; set; }
        public double Quantity { get; set; }
        public int UnitId { get; set; }
        public string UnitAr { get; set; }
        public string UnitEn { get; set; }
        public int ItemPartActive { get; set; }
        public int itemType { get; set; }

    }

}
