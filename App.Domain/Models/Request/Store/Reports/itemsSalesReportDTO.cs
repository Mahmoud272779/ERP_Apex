using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store.Reports
{
    public class itemSalesReportDTO
    {
        [Required]
        public int itemId { get; set; }
        [Required]
        public int unitId { get; set; }
        [Required]
        public int pageNumber { get; set; }
        [Required]
        public int pageSize { get; set; }

    }
}
