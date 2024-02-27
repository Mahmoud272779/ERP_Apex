using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.SetQuantityForExpiaryDate
{
    public class setQuantityForExpiaryDateRequest : IRequest<ResponseResult>
    {
        public int invoiceId { get; set; }
        public int itemId { get; set; }
        public int unitId { get; set; }
        public int storeId { get; set; }
        public double quantity { get; set; } // current quantity
        public DateTime? EditedDate { get; set; }
        public int invoiceTypeId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double price { get; set; }
        public double discountValue { get; set; }
        public List<ExpiaryData> oldData { get; set; }  // quantities that enterd in other records in the same invoice
    }
}
