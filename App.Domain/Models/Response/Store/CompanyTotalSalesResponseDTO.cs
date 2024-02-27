using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store
{
    public class CompanyTotalSalesResponseDTO
    {
        public double Totalamount { get; set; }
        public double Totaldiscount { get; set; }
        public double Totalnet { get; set; }
        public double Totalvat { get; set; }
        public double Totaldue { get; set; }
        public double Totalpaied { get; set; }
        public List<CompanyTotalSalesBranches> data { get; set; }
    }
    public class CompanyTotalSalesBranches
    {
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string paymentTypeAr { get; set; }
        public string paymentTypeEn { get; set; }
        public double amount { get; set; }
        public double discount { get; set; }
        public double net { get; set; }
        public double vat { get; set; }
        public double due { get; set; }
        public double paied { get; set; }
        public bool isExpanded { get; set; }
        public int Id { get; set; }
        public List<CompanyTotalSalesResponseBranches_Detalies> detalies { get; set; }
    }
    public class CompanyTotalSalesResponseBranches_Detalies
    {
        public string paymentTypeAr { get; set; }
        public string paymentTypeEn { get; set; }
        public double amount { get; set; }
        public double discount { get; set; }
        public double net { get; set; }
        public double vat { get; set; }
        public double due { get; set; }
        public double paied { get; set; }
    }
}
