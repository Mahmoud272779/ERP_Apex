using App.Domain.Entities.Setup;
using Microsoft.AspNetCore.Routing.Constraints;

namespace App.Domain.Models.Request
{
    public class ProfiteRequest
    {
        public List<InvoicesData> Invoices { get; set; }
        public lastInvoiceEditedDTO lastDataDTO { get; set; }


        public List<InvStpItemCardUnit> ItemDataDTO { get; set; }
    }
    public class SelectionData
    {
        public double AvgPrice { get; set; }
        public int invoiceId { get; set; }
        public double Cost { get; set; }
    }
    public class CompositeForDataProfit
    {
        public double AvgPrice { get; set; }
        public double Cost { get; set; }
        public int ItemId { get; set; }//composite item id
        public int PartId { get; set; } //component item id  of composite item
        public double Quantity { get; set; }
        public int UnitId { get; set; }
        public double Factor { get; set; }
    }
    public class lastInvoiceEditedDTO
    {
        public double LastQTY { get; set; }
        public double QTyOfPurchase { get; set; }
        public double AvgPrice { get; set; }
        public int invoiceId { get; set; }
        public double Cost { get; set; }
    }
    public class InvoicesData
    {
        public int InvoiceId { get; set; }
        public double Serialize { get; set; }
        public double QTY { get; set; }
        public double factor { get; set; }
        public int ItemId { get; set; }
        public int? SizeId { get; set; }
        public int ItemIndex { get; set; }
        public double Price { get; set; }
        public double AvgPrice { get; set; }
        public double Cost { get; set; }
        public int InvoiceType { get; set; }
        public double VatRatio { get; set; }
        public bool PriceWithVate { get; set; }

    }

    public class JEnteryInvoicedata
    {

        public double cost { get; set; }
        public int invoiceId { get; set; }
        public double serial { get; set; }
        public int invoiceType { get; set; }
        public DateTime lastCalcProfit { get; set; }
    }


        public class porgressData
    {
        public int Count { get; set; }
        public int totalCount { get; set; }
        public double percentage { get; set; }
        public int status { get; set; }
        public string Notes { get; set; }
    }

    public class CompositeData
    {
        public int itemId { get; set; }
        public int size { get; set; }
        public double serialize { get; set; }
        public int  branchId { get; set; }
    }
}
