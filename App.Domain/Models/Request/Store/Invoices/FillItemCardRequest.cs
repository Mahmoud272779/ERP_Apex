using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;

namespace App.Domain.Models.Security.Authentication.Request.Invoices
{
    public class FillItemCardRequest//:IRequest<ResponseResult>
    {
       /* public FillItemCardRequest(string ItemCode, int? unitId, int invoiceType , int? PersonId, int storeId ,string ParentInvoiceType , int? invoiceId)
        {
            this.ItemCode = ItemCode;
            UnitId = unitId;
            InvoiceType = invoiceType;
            this.PersonId = PersonId;
            this.storeId = storeId;
            this.ParentInvoiceType = ParentInvoiceType;
            this.invoiceId = invoiceId;
        }*/

        public string ItemCode { get; set; } // front send ItemId , barcode , itemcode or nationalBarcode 
        public int? UnitId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int? PersonId { get; set; }
        public int storeId { get; set; }
        public string ParentInvoiceType { get; set; } // to return quantity of item in store without this invoice
        public int? invoiceId { get; set; }  
        public DateTime InvoiceDate { get; set; }
        public List<ExpiaryData> oldData { get; set; } = new List<ExpiaryData>();  // quantities that enterd in other records in the same invoice
        public List<CalcQuantityRequest> items  { get; set; }
    }
}
