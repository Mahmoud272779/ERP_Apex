using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models
{
    
    public  class itemsRequest
    {
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public int StoreId { get; set; }
        public string? ParentInvoiceType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsExpiared { get; set; }
        public int invoiceId { get; set; }
        public DateTime invoiceDate { get; set; }
        public int? invoiceTypeId { get; set; }
        public double currentQuantity { get; set; }
        public List<CalcQuantityRequest> currentItems { get; set; }
    }
    public class CalcQuantityRequest
    {
        public int itemId { get; set; }
        public double conversionFactor { get; set; }
        public double enteredQuantity { get; set; }
        public int itemTypeId { get; set; }
        public DateTime? enteredExpiryDate { get; set; }
    }


}
