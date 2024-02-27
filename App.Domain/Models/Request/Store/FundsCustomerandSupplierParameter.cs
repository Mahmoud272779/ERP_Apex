using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class FundsCustomerandSupplierParameter
    {
        public List<supAndCustUpdateFund> listOfPersonsFunds { get; set; }
        public DateTime date { get; set; }

    }
    public class supAndCustUpdateFund
    {
        public int Id { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
    }
    public class FundsCustomerandSupplierSearch : GeneralPageSizeParameter
    {
        public string? SearchCriteria { get; set; }
        public int Type { get; set; }
        
      
    }
}

