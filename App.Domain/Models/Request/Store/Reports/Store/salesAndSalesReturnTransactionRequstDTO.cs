using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class salesAndSalesReturnTransactionRequstDTO : GeneralPageSizeParameter
    {
        public int? itemId { get; set; }
        public PaymentType paymentTypes { get; set; }
        public salesType salesType { get; set; }
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }
    }
}
