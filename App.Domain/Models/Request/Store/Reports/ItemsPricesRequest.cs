using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Reports
{
   public class ItemsPricesRequest : GeneralPageSizeParameter
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? CategoryId { get; set; }
        public int? Status { get; set; }
        public int? TypeId { get; set; }
    }
}
