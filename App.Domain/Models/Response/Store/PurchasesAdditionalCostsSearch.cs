using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class PurchasesAdditionalCostsSearch : GeneralPageSizeParameter
    {
        public string SearchCriteria { get; set; }
        public int Status { get; set; }
    }
    public class PurchasesAdditionalCostsDto
    {
        public int PurchasesAdditionalCostsId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int AdditionalType { get; set; }
         public int Code { get; set; }
          public int Status { get; set; }
        public string Notes { get; set; }
         public bool CanDelete { get; set; }
        // public int Code { get; set; }
    }
}
