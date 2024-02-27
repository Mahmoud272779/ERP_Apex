using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Stores
{
    public class ReviewWarehouseTransfers
    {
        public string documentId { get; set; }
        public string date { get; set; }
        public int status { get; set; }
        public string statusTypeAr { get; set; }
        public string statusTypeEn { get; set; }
        public string receivedDate { get; set; }
        public int? InvoiceTypeId { get; set; }
        public int? InvoiceId { get; set; }
        public int? pageId { get; set; }
        public int? pageEnum { get; set; }
        public int? DocTypeId { get; set; }
    }
    public class ReviewWarehouseTransfersResponse
    {
        public List<ReviewWarehouseTransfers> data { get; set; }
        public int dataCount { get; set; }
        public int totalCount { get; set; }
        public string Note { get; set; }
        public Result Result { get; set; }

    }
}
