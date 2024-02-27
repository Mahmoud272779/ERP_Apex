using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class TotalTransactionOfItemsRequestDTO
    {
        [Required]
        public int storeId { get; set; }        
        public int? itemId { get; set; }
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }
        public int pageSize { get; set; } = 5;
        public int pageNumber { get; set; } = 1;

    }
}
