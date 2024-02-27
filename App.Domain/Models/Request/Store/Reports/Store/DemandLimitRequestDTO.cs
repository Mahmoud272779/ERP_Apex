using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class DemandLimitRequestDTO
    {
        [Required]
        public int storeId { get; set; }
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 5;
    }
}
