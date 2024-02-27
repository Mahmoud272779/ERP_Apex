using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.GeneralLedger
{
    public class CostCenterReportRequest
    {

        public int? financialAccountId { get; set; }
        public int CostCenterID { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }


    }
}
