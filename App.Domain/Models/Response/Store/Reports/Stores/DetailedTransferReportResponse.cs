using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Stores
{
    public class DetailedTransferReport
    {
        public string date { get; set; }
        public string itemCode { get; set; }
        public string transferCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public int status { get; set; }
        public string statusAr { get; set; }
        public string statusEn { get; set; }
        public string unitAr { get; set; }
        public string unitEn { get; set; }
        public double transferedQyt { get; set; }
        public double acceptedQyt { get; set; }
        public double price { get; set; }
        public double totalPrice { get; set; }
        public string outgoingSerials { get; set; }
        public string incomingSerials { get; set; }
        public int? InvoiceId { get; set; }

        public int? InvoiceTypeId { get; set; }
        public int? pageId { get; set; }
        public int? pageEnum { get; set; }
        public int? DocTypeId { get; set; }
    }
    public class DetailedTransferReportResponse
    {
        public List<DetailedTransferReport> data { get; set; }
        public int totalCount { get; set; }
        public int dataCount { get; set; }
        public string Note { get; set; }
        public Result Result { get; set; }
    }
}
