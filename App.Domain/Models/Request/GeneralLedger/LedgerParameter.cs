using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class LedgerParameter
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public LedgerSearchParameter Search { get; set; }
    }
    public class LedgerSearchParameter
    {
        public string  FinancialAcountCode { get; set; }
        public string FinancialAccountName { get; set; }
        public int? Currency { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
