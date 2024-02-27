using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLFinancialAccountForOpeningBalance: GeneralProperities
    {
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string Notes { get; set; }
        public string AccountCode { get; set; }
        public DateTime Date { get; set; }

    }
}
