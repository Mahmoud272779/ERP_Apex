using App.Domain.Models.Request.Store.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Response.Store.Reports.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store.Reports.Purchases
{
    public class TotalVatDataResponse
    {
        public double TotalPriceWithoutVat { get; set; }
        public double TotalCreditor { get; set; }
        public double TotalDebitor { get; set; }
        public double TotalBalance { get; set; }

        public string TotalBalances { get; set; }
        public HashSet<TotalInvoicesVatList> TotalInvoiceList { get; set; }
    }
    public class InvoicesVatList
    {
        public int InvoiceType { get; set; }
        public int BranchId { get; set; }
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }

        public DateTime? Date { get; set; }
        public string InvoiceTypeAr { get; set; }
        public string InvoiceTypeEn { get; set; }

        public double totalHasVat { get; set; }
        public double Creditor { get; set; } // الدائن
        public double Debitor { get; set; } // المدين
        public double Balance { get; set; }
        public string rowClassName { get; set; }

        public int InvoiceId { get; set; }

    }
    public class TotalInvoicesVatList
    {
        public int TypeId { get; set; }
        public int BranchId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public double TotalPriceWithoutVat { get; set; }
        public double Creditor { get; set; } // الدائن
        public double Debitor { get; set; } // المدين
        public double Balance { get; set; }
        public string Balaces { get; set; }
        public int Level { get; set; }
        public bool isExpanded { get; set; } = false;



    }
}
