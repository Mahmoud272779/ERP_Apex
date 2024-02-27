using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Sales
{
    public class itemSalesForCustomersResponseDTO
    {
        public double totalQyt { get; set; }
        public double totalAvgPrice{ get; set; }

        public double totalTotal { get; set; }
        public double totalDiscount { get; set; }
        public double totalVat { get; set; }
        public double totalNet { get; set; }
        public List<itemSalesForCustomersResponseList> itemSalesForCustomersResponseLists { get; set; }
    }
    public class itemSalesForCustomersResponseList
    {
        public int PersonCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string paymentMethodAr { get; set; }
        public string paymentMethodEn { get; set; }
        public double qyt { get; set; }
        public double avgPrice { get; set; }
        public double total { get; set; }
        public double discount { get; set; }
        public double vat { get; set; }
        public double net { get; set; }
    }
    public class itemSalesForCustomersResponse
    {
        public itemSalesForCustomersResponseDTO data { get; set; }
        public string notes { get; set; }
        public int TotalCount { get; set; }
        public Result Result { get; set; }
        public int dataCount { get; set; }
    }

}
