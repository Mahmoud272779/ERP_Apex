using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Store.Reports.Purchases
{
    public class SuppliersAccountResponse
    {
        public double totalPrevDebtor { get; set; }
        public double totalPrevCreditor { get; set; }
        public double totalTransDebtor { get; set; }
        public double totalTransCreditor { get; set; }
        public double totalBalanceDebtor { get; set; }
        public double totalBalanceCreditor { get; set; }
        public List<SuppliersAccountList> SuppliersAccountData { get; set; }
    }
    public class SuppliersAccountList
    {
        
        public int supplierId { get; set; }
        public string  supplierName { get; set; }
        public string supplierNameEn { get; set; }

        public double prevDebtor { get; set; }
        public double prevCreditor { get; set; }
        public double transDebtor { get; set; }
        public double transCreditor { get; set; }
        public double balanceDebtor { get; set; }
        public double balanceCreditor { get; set; }
        

    }
}
