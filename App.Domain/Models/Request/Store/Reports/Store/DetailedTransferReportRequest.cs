using App.Domain.Enums;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class DetailedTransferReportRequest : PaginationVM
    {
        public int transferFrom { get; set; }
        public int transferTo { get; set; }
        public TransferStatusEnum status { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
    }
}
