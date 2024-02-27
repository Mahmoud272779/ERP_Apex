using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.ReportExporting
{
    public class FastReportRequestDto
    {
        public string fpxPath { get; set; }
        public string fileName { get; set; }
        public int exportingType { get; set; }
    }
}
