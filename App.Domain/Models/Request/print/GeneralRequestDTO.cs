using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request
{
    public class GeneralRequestDTO
    {
        public int screenId { get; set; }
        public bool isArabic { get; set; }
        public int? reportId { get; set; }
      

    }
}
