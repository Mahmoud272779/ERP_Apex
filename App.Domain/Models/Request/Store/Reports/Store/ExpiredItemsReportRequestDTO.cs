using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class ExpiredItemsReportRequestDTO
    {
        public int pageSize { get; set; } = 5;
        public int pageNumber { get; set; } = 1;
        public int storeId { get; set; }
        public int NumberOfDays { get; set; }
    }

    public class ExpiredItemsReportForPrintRequestDTO
    {
        public int storeId { get; set; }
        public int NumberOfDays { get; set; }
    }
}
