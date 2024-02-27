using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup
{
    public class GetAllItemsResponse
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public string ImagePath { get; set; }
        public int GroupId { get; set; }//Category Id
        //public string GroupNameArabic { get; set; }//Category name Arabic
        //public string GroupNameLatin { get; set; }//Category name Latin
        //public InvCategories GroupData { get; set; }
        public bool CanDelete { get; set; }

        public virtual CategoriesVM Category { get; set; }
        public int Status { get; set; }
        

    }
}
