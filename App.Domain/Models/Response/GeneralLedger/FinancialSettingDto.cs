using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class FinancialSettingDto
    {
        public bool IsAssumption { get; set; }
        public int FinancialAccountId { get; set; }
        public bool UseFinancialAccount { get; set; }
        public bool AddUnderFinancialAccount { get; set; }
    }
}
