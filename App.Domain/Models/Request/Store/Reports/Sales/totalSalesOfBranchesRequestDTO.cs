using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.Store.Reports.Sales
{
    public class totalSalesOfBranchesRequestDTO
    {
        [Required]
        public PaymentType? paymentTypes { get; set; } = PaymentType.all;
        public int paymentMethodId { get; set; } = 0;
        [Required]
        public string branches { get; set; }
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 5;
      
    }
}
