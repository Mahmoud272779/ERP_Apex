using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store
{
    public class discountsResponseDTO
    {
        public int id { get; set; }
        public int code { get; set; }
        public string docNumber { get; set; }
        public string paperNumber { get; set; }
        public string docDate { get; set; }
        public bool isCustomer { get; set; }
        public int personId { get; set; }
        public double amountMoney { get; set; }
        public double totalAmountMoney { get; set; }
        public string notes { get; set; }
        public int docType { get; set; }
        public bool isDeleted { get; set; }
        public bool canDelete { get; set; }

        public string refrience { get; set; }
        public int branchId { get; set; }
        public object person { get; set; }
    }
}
