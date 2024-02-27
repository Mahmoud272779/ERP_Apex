using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class itemSalesForCustomersRequestDTO : GeneralPageSizeParameter
    {
        public int itemId { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public string branches { get; set; }
        public PaymentType paymentType { get; set; }
    }
}
