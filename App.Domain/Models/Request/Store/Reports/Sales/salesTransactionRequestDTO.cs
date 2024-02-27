using App.Domain.Models.Response.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.Store.Reports.Sales
{
    public class salesTransactionRequestDTO
    {
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }
        [Required]
        public string branches { get; set; }
        [Required]
        public int pageNumber { get; set; } = 1;
        [Required]
        public int pageSize { get; set; } = 5;
        [Required]
        public PaymentType paymentTypes { get; set; }
        [Required]
        public invoiceTypes invoiceTypes { get; set; }
        public int salesmanId { get; set; } = 0;
        public bool isSales { get; set; } = true;
        public int personId { get; set; } = 0;
        public bool? isExpenses { get; set; } = false;// مصروفات

    }
}
