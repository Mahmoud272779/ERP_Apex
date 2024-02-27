using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain
{
    public class ReportManger
    {

        public int  Id { get; set; }
        public int  screenId { get; set; }
        public int  ArabicFilenameId { get; set; }
        public bool IsArabic { get; set; }
        public int  Copies { get; set; }
        public ReportFiles Files { get; set; }
        public ScreenName ScreenNames { get; set; }
    }
}
