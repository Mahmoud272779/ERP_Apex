using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.Store.Reports.Sales
{
    public class ItemSalesRequestDto
    {
        public int? itemId { get; set; } = 0;
        [Required]
        public string branches { get; set; }
        public int employeeId { get; set; } = 0;
        [Required]
        public PaymentType paymentType { get; set; }
        public int? catId { get; set; } = 0;
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }

        public int PageSize { get; set; } = 5;
        public int PageNumber { get; set; } = 1;
    }
}
