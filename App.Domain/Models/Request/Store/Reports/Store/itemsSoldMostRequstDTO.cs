using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class itemsSoldMostRequstDTO : GeneralPageSizeParameter
    {
        [Required]
        public itemSoldMostEnum itemSoldMostEnum { get; set; }
        [Required]
        public int searchValue { get; set; }
        public int? itemId { get; set; }
        public int? catId { get; set; }
        public string branches { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }

    }
    public enum itemSoldMostEnum
    {   
        qyt =1,
        avgPrice = 2,
        cost = 3
    }
}
