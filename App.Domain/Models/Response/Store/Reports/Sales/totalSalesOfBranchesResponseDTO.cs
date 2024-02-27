using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Sales
{
    public class totalSalesOfBranchesResponseDTO
    {
        public double TotalAmount { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalAfterDiscount { get; set; }
        public double TotalVat { get; set; }
        public double TotalNet { get; set; }
        public double totalPaid { get; set; }
        public List<totalSalesOfBranchesResponseList> data { get; set; }
    }
    public class totalSalesOfBranches
    {
        public string arabicName { get; set; }
        public string latinName { get; set; }
    }
    public class totalSalesOfBranchesResponseList
    {
        public int paymentMethodId { get; set; }
        public string paymentTypeAr { get; set; }
        public string paymentTypeEn { get; set; }
        public double amount { get; set; }
        public double discount { get; set; }
        public double totalAfterDiscount { get; set; }
        public double vat { get; set;}
        public double net { get; set;}
        public double paid { get; set; }
        public int branchId { get; set; }
        public totalSalesOfBranches branch { get; set; }
    }
    public class totalSalesOfBranchesResponse
    {
        public CompanyTotalSalesResponseDTO data { get; set; }
        public string notes { get; set; }
        public Result Result { get; set; }
        public int TotalCount { get; set; }
        public int dataCount { get; set; }
    }
}
