using App.Application.Helpers.Service_helper.FileHandler;
using FastReport.Export.Html;
using System.IO;
using Microsoft.Extensions.Configuration;
using App.Application.Services.Printing.InvoicePrint;
using SelectPdf;
using FastReport.Export.PdfSimple;
using App.Domain.Models.Request.ReportExporting;
using Newtonsoft.Json;
using System.Threading;
using FastReport.Export.Image;
using System.IO.Compression;
using Microsoft.Net.Http.Headers;
using App.Infrastructure;

namespace App.Application.Services.Printing.PrintFile
{



    public class PrintFileService : BaseClass, IprintFileService
    {
        private readonly IFileHandler _fileHandler;
        private readonly IPrintService _iPrintService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;


        public PrintFileService(IFileHandler fileHandler, IPrintService iPrintService, IHttpContextAccessor _httpContext
, IConfiguration configuration, IHttpContextAccessor httpContext) : base(_httpContext)
        {
            _fileHandler = fileHandler;
            _iPrintService = iPrintService;
            this.httpContext = _httpContext;
            _configuration = configuration;
            this._httpContext = httpContext;
        }


        public async Task<ReportsReponse> PrintFile(WebReport webReport, string SavedFileName, exportType _exportType, bool isPOS = false,bool barcode = false)
        {


            //webReport.Report.Prepare();


            if (barcode)
            {
                var res = htmlReturn(webReport, SavedFileName, false);
                return await res;
            }
            if (_exportType == exportType.Print)
            {
                _exportType = exportType.ExportToPdf;
            }

            else if (_exportType == exportType.ExportToImage)
            {
                return await imageExport(webReport, SavedFileName);
            }
            


            string extension = Extension(_exportType);
            string PreparedFilePath = Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], SavedFileName + "-" + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".fpx");
            var fileName = SavedFileName + "-" + DateTime.Now.ToString("yyyyMMddTHHmmss") + extension;
            string ReportName = Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], fileName);
            webReport.Report.Prepare();

            webReport.Report.SavePrepared(PreparedFilePath);
            FastReportRequestDto req = new FastReportRequestDto
            {
                fpxPath = PreparedFilePath,
                exportingType = (int)_exportType,
                fileName = ReportName
            };
            var reqObj = JsonConvert.SerializeObject(req);
            var TCPResponse = TcpListenerServices.Send(reqObj, _configuration);
             
            //if(TCPResponse != "1")
            //{
            //    TCPResponse = TcpListenerServices.PrintExporting(reqObj, _configuration);
            //}

            if (TCPResponse == "1")
            {
                if (File.Exists(ReportName))
                {
                    PrintHelper.DeleteFileBackground(ReportName, 1000);
                    return new ReportsReponse
                    {
                        FileURL = Path.Combine(_configuration["ApplicationSetting:FileURL"], fileName),
                        FileName = fileName,
                        Result = Result.Success,
                        isOverSize = false
                    };
                }
                else
                {
                    File.Delete(PreparedFilePath);
                    return new ReportsReponse
                    {
                        Result = Result.Failed
                    };
                }
            }
            else
            {
                File.Delete(PreparedFilePath);
                return new ReportsReponse
                {
                    Result = Result.Failed
                };
            }
        }

        public async Task<ReportsReponse> simplePDF(WebReport webReport, string SavedFileName)
        {
            webReport.Report.Prepare();

            //var hTMLExport = await getHtmlReport(webReport, true);

            //MemoryStream hrmlMs = new MemoryStream();
            //hTMLExport.Export(webReport.Report, hrmlMs);

            //hrmlMs.Flush();


            //var htmlFile = Encoding.UTF8.GetString(hrmlMs.ToArray()).Replace("page-break-after: always;", "page-break-after: inherit;");

            MemoryStream ms = new MemoryStream();
            var pdf = new PDFSimpleExport();
           
            //pdf.ImageDpi = 300;
            //pdf.JpegQuality = 600;
            // pdf.OpenAfterExport = true;
            // pdf.AllowOpenAfter = true;
            pdf.AllowOpenAfter = true;
            
            webReport.Report.Export(pdf, ms);
            
            //ms.Position = 0;




            string fileName = SavedFileName + "-" + DateTime.Now.ToString("yyyyMMddTHHmmss");
            ms.Flush();
            var file = _fileHandler.CreateInvoiceForPrint(ms.ToArray(), fileName, fileExt: "pdf");
            PrintHelper.DeleteFileBackground(file.Item2, 1000);
            return new ReportsReponse()
            {
                FileURL = file.Item1,
                FileName = file.Item3,
                Result = !string.IsNullOrEmpty(file.Item3) ? Result.Success : Result.Failed
            };
        }
        public async Task<ReportsReponse> pdfReturn(WebReport webReport, string SavedFileName)
        {

            //return await htmlReturn(webReport, SavedFileName);


            var hTMLExport = await getHtmlReport(webReport);
            var isLandscape = webReport.Report.ReportResourceString.Contains("Landscape=\"true\"");



            string extension = "pdf";




            MemoryStream ms = new MemoryStream();
            hTMLExport.Export(webReport.Report, ms);
            ms.Flush();

            var HtmlContant = Encoding.UTF8.GetString(ms.ToArray()).Replace("page-break-after: always;", "page-break-after: inherit;");



            HtmlToPdf file = new HtmlToPdf(1, 1);
            if (isLandscape)
                file.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            file.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
            file.Options.PdfPageSize = PdfPageSize.A4;

            file.Options.DrawBackground = false;
            //file.Options.ViewerPreferences.PageLayout = PdfViewerPageLayout.TwoColumnLeft;
            file.Options.ViewerPreferences.NonFullScreenPageMode = PdfViewerFullScreenExitMode.UseNone;
            file.Options.ViewerPreferences.PageMode = PdfViewerPageMode.UseNone;
            var f = file.ConvertHtmlString(HtmlContant);

            // f.DocumentInformatio;
            //var ff= f.DocumentInformation.;
            //f.ViewerPreferences.NonFullScreenPageMode=PdfViewerFullScreenExitMode.UseNone;
            f.ViewerPreferences.PageMode = PdfViewerPageMode.UseNone;
            f.ViewerPreferences.HideWindowUI = true;
            f.ViewerPreferences.HideMenuBar = true;
            f.ViewerPreferences.HideToolbar = true;
            var y = f.ToString().Contains("Demo");
            var t = f.ViewerPreferences.PageLayout.ToString().Contains("Demo Version - Select.Pdf SDK");


            var stream = new MemoryStream();
            f.Save(stream);





            var fileName = SavedFileName + "-" + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".pdf";
            FileStream fs = new FileStream(Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], fileName), FileMode.Create);

            fs.Write(stream.ToArray(), 0, stream.ToArray().Length);
            fs.Close();
            var fileURL = _configuration["ApplicationSetting:FileURL"] + "/" + fileName;
            return new ReportsReponse()
            {
                FileURL = fileURL,
                FileName = fileName,
                Result = !string.IsNullOrEmpty(fileName) ? Result.Success : Result.Failed
            };
        }
        private async Task<HTMLExport> getHtmlReport(WebReport webReport, bool isPOS = false)
        {
            webReport.PrintInHtml = true;
            webReport.Report.Prepare();
            //var returnHtml = webReport.PrintHtml();
            HTMLExport hTMLExport = new HTMLExport();
            hTMLExport.HighQualitySVG = true;
            hTMLExport.Preview = true;
            hTMLExport.WidthUnits = HtmlSizeUnits.Percent;
            hTMLExport.Wysiwyg = true;
            hTMLExport.Pictures = true;
            hTMLExport.AllowOpenAfter = true;
            hTMLExport.EmbedPictures = true;

            hTMLExport.ExtractMacros();
            //hTMLExport.Format = HTMLExportFormat.HTML;
            hTMLExport.EnableVectorObjects = true;
            hTMLExport.NotRotateLandscapePage = true;
            hTMLExport.SinglePage = true;
            hTMLExport.PageBreaks = true;
            hTMLExport.EnableMargins = false;
            hTMLExport.BaseAssign(hTMLExport);
            hTMLExport.SubFolder = false;
            hTMLExport.OnAfterLoad();
            
            hTMLExport.BaseName = "Apex-Report-System";

            return hTMLExport;
        }
        public async Task<ReportsReponse> htmlReturn(WebReport webReport, string SavedFileName, bool isPOS = false)
        {

            var hTMLExport = await getHtmlReport(webReport, isPOS);

            MemoryStream ms = new MemoryStream();
            hTMLExport.Export(webReport.Report, ms);

            ms.Flush();


            var htmlFile = Encoding.UTF8.GetString(ms.ToArray()).Replace("page-break-after: always;", "page-break-after: inherit;");
            var timenow = DateTime.Now.ToString().Replace("/", string.Empty).Replace(" ", string.Empty).Replace(":", string.Empty);
            var fileName = SavedFileName + "-" + timenow;

            //HtmlToPdf pdfFile = new HtmlToPdf();

            //var pdf1 = pdfFile.ConvertHtmlString(htmlFile);
            //var mm = new MemoryStream();
            //pdf1.Save(mm);

            string html = "";
            var file = _fileHandler.CreateInvoiceForPrint(Encoding.UTF8.GetBytes(htmlFile), fileName, fileExt: "html");



            html = File.ReadAllText(file.Item2);


            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(html);
            var b64 = System.Convert.ToBase64String(plainTextBytes);

            PrintHelper.DeleteFileBackground(file.Item2, 1000);

            return new ReportsReponse()
            {
                FileURL = file.Item1,
                FileName = file.Item3,
                htmlPrint = html,
                fileBase64 = b64,
                isFireFox = true,
                Result = !string.IsNullOrEmpty(file.Item3) ? Result.Success : Result.Failed
            };//shoura
        }
        private async Task<ReportsReponse> GetPDFReport(WebReport webReport, string SavedFileName)
        {
            //var hTMLExport = await getHtmlReport(webReport);
            //MemoryStream ms = new MemoryStream();
            //hTMLExport.Export(webReport.Report, ms);
            //ms.Flush();
            //var htmlFile = Encoding.UTF8.GetString(ms.ToArray());
            //var renderer = new ChromePdfRenderer();
            //var pdf = renderer.RenderHtmlAsPdf(htmlFile);

            //var timenow = DateTime.Now.ToString().Replace("/", string.Empty).Replace(" ", string.Empty).Replace(":", string.Empty);
            //var fileName = SavedFileName + "-" + timenow;
            //var filePath = Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], fileName + ".pdf");
            //pdf.SaveAs(filePath);
            //var fileURL = Path.Combine(_configuration["ApplicationSetting:FileURL"], fileName);
            //return new ReportsReponse()
            //{
            //    FileURL = fileURL,
            //    FileName = SavedFileName,
            //    //fileBase64 = arr,
            //    Result = Enums.Result.Success
            //};
            return new ReportsReponse();


        }
        private async Task<ReportsReponse> getExlsReport(WebReport webReport, string SavedFileName)
        {

            return new ReportsReponse();
        }
        private string Extension(exportType _exportType)
        {
            string extension = string.Empty;
            if (_exportType == exportType.Print)
                extension = ".html";
            else if (_exportType == exportType.ExportToPdf)
                extension = ".pdf";
            else if (_exportType == exportType.ExportToWord)
                extension = ".docx";
            else if (_exportType == exportType.ExportToExcle)
                extension = ".xlsx";
            else if (_exportType == exportType.ExportToSVG)
                extension = ".svg";
            else if (_exportType == exportType.ExportToImage)
                extension = ".png";
            return extension;
        }
        private async Task<ReportsReponse> imageExport(WebReport webReport, string SavedFileName)
        {
            webReport.Report.Prepare();
            var pagesCount = webReport.TotalPages;


            var timenow = DateTime.Now.ToString("yyyyMMddThhmmss");
            var file = pagesCount > 1 ? SavedFileName + ".Jpeg" : SavedFileName + timenow + ".Jpeg";
            var ImagesFolder = Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], SavedFileName + "-" + timenow);
            if (pagesCount > 1)
                Directory.CreateDirectory(ImagesFolder);

            var zipFileName = SavedFileName + "-" + timenow + ".zip";
            var zipFilePath = Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], zipFileName);



            var fileName = pagesCount > 1 ? Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], ImagesFolder, file) : Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], file);

            ImageExport imageExport = new ImageExport();
            imageExport.JpegQuality = 500;
            imageExport.ExportAllTabs = true;

            webReport.Report.Export(imageExport, fileName);
            imageExport.Dispose();
            webReport.Report.Dispose();

            if (pagesCount > 1)
            {
                ZipFile.CreateFromDirectory(ImagesFolder, zipFilePath);
                Directory.Delete(ImagesFolder, true);
            }
            var fileURL = pagesCount > 1 ? Path.Combine(_configuration["ApplicationSetting:FileURL"], zipFileName) : Path.Combine(_configuration["ApplicationSetting:FileURL"], file);

            PrintHelper.DeleteFileBackground(pagesCount > 1 ? zipFilePath : file, 1000);


            return new ReportsReponse
            {
                FileName = pagesCount > 1 ? zipFileName : file,
                FileURL = fileURL,
                Result = Result.Success
            };
        }
       

    }
}
