using App.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.calcQyt
{
    public class calcItemQuantityRequest : IRequest<QuantityInStoreAndInvoice>
    {
        public int? invoiceId { get; set; }
        public int ItemId { get; set; }
        public int? UnitId { get; set; }
        public int StoreId { get; set; }
        public string ParentInvoiceType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsExpiared { get; set; }
        public int? invoiceTypeId { get; set; }
        public DateTime invoiceDate { get; set; }
        public List<CalcQuantityRequest> items { get; set; }
        public int itemTypeId { get; set; }
        public double  currentQuantity { get; set; }
    }
}
