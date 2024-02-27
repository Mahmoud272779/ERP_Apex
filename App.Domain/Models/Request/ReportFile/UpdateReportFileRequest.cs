using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.ReportFile
{
    public class UpdateReportFileRequest
    {
        public int Id { get; set; }
        public string ReportFileName { get; set; } //Arabic or english
        public bool IsArabic { get; set; } //ar ,en 
       
       
    }
}
