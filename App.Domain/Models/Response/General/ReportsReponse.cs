using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.General
{
    public class ReportsReponse
    {
        public string FileURL { get; set; }
        public string FileName { get; set; }
        public string htmlPrint { get; set; }
        public string fileBase64 { get; set; }
        public bool isFireFox { get; set; }
        public Result Result { get; set; }
        public Result ResultForPrint { get; set; }
        public InvoiceData data { get; set; }
       public  bool isOverSize { get; set; }

    }
    public class _ResponseResult
    {
        public ReportsReponse result { get; set; }
    }
    public class InvoiceData
    {
        public double  net { get; set; }
        public double virualPaid { get; set; }
        public double remain { get; set; }
    }
}
