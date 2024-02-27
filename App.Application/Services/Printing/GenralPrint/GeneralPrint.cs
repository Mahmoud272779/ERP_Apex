using App.Application.Services.Printing.GenralPrint;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Printing.PrintResponse;
using App.Domain.Models.Request.ReportFile;
using App.Domain.Models.Response.Store.Reports;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing
{
    public   class GeneralPrint :IGeneralPrint
    {
        // private readonly ICompanyDataService _CompanyDataService;
        // private readonly IPrintService _iprintService;
        private readonly ICreateDataTable _createDataTable;
        private readonly IFilesMangerService _filesMangerService;

       
        public GeneralPrint(IFilesMangerService filesMangerService,ICreateDataTable createDataTable)
        {

            // _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _createDataTable = createDataTable;
        }


        // List<object> obj is plus list case for paid transaction for invoies report
        public async Task<WebReport> PrintReport<T, T1, T2>(T Data, List<T1> FirstList, List<T2> SecondList, TablesNames tablesNames, object otherData, int screenId, exportType exportType, bool isArabic,int fileId=0, int closingStep = 0, bool isBarcode = false, List<object> obj = null)
        {
            var data = new DataToCreateDataTable<T, T1, T2>()
            {
                DataObjet = Data,
                FirstList = FirstList,
                SecondList = SecondList,
               
            };
            
            var tables = await _createDataTable.CreateDataTables(data, tablesNames, otherData, isBarcode,obj);

            var fileContents = new ReportFileRequest();
            if(isBarcode)
                fileContents = await _filesMangerService.GetBarcodePrintFile(screenId);

            else

                fileContents = await _filesMangerService.GetReportPrintFiles(screenId, isArabic, closingStep,fileId);

            return await RegisterDataInReportFile.Report(tables, fileContents.Files, exportType);


        }



    }
    public class DataToCreateDataTable<T,T1,T2>
    {
        public T DataObjet { get; set; }
        public List<T1> FirstList { get; set; }
        public List<T2> SecondList { get; set; }
       



    }
}
