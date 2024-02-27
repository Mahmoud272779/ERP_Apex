using App.Domain.Common;
using App.Domain.Entities.Process.General_Ledger;
using App.Domain.Entities.Process.Restaurants;
using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
   public class InvCategories 
    {
        public int Id { get; set; }
        public int Code { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public double VatValue { get; set; } = 0;
        public string Color { get; set; }
        //مجموعات الأصناف
        public int Status { get; set; }//Represent the status of the Category 1 if active 2 if inactive
        public int UsedInSales { get; set; }
        public string Notes { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public bool CanDelete { get; set; }
        public DateTime UTime { get; set; }
        public int CategoryType { get; set; } = 1; // Represent the Type of Category 1 - Inventory 2 - Kitchen
        public int? kitchenId { get; set; }
        public virtual Kitchens kitchens { get; set; }
        public ICollection<CategoriesPrinters> rstCategoriesPrinters { get; set; }
        public virtual ICollection<InvStpItemCardMaster> Items { get; set; }
    }
}
