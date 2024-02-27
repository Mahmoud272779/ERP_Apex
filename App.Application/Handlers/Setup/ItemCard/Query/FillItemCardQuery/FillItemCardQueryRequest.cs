using App.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Setup.ItemCard.Query.FillItemCardQuery
{
    public class FillItemCardQueryRequest : IRequest<ResponseResult>
    {
        public string ItemCode { get; set; } // front send ItemId , barcode , itemcode or nationalBarcode 
        public int? UnitId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int? PersonId { get; set; }
        public int storeId { get; set; }
        public string ParentInvoiceType { get; set; } // to return quantity of item in store without this invoice
        public int? invoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public List<ExpiaryData> oldData { get; set; } = new List<ExpiaryData>();  // quantities that enterd in other records in the same invoice
        public List<CalcQuantityRequest> items { get; set; }
        public bool? serialRemovedInEdit { get; set; }
        public string? invoiceType { get; set; }
        public int? salesManId { get; set; }
        public int? PriceListId { get; set; } // قائمة الأسعار
    }
}
