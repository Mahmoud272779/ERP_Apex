using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing
{
    public static class ConvertReportToBytes
    {
        public static byte[] ConvertReport(IWebHostEnvironment webHostEnvironment, string reportName, bool isArabic,bool IsBarcode=false)
        {
            string folder = "Reports";
            if (isArabic)
                folder = folder + "\\ar";
            else
                folder = folder + "\\en";
            var path = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot",IsBarcode? "BarcodeFiles" : folder, reportName + ".frx");     
            byte[] arrbytes = File.ReadAllBytes(path);
            return arrbytes;
        }
    }
}
