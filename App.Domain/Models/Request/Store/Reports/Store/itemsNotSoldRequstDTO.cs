using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class itemsNotSoldRequstDTO : GeneralPageSizeParameter
    {
        public int? itemId { get; set; }
        public int? catId { get; set; }
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }
        public string branches { get; set; }
    }
}
