using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain
{
    public class ReportFiles
    {

        public int  Id { get; set; }
        public string  ReportFileName { get; set; } // latine name
        public string ReportFileNameAr { get; set; } //Arabic name

        public bool IsArabic { get; set; } //ar ,en 
        public int  IsReport { get; set; }
        public byte[]  Files { get; set; }
        public bool IsDefault { get; set; }
        public DateTime uTime { get; set; }
        public ICollection<ReportManger> reportmanger { get; set; }
        
    }
}
