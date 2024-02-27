using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing
{
    public static class RegisterDataInReportFile
    {
        public static async Task<WebReport> Report(List<DataTable> data, byte[] fileContents, exportType type)
        {
            WebReport webReport = new WebReport();
            //var path = Path.Combine("wwwroot", "Reports\\en", "DebtAgingForSuppliers" + ".frx");
            Stream stream = new MemoryStream(fileContents);

            webReport.Report.Load(stream);
            //webReport.Report.MaxPages = 501;


            foreach (var table in data)
            {

                webReport.Report.RegisterData(table, name: "Ref" + table.TableName);
            }

            return webReport;
        }
    }
}
