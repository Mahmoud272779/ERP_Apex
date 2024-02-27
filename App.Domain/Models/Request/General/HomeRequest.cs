using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.General
{
    public class HomeRequest
    {
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
    }
}
