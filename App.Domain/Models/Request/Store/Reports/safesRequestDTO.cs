using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.Store.Reports
{
    public class safesRequestDTO
    {
        [Required]
        public int id { get; set; }
        [Required]
        public bool isSafe { get; set; } = false;
        [Required]
        public string branches { get; set; }
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }
        [Required]
        public PaymentMethod paymentMethod { get; set; }
        [Required]
        public int pageNumber { get; set; } = 1;
        [Required]
        public int pageSize { get; set; } = 5;

    }
}
