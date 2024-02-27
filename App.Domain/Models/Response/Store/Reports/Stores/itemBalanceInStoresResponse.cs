using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Store.Reports.Stores
{
    public class itemBalanceInStoresResponse
    {
        public int storeId { get; set; }
        public string storeName { get; set; }
        public string  storeNameEn {get;set;}
        public string unitName { get; set; }
        public string unitNameEn { get; set; }
        public double balance { get; set; }
    }
}
