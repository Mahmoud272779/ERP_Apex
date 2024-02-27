using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Profit.CalculatProfit
{
    internal interface IPCS
    {
        public List<InvoicesData> calculateItemsProfite(ProfiteRequest parameter);
    }
}
