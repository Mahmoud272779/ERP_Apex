using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class FinancialAccountForOpeningBalanceParameter
    {
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string AccountCode { get; set; }
        public string Notes { get; set; }
    }
    public class UpdateFinancialAccountForOpeningBalanceParameter
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string AccountCode { get; set; }
        public string Notes { get; set; }
    }
}
