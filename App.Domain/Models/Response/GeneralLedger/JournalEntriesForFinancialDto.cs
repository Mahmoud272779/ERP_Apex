using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class JournalEntriesForFinancialDto
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Time { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public double BalanceAfterOperation { get; set; }
        public int FA_Nature { get; set; }
    }
}
