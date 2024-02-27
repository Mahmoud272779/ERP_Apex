using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Store.Reports.Purchases
{
    public class VatStatmentResponse
    {
        public double  totalDebtor { get; set; }
        public double  totalCreaditor { get; set; }
        public double  totalBalance { get; set; }
        //for print
        public string totalBalaceData { get; set; }
        public List<VatStatmentList> VatStatmentResList { get; set; }
    }

    public class VatStatmentList    
    {
        public string Date { get; set; }
        public string InvoiceTypeAr { get; set; }
        public string InvoiceTypeEn { get; set; }
        public string rowClassName { get; set; }
        public string InvoiceCode { get; set; }
        public double Total { get; set; }
        public double totalAfterVat { get; set; }
        public double Debtor { get; set; }
        public double Creaditor { get; set; }
        public double balance { get; set; }
         //for print
        public string balanceListData { get; set; }

        public int? InvoiceId { get; set; }
        public int? InvoiceTypeId { get; set; }

        public int? pageId { get; set; }
        public int? pageEnum { get; set; }
        public int? DocTypeId { get; set; }
    }
}
