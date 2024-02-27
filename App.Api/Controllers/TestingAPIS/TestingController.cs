using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Application.Services.Process.Bank;
using App.Application.Services.Process.Employee;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Application.Services.Process.Persons;
using App.Application.Services.Process.Safes;
using App.Application.Services.Process.Sales_Man;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.ViewModels;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.UserManagementDB;
using FastReport;
using FastReport.Web;
//using iTextSharp.text.pdf;
//using iTextSharp.text;
//using FastReport.Web;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;
//using Font = iTextSharp.text.Font;
using App.Domain.Entities.Process.Store;
//using iTextSharp.text.pdf.qrcode;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Options;
using System.Text;
using FastReport.Export.PdfSimple;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics;
//using System.Windows.Forms;
using System.Threading;

using static System.Net.WebRequestMethods;
using FastReport.Export.Html;
using System.Linq;
using System;
using App.Application.Services.HelperService.authorizationServices;
using App.Domain.Enums;
using App.Application.Services.Reports.Items_Prices;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Domain.Models.Request.General;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using System.Data.Entity.Infrastructure;
using App.Application.Services.Printing.InvoicePrint;

namespace App.Api.Controllers.TestingAPIS
{
    [Route("api/[controller]")]

    public class TestingController : Controller
    {
        private readonly ISalesManService salesManService;
        private readonly IPersonService personService;
        private readonly IEmployeeServices employeeServices;
        private readonly ISafesBusiness safesBusiness;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly iInvoicesIntegrationService _iInvoicesIntegrationService;
        private readonly IRpt_Store _rpt_Store;
        private readonly IGeneralAPIsService _generalAPIsService;
        private readonly iBanksService banksBusiness;
        private readonly IMediator _mediator;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;

        public TestingController(IPrintService iPrintService,
                                ISalesManService salesManService,
                                IPersonService personService,
                                IEmployeeServices employeeServices,
                                ISafesBusiness safesBusiness,
                                ISystemHistoryLogsService systemHistoryLogsService,
                                iInvoicesIntegrationService iInvoicesIntegrationService,
                                IRpt_Store rpt_Store,
                                IGeneralAPIsService generalAPIsService,
                                iBanksService BanksBusiness, IMediator mediator,
                                iAuthorizationService iAuthorizationService,
                                IHttpContextAccessor httpContext,
                                IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery
            )
        {
            IPrintService = iPrintService;
            this.salesManService = salesManService;
            this.personService = personService;
            this.employeeServices = employeeServices;
            this.safesBusiness = safesBusiness;
            _systemHistoryLogsService = systemHistoryLogsService;
            _iInvoicesIntegrationService = iInvoicesIntegrationService;
            _rpt_Store = rpt_Store;
            _generalAPIsService = generalAPIsService;
            banksBusiness = BanksBusiness;
            _mediator = mediator;
            _iAuthorizationService = iAuthorizationService;
            _httpContext = httpContext;
            this.invGeneralSettingsRepositoryQuery = invGeneralSettingsRepositoryQuery;
        }

        public IPrintService IPrintService { get; }


        ////[HttpGet]
        ////[AllowAnonymous]
        ////public IActionResult GetData()
        ////{
        ////    ERP_UsersManagerContext context = new ERP_UsersManagerContext(_configuration);
        ////    return Ok(context.UserApplications);
        ////}
        //[HttpGet("Calc")]
        //[AllowAnonymous]
        //public IActionResult Calc([FromQuery] double FirstNumber, [FromQuery] double scndNunber)
        //{
        //    double num1 = 0;
        //    double num2 = 0;
        //    double num3 = 0;
        //    double num4 = 0;

        //    if (FirstNumber.ToString().Contains('.'))
        //    {
        //        var firstNumbreSplited = FirstNumber.ToString().Split('.');
        //        num1 = double.Parse("0." + firstNumbreSplited[1]);
        //        num3 = double.Parse(firstNumbreSplited[0]);
        //    }
        //    else
        //        num3 = FirstNumber;

        //    if (scndNunber.ToString().Contains('.'))
        //    {
        //        var scndNunberSplited = scndNunber.ToString().Split('.');
        //        num2 = double.Parse("0." + scndNunberSplited[1]);
        //        num4 = double.Parse(scndNunberSplited[0]);
        //    }
        //    else
        //        num4 = scndNunber;

        //    string res = "";
        //    var sum1 = num3 + num4;
        //    var sum2 = num1 + num2;

        //    if (sum2 != 0)
        //        res = sum1.ToString() + '.' + (sum2.ToString()).Split('.')[1];
        //    else
        //        res = sum1.ToString();
        //    return Ok(res);
        //}
        //[HttpGet("GETURL")]
        //public IActionResult GETURL()
        //{
        //    var URL = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";

        //    return Ok(URL);
        //}
        //[HttpPost]
        //[Route("InjectItemCard")]
        //public async Task<ActionResult<ResponseResult>> PostItemCard(int flag)
        //{
        //    var _units = new List<ItemUnitVM>()
        //    {
        //        new ItemUnitVM()
        //        {
        //            ConversionFactor = 1,
        //            ItemId = 1398,
        //            PurchasePrice = 1,
        //            SalePrice1 =1,
        //            SalePrice2 =1,
        //            SalePrice3 =1,
        //            SalePrice4 =1,
        //            UnitId = 1

        //        }
        //    };

        //    var request = new AddItemRequest()
        //    {
        //        ApplyVAT = true,
        //        ArabicName = "Test",
        //        DepositeUnit = 1,
        //        GroupId = 1,
        //        ItemCode = "1728",
        //        LatinName = "test",
        //        ReportUnit = 1,
        //        Status = 1,
        //        TypeId = 1,
        //        Units = _units,
        //        UsedInSales = true,
        //        VAT = 15,
        //        WithdrawUnit = 1,
        //    };

        //    var result = new ResponseResult();
        //    //If the item is of type serial only one unit should be sent from the front end
        //    if (request.TypeId == (int)ItemTypes.Serial)
        //    {
        //        if (request.Units.Count > 1)
        //        {
        //            return new ResponseResult() { Data = "Select only one unit" };
        //        }
        //    }


        //    for (int i = 0; i < 5000; i++)
        //    {
        //        request.ArabicName = flag.ToString() + "تجريبي" + " - " + (1 + i).ToString();
        //        request.LatinName = flag.ToString() + "Test" + " - " + (1 + i).ToString();
        //        result = await _mediator.Send(request);
        //    }
        //    return result;
        //}

        //private bool CheckUnitPriceValidation(ItemUnitVM resReport, bool other_ZeroPricesInItems)
        //{
        //    if (!other_ZeroPricesInItems)
        //    {
        //        if (resReport.SalePrice1 >= resReport.SalePrice2
        //            && resReport.SalePrice1 >= resReport.SalePrice3
        //            && resReport.SalePrice1 >= resReport.SalePrice4)
        //        {
        //            if (resReport.SalePrice2 >= resReport.SalePrice3
        //            && resReport.SalePrice2 >= resReport.SalePrice4)
        //            {
        //                if (resReport.SalePrice3 >= resReport.SalePrice4 && resReport.PurchasePrice <= resReport.SalePrice4)
        //                {
        //                    return true;
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if (resReport.SalePrice1 >= resReport.SalePrice2
        //        && resReport.SalePrice1 >= resReport.SalePrice3
        //        && resReport.SalePrice1 >= resReport.SalePrice4)
        //        {
        //            if (resReport.SalePrice2 >= resReport.SalePrice3
        //            && resReport.SalePrice2 >= resReport.SalePrice4)
        //            {
        //                if (resReport.SalePrice3 >= resReport.SalePrice4 || resReport.PurchasePrice == 0)
        //                {
        //                    return true;
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}
        //[HttpGet("InjectData")]
        //public async Task<IActionResult> InjectData()
        //{
        //    for (int i = 0; i < 200; i++)
        //    {
        //        //await salesManService.AddSalesMan(new SalesManRequest()
        //        //{
        //        //    ArabicName = "مندوب مبيعات - " + i.ToString(),
        //        //    LatinName = "SalesMan - " + i.ToString(),
        //        //    Branches = new int[] {1}

        //        //});
        //        await personService.AddPerson(new PersonRequest()
        //        {
        //            ArabicName = "عميل - " + i.ToString(),
        //            LatinName = "Customer - " + i.ToString(),
        //            IsSupplier = true,
        //            SalesManId = 1,
        //            Type = 1,
        //            Branches = new int[] { 1 },
        //            Status = 1
        //        });
        //        await personService.AddPerson(new PersonRequest()
        //        {
        //            ArabicName = "مورد - " + i.ToString(),
        //            LatinName = "Supplier - " + i.ToString(),
        //            IsSupplier = true,
        //            Type = 1,
        //            Branches = new int[] { 1 },
        //            Status = 1
        //        });

        //        // await employeeServices.AddEmployee(new EmployeesRequestDTOs.Add()
        //        // {
        //        //     ArabicName = "موظف - " + i.ToString(),
        //        //     LatinName = "Employee - " + i.ToString(),
        //        //     Branches  = new int[] {1},
        //        //     JobId = 1,
        //        //     Status = 1,
        //        // });

        //        // var SafeSaved = await safesBusiness.AddTreasury(new TreasuryParameter()
        //        // {
        //        //     ArabicName = "خزينة - " + i.ToString(),
        //        //     LatinName = "Safe - " + i.ToString(),
        //        //     BranchId = 1,
        //        //     Status = 1
        //        // });
        //        //var bankSaved =  await banksBusiness.AddBanks(new BankRequestsDTOs.Add()
        //        // {
        //        //     ArabicName = "بنك -" + i.ToString(),
        //        //     LatinName = "Bank -" + i.ToString(),
        //        //     BranchesId = new int[] {1},
        //        //     Status = 1
        //        // });
        //    }

        //    return Ok();
        //}
        ////WebReport webReport = new WebReport();
        ////[HttpGet("PersonReport")]

        ////public async Task<IActionResult> PersonReport(string? Type, [FromQuery] PersonsSearch parameters)
        ////{
        ////    int[] supplierTypes = null;
        ////    if (!string.IsNullOrEmpty(Type))
        ////        supplierTypes = Array.ConvertAll(Type.Split(','), s => int.Parse(s));

        ////    parameters.Type = supplierTypes;
        ////    var add = await personService.GetListOfPersons(parameters);

        ////    webReport = await IPrintService.ReportInvoice(invoiceId);
        ////    ViewBag.WebReport = webReport;
        ////    return View();

        ////    // SetReport();
        ////    // if (ExportType != null) Export(ExportType);

        ////    // SetPage("print");

        ////     webReport   = IPrintService.Report(ReportName: "Persons", ReferenceName: "RefPersons");
        ////    ViewBag.WebReport = webReport;
        ////    // = webReport;
        ////    //ViewBag.WebReport = webReport;
        ////// return   ExportToExcel(webReport);

        ////  return View();


        ////}
        ////-------------------------------------------------------------------------
        //PersonsSearch obj = new PersonsSearch()
        //{
        //    IsSupplier = true,
        //    Type = new int[2],
        //    Status = 1,
        //    Name = ""


        //};

        ////[HttpPost]
        ////public async Task<IActionResult> ExporDataToFile()
        ////{
        ////    var dictioneryexportType = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
        ////    var exportType = dictioneryexportType["Export"];

        ////    switch (exportType)
        ////    {

        ////        case "Pdf":
        ////            return await ExportToPdf(false);
        ////            break;
        ////        case "Excel":
        ////            return await ExportToExcel();
        ////            break;
        ////        case "Print":
        ////            PrintReport();

        ////            break;

        ////    }
        ////    return null;
        ////    //return null;
        ////    // var persons = await personService.GetListOfPersonsExport(obj);
        ////    ////var collection = ((IEnumerable<object>)persons).Cast<object>().ToList();

        ////    // PropertyInfo[] properties = typeof(ExportPersonModel).GetProperties();
        ////    // DataTable dt = new DataTable();


        ////public async Task<FileContentResult> ExportToPdf(bool forPrintOrExportToExcel)
        ////{
        ////    int id = 30;
        ////    webReport = await IPrintService.ReportInvoice(id);

        ////    // dt.Columns.Add("Id");
        ////    // dt.Columns.Add("Code");
        ////    // dt.Columns.Add("ArabicName");
        ////    // dt.Columns.Add("LatinName");
        ////    // dt.Columns.Add("Type");

        ////    PDFSimpleExport pdfExport = new PDFSimpleExport();

        ////    MemoryStream ms = new MemoryStream();


        ////    pdfExport.Export(webReport.Report, ms);

        ////    // IList<ExportPersonModel> personsToExport= new List<ExportPersonModel>();

        ////    // var collection = ((IEnumerable<object>)persons.Data).Cast<object>().ToList();
        ////    //IList<object> collection = new List<object>();
        ////    // collection.Add(persons.Data);
        ////    //IList<object> res = (IList<object>)persons.Data;
        ////    // IList res = persons.Data as IList;
        ////    //foreach (var data in persons)
        ////    //{
        ////    //    // DataRow row = dt.NewRow();
        ////    //    //personsToExport.Add(data);
        ////    //    var values = new object[properties.Length];
        ////    //    for(int i = 0; i < properties.Length; i++)
        ////    //    {

        ////    //        values[i] = properties[i].GetValue(data,null);
        ////    //    }

        ////    //    dt.Rows.Add(values);
        ////    //}




        ////}

        ////[HttpGet("PrintReport")]
        ////public async Task<int> PrintReport()
        ////{

        ////    // PdfDocument pdfDocument = file.;
        ////    // IronPdf.PdfDocument file =(IronPdf.PdfDocument) ExportToPdf();




        ////    await ExportToPdf(true);

        ////    string filepath = Path.Combine("App_Data", "PersonsPrint.pdf");



        ////    using IronPdf.PdfDocument pdfDocument = IronPdf.PdfDocument.
        ////      FromFile(filepath);

        ////    //using IronPdf.PdfDocument pdfDocument = IronPdf.PdfDocument.FromFile
        ////    //    (filepath);
        ////    // pdfDocument.Print();
        ////    //pdfDocument.GetPrintDocument().PrinterSettings.FromPage = 1;
        ////    //pdfDocument.GetPrintDocument().PrinterSettings.ToPage = 2;



        ////    // var pp = pdfDocument.GetPrintDocument();

        ////    return pdfDocument.Print();

        ////    // return Ok();

        ////    //  pdfDocument.GetPrintDocument().PrinterSettings. = 5;

        ////    // return Ok(ppp);

        ////    // System.IO.File.Delete(filepath);
        ////    //  return File(ppp.BinaryData,"application/pdf");



        ////}
        ////[HttpGet("ExportToPdf")]

        ////public async Task<IActionResult> ExportToPdf(bool forPrintOrExportToExcel)
        ////{
        ////    //Report report = new Report();

        ////    webReport = IPrintService.Report(ReportName: "Persons", ReferenceName: "RefPersons");

        ////    webReport.Report.Prepare();
        ////    //webReport.PrintHtml();
        ////    PDFSimpleExport pdfExport = new PDFSimpleExport();
        ////    if (!forPrintOrExportToExcel)
        ////    {
        ////        MemoryStream ms = new MemoryStream();


        ////        pdfExport.Export(webReport.Report, ms);
        ////        // pdfExport.AllowOpenAfter = true;
        ////        // return (pdfExport);
        ////        ms.Flush();
        ////        return File(ms.ToArray(), "application/pdf", "Persons" + ".pdf");
        ////    }
        ////    else
        ////    {
        ////        string filepath = Path.Combine("App_Data", "PersonsPrint.pdf");
        ////        Stream stream = new FileStream(filepath, FileMode.Create);


        ////        pdfExport.Export(webReport.Report, stream);
        ////        // file.CopyTo(stream);
        ////        stream.Dispose();
        ////        return Ok(filepath);
        ////    }



        ////    //if (persons.Rows.Count > 0)
        ////    //{
        ////    //    int pdfRowIndex = 1;
        ////    //    string filename = "Persons-" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt");
        ////    //    string filepath = Path.Combine("App_Data", filename + ".pdf");
        ////    //    Document document = new Document(PageSize.A5, 5f, 5f, 10f, 10f);
        ////    //    FileStream fs = new FileStream(filepath, FileMode.Create);
        ////    //    PdfWriter writer = PdfWriter.GetInstance(document, fs);
        ////    //    document.Open();

        ////    //    Font font1 = FontFactory.GetFont(FontFactory.COURIER_BOLD, 10);
        ////    //    Font font2 = FontFactory.GetFont(FontFactory.COURIER, 8);

        ////    //    float[] columnDefinitionSize = { 2F, 2F, 5F, 5F, 2F };
        ////    //    PdfPTable table;
        ////    //    PdfPCell cell;

        ////    //    table = new PdfPTable(columnDefinitionSize)
        ////    //    {
        ////    //        WidthPercentage = 100
        ////    //    };

        ////    //    cell = new PdfPCell
        ////    //    {
        ////    //        BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
        ////    //    };
        ////    //    foreach (var col in persons.Columns)
        ////    //    {
        ////    //        table.AddCell(new Phrase(col.ToString(), font1));
        ////    //    }

        ////    //    table.HeaderRows = 1;

        ////    //    foreach (DataRow data in persons.Rows)
        ////    //    {
        ////    //        foreach (var col in persons.Columns)
        ////    //        {
        ////    //            table.AddCell(new Phrase(data[col.ToString()].ToString(), font2));
        ////    //        }


        ////    //        pdfRowIndex++;
        ////    //    }

        ////    //    document.Add(table);
        ////    //    document.Close();
        ////    //    document.CloseDocument();
        ////    //    document.Dispose();
        ////    //    writer.Close();
        ////    //    writer.Dispose();
        ////    //    fs.Close();
        ////    //    fs.Dispose();

        ////    //    FileStream sourceFile = new FileStream(filepath, FileMode.Open);
        ////    //    float fileSize = 0;
        ////    //    fileSize = sourceFile.Length;
        ////    //    byte[] getContent = new byte[Convert.ToInt32(Math.Truncate(fileSize))];
        ////    //    sourceFile.Read(getContent, 0, Convert.ToInt32(sourceFile.Length));
        ////    //    sourceFile.Close();
        ////    //    Response.Clear();
        ////    //    Response.Headers.Clear();
        ////    //    Response.ContentType = "application/pdf";
        ////    //    Response.Headers.Add("Content-Length", getContent.Length.ToString());
        ////    //    Response.Headers.Add("Content-Disposition", "attachment; filename=" + filename + ".pdf;");
        ////    //    Response.Body.WriteAsync(getContent);
        ////    //    Response.Body.Flush();
        ////    //}
        ////}
        ////public IFormFile ReturnFormFile(FileStreamResult result)
        ////{
        ////    var ms = new MemoryStream();
        ////    try
        ////    {
        ////        result.FileStream.CopyTo(ms);
        ////        return new FormFile(ms, 0, ms.Length);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        ms.Dispose();
        ////        throw;
        ////    }
        ////    finally
        ////    {
        ////        ms.Dispose();
        ////    }
        ////}

        ////[HttpGet("ExportToExcel")]
        ////private async Task<ActionResult> ExportToExcel()
        ////{
        ////    webReport = IPrintService.Report(ReportName: "Persons", ReferenceName: "RefPersons");

        ////    webReport.Report.Prepare();
        ////    HTMLExport html = new HTMLExport();

        ////    //html.EmbedPictures = true;

        ////    //html.SinglePage = true;

        ////    //html.SubFolder = false;

        ////    //html.Layers = true;

        ////    //html.Navigator = false;

        ////    MemoryStream ms = new MemoryStream();

        ////    webReport.Report.Export(html, ms);


        ////    // pdfExport.AllowOpenAfter = true;
        ////    // return (pdfExport);
        ////    ms.Flush();
        ////    return File(ms.ToArray(), "application/vnd.ms-excel", "personsnew.xls");
        ////    //using (var export = new FastReport.Export.OoXML.Excel2007Export())
        ////    //{
        ////    //    if (export.ShowDialog())
        ////    //        export.Export(report1, @"result.xlsx");
        ////    //}

        ////    //await ExportToPdf(true);
        ////    //string filepath = Path.Combine("App_Data", "PersonsPrint.pdf");

        ////    //byte[] pdf = System.IO.File.ReadAllBytes(filepath);
        ////    //byte[] xls = null;
        ////    //PdfFocus f = new PdfFocus();

        ////    //f.OpenPdf(pdf);

        ////    //if (f.PageCount > 0)
        ////    //{
        ////    //    xls = f.ToExcel();

        ////    //    //Save Excel workbook to a file in order to show it
        ////    //    if (xls != null)
        ////    //    {
        ////    //       // File.WriteAllBytes(pathToExcel, xls);
        ////    //        //System.IO.File.Delete(filePath.ToString());
        ////    //        return File(xls, "application/vnd.ms-excel", "personsnew.xls");
        ////    //    }

        ////    //}
        ////    //return Ok();
        ////    //using FastReport.Export.ExportBase();
        ////    //var file = ExportToPdf();
        ////    //var streamResult = new FileStreamResult(new MemoryStream(file.FileContents),
        ////    //                            file.ContentType);
        ////    //streamResult.FileDownloadName = file.FileDownloadName;
        ////    //  streamResult.FileStream.CopyTo(stream);
        ////    //  return Ok();
        ////    // var file2= ReturnFormFile(streamResult);
        ////    //var ms = new MemoryStream();

        ////    //{
        ////    //    file.fi.CopyTo(ms);
        ////    //    return new FormFile(ms, 0, ms.Length);
        ////    //};

        ////    //string filepath = Path.Combine("App_Data", "Persons" + ".frx");

        ////    //using (Stream stream = new FileStream())
        ////    //{

        ////    //   await streamResult.FileStream.CopyToAsync(stream);
        ////    //   return File(stream, contentType: "application/vnd.ms-excel", "personsnew.xls");

        ////    //}




        ////    //var streamResult = new FileStreamResult(new MemoryStream(file.FileContents),
        ////    //                            file.ContentType);
        ////    //streamResult.FileDownloadName = file.FileDownloadName;
        ////    //// FileContentResult file= (FileContentResult)  ExportToPdf();

        ////    //// var files = new FileContentResult(file.FileContents, file.ContentType);

        ////    //// var files=System.IO.File.ReadAllBytes(file.FileDownloadName);
        ////    ////string filepath = Path.Combine("App_Data", "Persons" + ".frx");

        ////    //// return File(file, "application/vnd.ms-excel", "Persons.xls");
        ////    //return File(streamResult.FileStream, "application/vnd.ms-excel","persons.xls");
        ////    ////GrapeCity.Documents.Drawing.
        ////    //string filepath = Path.Combine("App_Data", "Persons" + ".frx");


        ////    //var stream = new FileStream(filepath, FileMode.Open);
        ////    //// return  File(stream, "application/pdf","report.pdf");

        ////    //return new FileStreamResult(stream, "application/pdf")
        ////    //{
        ////    //    FileDownloadName = "downloadedfile.pdf"

        ////    //HtmlLoadOptions options = new HtmlLoadOptions();
        ////    //using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(File.)))
        ////    //{

        ////    //    document = new Document(ms, options);

        ////    //    document.Save(pdfFileName);
        ////    //}

        ////    //   return File(ms.ToArray(), "application/pdf", Path.GetFileNameWithoutExtension("Simple List") + ".pdf");

        ////    // Instantiate Renderer

        ////    //using (var workbook = new XLWorkbook())
        ////    //{
        ////    //    var worksheet = workbook.Worksheets.Add("Persons");
        ////    //    var currentRow = 1;
        ////    //    int column = 1;
        ////    //    foreach(var col in person.Columns)
        ////    //    {

        ////    //        worksheet.Cell(currentRow, column).Value = col;
        ////    //        column++;
        ////    //    }

        ////    //    int colNumber = 1;
        ////    //    int currentCellRow = -1;

        ////    //    for (int i = 0; i < person.Rows.Count; i++)
        ////    //    {
        ////    //        {

        ////    //            currentRow++;
        ////    //            currentCellRow++;

        ////    //            for (int j = 0; j < person.Columns.Count; j++)
        ////    //            {

        ////    //                var name = person.Columns[j].ColumnName;
        ////    //                worksheet.Cell(currentRow, colNumber).Value = person.Rows[currentCellRow][name];
        ////    //                colNumber++;
        ////    //            }

        ////    //            colNumber = 1;

        ////    //        }
        ////    //    }
        ////    //    using var stream = new MemoryStream();
        ////    //    workbook.SaveAs(stream);
        ////    //    var content = stream.ToArray();
        ////    //    Response.Clear();
        ////    //    Response.Headers.Add("content-disposition", "attachment;filename=Persons.xls");
        ////    //    Response.ContentType = "application/xls";
        ////    //    Response.Body.WriteAsync(content);
        ////    //    Response.Body.Flush();
        ////    //}
        ////}

        ////------------------------------------------------------------

        //[HttpGet("ShowReport")]
        //[AllowAnonymous]
        //public IActionResult ShowReport()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("EmployeeID");
        //    dt.Columns.Add("LastName");
        //    dt.Columns.Add("FirstName");
        //    dt.Columns.Add("Title");
        //    dt.Columns.Add("TitleOfCourtesy");
        //    dt.Columns.Add("BirthDate");
        //    dt.Columns.Add("HireDate");
        //    dt.Columns.Add("Address");
        //    dt.Columns.Add("City");
        //    dt.Columns.Add("Region");
        //    dt.Columns.Add("PostalCode");
        //    dt.Columns.Add("Country");
        //    dt.Columns.Add("HomePhone");
        //    dt.Columns.Add("Extension");
        //    dt.Columns.Add("Photo");
        //    dt.Columns.Add("Notes");
        //    dt.Columns.Add("ReportsTo");

        //    for (int i = 0; i < 20; i++)
        //    {
        //        string[] row =
        //        {
        //            "2",
        //            "Fuller",
        //            "Andrew",
        //            "Vice President, Sales",
        //            "Dr.",
        //            "1972-02-19T00:00:00+03:00",
        //            "2009-08-14T00:00:00+04:00",
        //            "908 W. Capital Way",
        //            "Tacoma",
        //            "WA",
        //            "98401",
        //            "USA",
        //            "(206) 555-9482",
        //            "3457",
        //            "/9j/4AAQSkZJRgABAQEBLAEsAAD/4QIaRXhpZgAASUkqAAgAAAAQAA4BAgBuAAAAzgAAABABAgAVAAAAPAEAABoBBQABAAAAUgEAABsBBQABAAAAWgEAACgBAwABAAAAAgAAADEBAgAQAAAAYgEAADIBAgAUAAAAcgEAADsBAgAMAAAAhgEAAD4BBQACAAAAkgEAAD8BBQAGAAAAogEAAAEDBQABAAAA0gEAAAIDAgASAAAA2gEAABBRAQABAAAAATABABFRBAABAAAAIy4AABJRBAABAAAAIy4AAGmHBAABAAAA7AEAAAAAAABTbWlsaW5nIHlvdW5nIGJ1c2luZXNzIHdvbWFuIHdpdGggZG9jdW1lbnQsIGxhcHRvcCwgY2VsbHBob25lIGFuZCBjb2ZmZWUgY3VwLCBzaXR0aW5nIGluIGFuIHVyYmFuIGVudmlyb25tZW50AEhhc3NlbGJsYWQgSDNEIElJLTM5AADfkwQA6AMAAN+TBADoAwAAUGFpbnQuTkVUIHYzLjIyADIwMDg6MDQ6MjQgMTc6MTM6MDEAWXVyaSBBcmN1cnMAD4cAAKCGAQAPjAAAoIYBAFL9AACghgEAQIEAAKCGAQB5fQAAoIYBAIvpAACghgEA5TwAAKCGAQDMGQAAoIYBAKCGAQCOsQAAc1JHQiBJRUM2MTk2Ni0yLjEAAQADkAIAFAAAAP4BAAAAAAAAMjAwODowMzoxNyAxMjo1MDoxNAD/4gxYSUNDX1BST0ZJTEUAAQEAAAxITGlubwIQAABtbnRyUkdCIFhZWiAHzgACAAkABgAxAABhY3NwTVNGVAAAAABJRUMgc1JHQgAAAAAAAAAAAAAAAAAA9tYAAQAAAADTLUhQICAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABFjcHJ0AAABUAAAADNkZXNjAAABhAAAAGx3dHB0AAAB8AAAABRia3B0AAACBAAAABRyWFlaAAACGAAAABRnWFlaAAACLAAAABRiWFlaAAACQAAAABRkbW5kAAACVAAAAHBkbWRkAAACxAAAAIh2dWVkAAADTAAAAIZ2aWV3AAAD1AAAACRsdW1pAAAD+AAAABRtZWFzAAAEDAAAACR0ZWNoAAAEMAAAAAxyVFJDAAAEPAAACAxnVFJDAAAEPAAACAxiVFJDAAAEPAAACAx0ZXh0AAAAAENvcHlyaWdodCAoYykgMTk5OCBIZXdsZXR0LVBhY2thcmQgQ29tcGFueQAAZGVzYwAAAAAAAAASc1JHQiBJRUM2MTk2Ni0yLjEAAAAAAAAAAAAAABJzUkdCIElFQzYxOTY2LTIuMQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAWFlaIAAAAAAAAPNRAAEAAAABFsxYWVogAAAAAAAAAAAAAAAAAAAAAFhZWiAAAAAAAABvogAAOPUAAAOQWFlaIAAAAAAAAGKZAAC3hQAAGNpYWVogAAAAAAAAJKAAAA+EAAC2z2Rlc2MAAAAAAAAAFklFQyBodHRwOi8vd3d3LmllYy5jaAAAAAAAAAAAAAAAFklFQyBodHRwOi8vd3d3LmllYy5jaAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABkZXNjAAAAAAAAAC5JRUMgNjE5NjYtMi4xIERlZmF1bHQgUkdCIGNvbG91ciBzcGFjZSAtIHNSR0IAAAAAAAAAAAAAAC5JRUMgNjE5NjYtMi4xIERlZmF1bHQgUkdCIGNvbG91ciBzcGFjZSAtIHNSR0IAAAAAAAAAAAAAAAAAAAAAAAAAAAAAZGVzYwAAAAAAAAAsUmVmZXJlbmNlIFZpZXdpbmcgQ29uZGl0aW9uIGluIElFQzYxOTY2LTIuMQAAAAAAAAAAAAAALFJlZmVyZW5jZSBWaWV3aW5nIENvbmRpdGlvbiBpbiBJRUM2MTk2Ni0yLjEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHZpZXcAAAAAABOk/gAUXy4AEM8UAAPtzAAEEwsAA1yeAAAAAVhZWiAAAAAAAEwJVgBQAAAAVx/nbWVhcwAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAo8AAAACc2lnIAAAAABDUlQgY3VydgAAAAAAAAQAAAAABQAKAA8AFAAZAB4AIwAoAC0AMgA3ADsAQABFAEoATwBUAFkAXgBjAGgAbQByAHcAfACBAIYAiwCQAJUAmgCfAKQAqQCuALIAtwC8AMEAxgDLANAA1QDbAOAA5QDrAPAA9gD7AQEBBwENARMBGQEfASUBKwEyATgBPgFFAUwBUgFZAWABZwFuAXUBfAGDAYsBkgGaAaEBqQGxAbkBwQHJAdEB2QHhAekB8gH6AgMCDAIUAh0CJgIvAjgCQQJLAlQCXQJnAnECegKEAo4CmAKiAqwCtgLBAssC1QLgAusC9QMAAwsDFgMhAy0DOANDA08DWgNmA3IDfgOKA5YDogOuA7oDxwPTA+AD7AP5BAYEEwQgBC0EOwRIBFUEYwRxBH4EjASaBKgEtgTEBNME4QTwBP4FDQUcBSsFOgVJBVgFZwV3BYYFlgWmBbUFxQXVBeUF9gYGBhYGJwY3BkgGWQZqBnsGjAadBq8GwAbRBuMG9QcHBxkHKwc9B08HYQd0B4YHmQesB78H0gflB/gICwgfCDIIRghaCG4IggiWCKoIvgjSCOcI+wkQCSUJOglPCWQJeQmPCaQJugnPCeUJ+woRCicKPQpUCmoKgQqYCq4KxQrcCvMLCwsiCzkLUQtpC4ALmAuwC8gL4Qv5DBIMKgxDDFwMdQyODKcMwAzZDPMNDQ0mDUANWg10DY4NqQ3DDd4N+A4TDi4OSQ5kDn8Omw62DtIO7g8JDyUPQQ9eD3oPlg+zD88P7BAJECYQQxBhEH4QmxC5ENcQ9RETETERTxFtEYwRqhHJEegSBxImEkUSZBKEEqMSwxLjEwMTIxNDE2MTgxOkE8UT5RQGFCcUSRRqFIsUrRTOFPAVEhU0FVYVeBWbFb0V4BYDFiYWSRZsFo8WshbWFvoXHRdBF2UXiReuF9IX9xgbGEAYZRiKGK8Y1Rj6GSAZRRlrGZEZtxndGgQaKhpRGncanhrFGuwbFBs7G2MbihuyG9ocAhwqHFIcexyjHMwc9R0eHUcdcB2ZHcMd7B4WHkAeah6UHr4e6R8THz4faR+UH78f6iAVIEEgbCCYIMQg8CEcIUghdSGhIc4h+yInIlUigiKvIt0jCiM4I2YjlCPCI/AkHyRNJHwkqyTaJQklOCVoJZclxyX3JicmVyaHJrcm6CcYJ0kneierJ9woDSg/KHEooijUKQYpOClrKZ0p0CoCKjUqaCqbKs8rAis2K2krnSvRLAUsOSxuLKIs1y0MLUEtdi2rLeEuFi5MLoIuty7uLyQvWi+RL8cv/jA1MGwwpDDbMRIxSjGCMbox8jIqMmMymzLUMw0zRjN/M7gz8TQrNGU0njTYNRM1TTWHNcI1/TY3NnI2rjbpNyQ3YDecN9c4FDhQOIw4yDkFOUI5fzm8Ofk6Njp0OrI67zstO2s7qjvoPCc8ZTykPOM9Ij1hPaE94D4gPmA+oD7gPyE/YT+iP+JAI0BkQKZA50EpQWpBrEHuQjBCckK1QvdDOkN9Q8BEA0RHRIpEzkUSRVVFmkXeRiJGZ0arRvBHNUd7R8BIBUhLSJFI10kdSWNJqUnwSjdKfUrESwxLU0uaS+JMKkxyTLpNAk1KTZNN3E4lTm5Ot08AT0lPk0/dUCdQcVC7UQZRUFGbUeZSMVJ8UsdTE1NfU6pT9lRCVI9U21UoVXVVwlYPVlxWqVb3V0RXklfgWC9YfVjLWRpZaVm4WgdaVlqmWvVbRVuVW+VcNVyGXNZdJ114XcleGl5sXr1fD19hX7NgBWBXYKpg/GFPYaJh9WJJYpxi8GNDY5dj62RAZJRk6WU9ZZJl52Y9ZpJm6Gc9Z5Nn6Wg/aJZo7GlDaZpp8WpIap9q92tPa6dr/2xXbK9tCG1gbbluEm5rbsRvHm94b9FwK3CGcOBxOnGVcfByS3KmcwFzXXO4dBR0cHTMdSh1hXXhdj52m3b4d1Z3s3gReG54zHkqeYl553pGeqV7BHtje8J8IXyBfOF9QX2hfgF+Yn7CfyN/hH/lgEeAqIEKgWuBzYIwgpKC9INXg7qEHYSAhOOFR4Wrhg6GcobXhzuHn4gEiGmIzokziZmJ/opkisqLMIuWi/yMY4zKjTGNmI3/jmaOzo82j56QBpBukNaRP5GokhGSepLjk02TtpQglIqU9JVflcmWNJaflwqXdZfgmEyYuJkkmZCZ/JpomtWbQpuvnByciZz3nWSd0p5Anq6fHZ+Ln/qgaaDYoUehtqImopajBqN2o+akVqTHpTilqaYapoum/adup+CoUqjEqTepqaocqo+rAqt1q+msXKzQrUStuK4trqGvFq+LsACwdbDqsWCx1rJLssKzOLOutCW0nLUTtYq2AbZ5tvC3aLfguFm40blKucK6O7q1uy67p7whvJu9Fb2Pvgq+hL7/v3q/9cBwwOzBZ8Hjwl/C28NYw9TEUcTOxUvFyMZGxsPHQce/yD3IvMk6ybnKOMq3yzbLtsw1zLXNNc21zjbOts83z7jQOdC60TzRvtI/0sHTRNPG1EnUy9VO1dHWVdbY11zX4Nhk2OjZbNnx2nba+9uA3AXcit0Q3ZbeHN6i3ynfr+A24L3hROHM4lPi2+Nj4+vkc+T85YTmDeaW5x/nqegy6LzpRunQ6lvq5etw6/vshu0R7ZzuKO6070DvzPBY8OXxcvH/8ozzGfOn9DT0wvVQ9d72bfb794r4Gfio+Tj5x/pX+uf7d/wH/Jj9Kf26/kv+3P9t////2wBDAAMCAgMCAgMDAwMEAwMEBQgFBQQEBQoHBwYIDAoMDAsKCwsNDhIQDQ4RDgsLEBYQERMUFRUVDA8XGBYUGBIUFRT/2wBDAQMEBAUEBQkFBQkUDQsNFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBT/wAARCADUAKoDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9Sh0p46U1akUcVIkc74oXNo/0rxu74un+te0eJhm0f6V41fLi7k+tcGI6FRLVpc7QOavHUTjG6sNGKjg07cx710UpPlMpLU111Iq2d1WY9aK/xVzwye9YHjPxpp3gjR5L7UJguAfLiz80jegFa8zElfY7DxJ8SdO8J6Y95qV2lvEo4BPzOfRR3NfLXxW/a81QpKumSPpFpzsMeDPIPc87foPzrxr4p/F3VPGGpzXEkE1yu4rFEi/JGPQEkD8cc1wdh8MPF3xGnEnkzPCTwxX5QPTINefXxNtE7Hp0MI5fZuyTWfj7rer3skz6jqO4nO572TJ/Piuo8B/tlePvh6TFZ6s2oWjEEwaivnbf90nkfhitXSf2Qb+7CC7d4/Ugk/1rsLT9jexjgG6Zs46EV5v1yMXoz0v7OqPSSR6t8Gv+ChyX+o2th4802K1s7g7U1jTVYxxHt5qEkgepHT0r7Bg1yx8QaTb6npV7Df2FwgeG5t3Do49QRX5qat+zHd6FbTrZuzoRlTjoe1WPgr+0H4j/AGdPEcOia6k114PuH23NmWJ+xsx5miJHT1XofY134fGqfuyZ5+IwFSiuZLQ/RFvE81m5UZNZ+peNbwISvFZ0t3BfxxXVtKk9vMgkjlQ5V1IyCD6EVQvv9Wa9JNnlNEF1431EuQGwKRfE2oTKC0prHnT56sxL8orpWxzyWpoyatePHzKa57VNQuyT++YVthcpWNqcPXiokK2hTt7m4ZhmZj+NbKyyYHzt+dY8CYcVtKBtHHaoQH0OvapB0qMdRUg6VR3GL4kH+iv9K8Y1Hi8k+te1eIhm0b6V4rqoxeyfWuHELRFR3IEGRUuw0luAauCIEVdJ2iTJalIoa+Nfj146v/FnjG7tNOgL2ts5gVrp9sYxwSoHJycnPAr7C8U340Tw5qd/wPs1tJLycDhSa/Pu81a413W41UebC7gs/QMfTjrSqytBtHRh4c00ep/BX4Bf8JTIdU1oo6hgRFEOD7Z64+tfUdh4RsdDs44Le3SJEGAqgDFUfgnY/ZfC9soh2fIAeK7XVLVkJPRRySeK+bV5as+sXuJRWhzzxog2qmBiqjrsJGPyrQmliVSdw/A1ULoxBzkU3DTY2T1uVxCtwjblDA8YIryT4yfBm08W6PPJBGqXKjcp2ivZZJ4LSEvLIkSDqWYAfrWHH4q0TUrs2keqWckx48oTKSf1rJwad4lu0lboc/8AsoeI72/8ATaDqd2bm+0Kf7ModcMITzH9RjI/CvZLxMxmvB/B6N4J+P8AFYRQH7JrtlIwlU8bkw2D6+1e+3akp0NfR4ao5002fFYqkqVVxRzsqfP+NTxr8op0kLGThT+VWEtnZeFJr1IvQ86SEQ/JWZqODnitlbKYr/qz+VUb3TLhs4japkQZMEeWFbCxjaPpUNppF0SMxGtcaRdYH7s1nZjSPbx2qQdKjHapF6VZ1GV4gGbR/pXies8X8g969u17/j0f6V4jrvF/JXJX2LjuMtMsa0kjY44qvpFv5pFdVbaTuwcVVKndXJk9TyD49Wssnwj8Soj+WzWrc5xkZHH49K+MPgZ4Yi8UfEnS9PupjIxcM+RwAP4QOij2r7x/aD8O2t78Mr22vLuawt7l4oDPbxCR1JcY+UkZ5HrXyP8ACTwzqPhf48QW8tm6pBLhXRcKw4+b245rixMoxcqd+iPYwdCbhGtbRtq/pY+4dMsLbQbFkhjWOGBOB0FeA/FfxhpF9ffZ9X166UuSEtre5aIHnsicn6nNev8AjGe6utKmtoG8vzBgnOK+Rvih+zzrWvvezWfiy909robZBaxbXYdlMm7djrwMD615UpK6gtEfRQg4pztd9EaOk6Zov9oK+meKtXt5VbeIpLl+cdsOORXunhudtTs0KybwOr18r/Df9nXVfDhsrT+0JvIt5jNvjUCSbOOJGJO4cdO2TX2X4H8If2doz5Xa+M7ewoUHze47op1Hy+8rP7zxz403Hh7QfKvPEVzLNGqkLbCYpGfUnkD8Sa4/wH8dfhreSw6dBZ2FvBO2xW+Qhj0PPU9uRnrXpHxg+EFh8Srcw3qGTyyRt3ED8R36VyfhH9njwto0dpFcaBBqP2U/uPtmZkhPqiuSF6DoO1JcqvzN3C9SVuVKx6/4M8L2es/Evwhcwyh4LaO6WNmbcTuj4AP4Gvod/CERTG0V4X4D0B4vFnh9LPEH2e7SXaBxsAIYf98k19O17uBk/ZarqfLZnT5a++6OLfwbED90Y+lWYPCUKj7ox9K6rAPajGK9LmPF9k3uzAj8MwgfdH5VJ/wjFuRygrcoo5mNUkY0fhu3T+AVONDt/wC6PyrSopXZXIisOlSqeKiXpUi8ipLM7XBm0f6V4f4i+XUXr3LWh/or/SvDfE3GotXNX+EuO5reGEDlPpXoNnbr5adK898KN8qmvQ7J8wpXVS+BGM9zE+JPgmLx34J1DR3YxtIFdGHUOrBh+orwHwjrD+IfHet3Emn29uLdysWBtkXaQDn1zmvqcPheK8a+JXhpvDeow63p9g9wPOPmfZwcqGI3bgPvdOM9K8bM8M5NV4dN/Tp912fUZNjIqEsJV66x8npf77Idfv8AaECnucc1RvbO1ihCyBScZOau6gp/s9Z1VkbAbawII+org9d19rfLMTivJ5+XdH0MLVEuUsa7qq6NaO9vEu7HygAZJp03xn0rwzoOnhmuL27nwjRRQs5Rj13BQcY98VwV54kjupPNvbiO1to+hkcDFUpfFGlySqNOsLjU52wA8URwSemDj2pRlJ+9E6VRjP3XG9v63PTrvxJDfoji3mMk2T5kYwqnHGcms7QvEp/tJrG/QRzDlW7OPUVzGjy+K9RtvPg0WUQqpdY5fkBAOOMj1ri/Dfxb0z4rT39vp1je2OraRcm3uPNjIQODjCv0P0qanMlzlKlGLcHb77tH1h8Pwh8Z2OzB4Y/+Omvbq+dvhPqceleKtNW/k+eUeQrHpvYcV9E19BgZKVJ+p8TmsXGuvQKKKK9E8UKKKKACiiigCqvSpEqNOlPXrQJFTVxm2f6V4Z4rG3UWr3XVebdvpXhfjAY1A1hW+EuG5b8LyYWvQ9Pl/dIPavMfDs2zPNd1pt6NqDNb0fgRjUWp0obim5ycVWjuww61Osisc962M7HGfEKxEcInUYEgw2PWvHr+ySbd5iggHvX0L4k09dT0W4hI+YKWX6ivnnUdRS0u2hnOwg456Gvl8wpqnV5ujPs8prOpS5OsTxf4jfBo+MNctdRsrxra9s23RRTjzLZ/ZkPGf9oc11mieLfGXg2GGCez02Ro9uJfspwMEkYCkep79zXfWa2txMCGHNO1fRreSMlpBt+tcNGq1oke8nTlJKrG/wB55V4x+Jvi/XLC5tZdVbTrSQMJDZxeQQpOThuWH1BzT/gj8NV0u1gZEa1stxmisoh88hJyZZWPJJ9+fWtLXtM0x7iKO5nSG0jPnTEnllXnA+pxXoI1TTtEuYtLmnWCM2/2uYKcPKMgYz2HNXOo5O0julTjyJUYKN+yGeINJ1CHW9H1G0b/AEG1dnmCt8wbja3uBz+dfVug6mms6NZ3qHImjDH2Pcfnmvhyx/aq8Maz4t/sPTpBdQRAxZgiLKrhtvl8d+tfQ/gz4mXGl2EMX2XfY9REwCuueTj/AANdeDr06Umr7nzmaYOvUpQfLt+R7XRVCy1eDULKG6hJ8qVdwz1HtU6XSt3r6Jaq6Pim+V2ZYopqsGHFOoGFFITijcPWgCqlSLUcdSL1oEivqXNu30rwzxqNt+frXuuoDNu30r5q+Nfir/hGbqzSGNZrie5jiO7OEVmwT9cVlVV4mlNNysi5pdx5TnmunsdRxt5rxq48etbSOEVFA7nk1har8SdQmZY4rp4wBuYIdv8AKsI1HBWOr6tKTPpiLWVjXLuqr6scUyTx5pNiwE2o26t12+YCfyFfIuqeNLp4fOv72ZlHIUyHGPU1zHgPxrL4v13UJovNisrbbBG+PvMx57+g/Wr+sMtYPuz3f4zfHW9mnGnaRcG20+GB7q7nBKuyKM4B7A4x+NReI4jqmk21xIozcRRzcdAWUHj8684+LZg0/wAEa7ds8T3Gox/ZISRghAORnH0r0XwZeDxB8KPC183zO+mwbvrsANeBmMnUtc93AU40vhRxFzaajpqedb3DgL0B5rNl8W6zcKYnnDDpnbg11WsSlYpEHQdq4eVtsh45zXjJtHuxdynqvw81fx9dW3kX5AhmjmmgA+Z4wwLKPqMj8aofGDxfqmj+Krm/udIvCotGt4VhG4ZJBAY9ug5r1D4WX0dt470pZmxFKXibPfcjAfrivXPF3gW01mIhrdJfUlc/SuyFNzpuSZ10MwlRqKElfQ/P79k34MazrfjuPUp3nsrZ7h5ZzGOwckg59+/vX6GeSlsghjXbGgwAeTWV4e8Pab4E077PZwpHNJyzAYJPesX4n+JT4c+HupX6MVuHURQ7eu5jjI+mSfwrsoUtG3uzzcwrt2S2PbrTXY7DTra2VgfLQA/Xv+tX7bxCrY+bn61+dvhn4z+JLHVw0eqTzRJz5UspcH2weMV9AeFP2g7e4jh/tGzdNw/1tudw/I19FCtFLlZ8NUw023Jan1Za60pUfNVz+2U25yM145oHxN8P6qyJDqsaSN0jmzGc+nIrrTOzx7lk3A8gg5BrR1I7nK4Sjozq59fRc4aqp8RL61xk08hcgsaaC+OprL6wuwuQ9SjNSrVaJsirCmukER3+BbsScACvgz9oXxBPNr8kgcAJcowyeg3cV9b/ABl8SS6JpNjbxkol1MBIw/ujHH418N/Hp3OpXIdtiRnzmY9CM5H+feuWtLZHo4WG8mJLdl2RCzM23e5/z+FVpr1YFBkPL/Mc+n+c1ZRE/syOckATgHP+yP8AP6V5x4o8RtcPcLACS37qPb6f/q/nXIz0olXxPr1x4guntoAdhOzC/wCfpXqXwm8Px6NosUeTDLLI0z8cHsM15/4E8PR3fnrvZLmN1UnGeo64+vFeyaPBqWjRMDHDeRRBQnlkKxX3DcdvWp2CTMb4827X3hLyg8eYAXXHHOKu/s8+OYtR+GOj6YzAS2sAiK/QYrI+Jupf2roL7o9sbgqeOQRx24xXnXwRF1pmnx+UrYindT/30a83GxukzvwjvdHvPiCcIz54zXJTzF5Plxgc1seIrh7iFG6ZGTiseys2kV2wSK8TU9uKVhttqUljexXEbbZYmDq3oQeK+rvBfjC08SeD49QhKy7iUdRjKuMZBr4516X7AksmDgDNdb+xvr+o3Wr+KrK7cm0udl3bwv0XB2M345X8hXo4Nyi2cOK5dGe+arMlxeFth+Xua8i/anMsfw9sJk/1Ud2DKM4/hbH616/q90Iy3yqn0FcR8U9I07xp8PdQ0y6vY7aV18yJmYffXkce/T8a7YPllqclaXtYr0PjLwtdJ9qjcqd7KeteheHp5JIk58tVJP4V58kccGuvplpDtWxHl3FweiueqZ7nH5ZrrU1RB5qwthFHlrjnn/INegk2ea5JbHcx6llVAbGT1Fdb4d8d6z4dJntb+aODdxATuVh6lTxivM7NZJPLLMdi4AzxuPU/zxW8TI/mYHJxGp9Oev6VS8iJar3j6J+H3xcHizVjpt9DHb3TKTG0eQrEdRg9+9el7wO9fHXgi4m0vxbpV2z/ADC4Qkexbn9K+sjdc9aOW55VZKD0PW4jircZyKgWOqeu3rabot7cqcPHCzL9ccV6jORbni/xi1/+37/ULJH/AOPE7IxnoR1NfI3xmuhq+kzuVP2hV8rI/iAHX+Vew+OteuIrtNXtzvLfLcR/zzXgfja8j1S2E1rNvhJLEHqp9CK8uU+aVz3qceSKRqatevafDzS59wDvZoox6kc/1/OvPfD9uLq6lkb5jCCevBb/APXj8q277Vft3wo8J5OS0Lq57jy3ZD/6DWb4HQkxgDJmckk+n+c0karRHoPw/wBKFnLcMcmSRA7H8RivSoJfMWQEYHlggflXN6RYfZEMhAVplP8A9at60cSG2X++DH/h/P8ASpZJzfxFt0k0e4jAAAQP+n/165D4MXVvNZzqcApcOD+ddl43bzLPB4Dho/8AP515L8I5Gtr/AFO3bKN57HHpzXHil7iZ6GDfvtHvuoRQ3Kqijt1FWLPR447RuMZHWqOm8RKx59Sa2J7sJbbV44ryuW9z1W7WseSfFrV9K8LabMbyYGQoWWBTl2H9B715V+yf+0fcXf7RsWmTxRWujXdjPbJHGOjBlZWJ78Lj8a+hvH3g/R/E3gyG+vtNtru5id4DJInzbc5Az6c186+DvCmnf8J/BYW1lDY28KvPL9mjVC6rgBSQMkEkZr1cPQUYX7nkYmvKc+Xoj7T+I/jiytSbazkE1wwxhOinHGTXlbX8l84llcs3UAnp6j8qpHOW5HAyPYj/AOtTbWT98ewb5wPcdv511wgo6nJKbkrHj3j2dND8V3dlbxiKNpmuCFH+sd/mJP41b0EJEwgdw0wQSygfw54A/H/GmfG2zg0zxRba5PIEtRZ4YseAUY8/iCv5Vx/gzxtp1rpcWo6zeRxX1/I1wlsMvKy/8s0CDJOAB+ZrV7GCkk7HsEWqJFLyNywjj/e//X/KrI1O8mUJCixhTyQNzZP+Fclp3iifUUUWWkSxxE5Nxf7Ygx9kJ3fmBXX6cksyq1zcoQfmdY2wAew4p7CbubHhTRrq512xjErNK8yqFJ5+8K+tfKxXivwM8PRS61c37qWS3jwhb++e/wDOvcuK6qUdLs8vEyvKy6Hsqc1xfxb1M6d4bARvmZxlR1IFdpH2r5y+LfjSWPxNd210xWBj5UaE4xirqy5YhQhzTPHvHEz2E091bfv7G5LGaL+6cdRXzz4lnWJ2msJQyAMXj7nrwRXs+v6o0MEsKsHjO5sE9etfOfxEdN5aCVrWUIxU9OefzFeaj2rHUHy5fg/4eu4gdsn2hFX+7+/kLfrWz8L9Ma9vIgoPloAucdT3/rWF4ZuZNR/Z68MXEigTK90H29Mi4cdPfivVPg7pAh0zft3MFzk96eyG3odfqaLZWUbAklXCqT396bYNh2ZTwjBl56D/ADim+O7hbLSFAwJAu0Aeveo/Dl2L62s5ehlQhgOmcf8A6vyqehJmeNYy9vcR+mJF47H/APWK8o8EDyfF2oY43vux6EivYfEaLIYpH4XBjYfp/I/pXjWjObLxvfoRgiTmufEfw0deF0qfI9z0+X92BVmebCcEdK56y1HYijOc1PPetIAifeY4AFebHXQ9Z6K7NyLGoeCdVtmJLRyiUfTof5V4j4E0kR+LNZvNvSMRqSPU5P8A6CK9l0+8j06dI5xm3YeVKB33dT+HX8K8k+Mfieb4S3tvaWNlHeahq995MImkEcUaKuSzN/wLH417dKOigjwKs1rUkdjM/lgH/bzxVZZBDPEOyvyT7/5NY3g7xHB4x8I2WpiOW3upSzPAxyq7SV4PGeQT9MVfibz5csQFc4I9DVuLTszOM1JXR4n+1wLzWF8K+HbJni+3TSmeVF3ERpszgd85FXvB/gaw0DS4Y4LY2kkoBmmY5mcf7Tcn9cV7VrOg2mvWMMN1CjyRfccj5kbpwe1cc0mxpY0WOfDeWpQc478flVNvRGSik2ypp0FkhaSNPNC8KFO7HvXRWUtopEMCncv3sD+L/wCtVGz03TtRaSN18ryV5ccEMfX8f5VoaJpGq22oLbLJ9rRseVkdQewoegH0Z8IYPsvhdJEXBmkLZ9gAB/I132+T1NYvgbRJNH8P2ltOAJUXLAdiecV03lCvQgrRSPCqS5ptnreoXTWOm3Fwo3NHGWAPc4r5W+IHk/EC7c6gWiuA4R5I8KR2yK7T9oL4r6zokT6XoCAtyssmMk8dK+PPEvxA8fR3Z8n7OrKMtvHf/JrirVU3yroevh6LUeZ9T1zVPgVLfQBLXXXDBSo+0Qg/qCK8d8ffsh+OdWs5RYahpeofKQsbSvET19QR3r6O+FniW68UeCdMvr4KuoeWI7kL08xeCfx6/jXaJKQOtdao05K6RzfWKsHZs+N/hj8CPH3hr4Op4e1rRJUvbS/neNY5lm3RuQwIKk8ZJ/KvZ/h/4TuvD+jqt3ay27cAiRCuAB/+qvaI5G25zVhZzjG4kelRLDRezLWLl1R80fE69Vn8sfchwWJ7sf8AP6Vl/Dm/cafPaOWZ4H3oCD0bkD9T+lfSmreHdF1dGW/021uAxyS8YBJ+orHh+GXhe2kMltYtbMevlStg/gSaxeHlayZssVHqjyzW4DNAxC8OPMH17/1rxa8eODxhdydCcZPuBj+lfXd38NNOvI0SO8uIdpOM4bj07VX8F/BvT/DV/dzT3EGpQzyeZsubVWK8AY5z6VzzwsqkeRux0U8ZCnLnWp4DBqsYiTDZPFbuizfaZTMw2pGNwJHU9q+q7LQtAtMNHptijDoVtUB/lXnHxQ8J6vr2pT31okJsraECOMOAzADJwAPWsFgHTalzXOl5kqqcOW1+tzySabzLgKSNsfzH3NcZ+1f9n0jwNZa7caZHqUsaIIxLnCOQF3HGD0963hciIgNhSx3tnv6f5967L4wacbv4dsq26XM0KELE6gqx24xj64raDs0zGa5otHz18EfGL+INHvbW6jgb7GEFvJp6lIeVyQQ3PB4PPevQoI/KuGXjDjcuK+dfgbrmv6b47trW6tJ/sbM/2yzlt/Jhi9CpBH6gZ9+tfSFw8cjSPEnl7HJC5yACen4V0VUlLQ5cNNyhr0LltI0jA9z6+orz/VbYR6zLLCSEiZ2wPXJP+H5V3MbiOORxx8u9f8/nXJajE8NgLuP5mY7HHUEdc/0rJHRI044ov7Jt72ZN8b8yyr97g4GR3r034GaALzxEs6zC6tYojKpbk7j6/nXBeGbiK5sJYiAY0AO3sD3r2f8AZ08DT6fLqOsklLSUeTCn985yzfyFaU1eaRy15ctNs9bWJoyMDipNr1eeAL1pvlivRPDOE8Z2kUuoma4AeSZy+DzgGvnX4tWsVpqMzRyIAz9v8/5xW94/+KevahNO1vaupYlQ2OBXN6V8FvG/xR0KfVlmtrO0TKxtduymU9yMA8Zzya8XSUrxPqYxaVjX/Zt8ZfaNe1TRJJF8qZRLbru/iUYOPqvP4V9EJHivjPwf8IfGPw88d6XqjNbr9huBJIRcDbImcMB9Vz1r7FtdYtL4nyZVf3Br0sPUjOPKnsedi6E6cuZrRlpTtUikEhReKmjXcOKiuAFUgjmuo88rz3A3DmomuwMVVuZAHHOAarPNiTYSamRSNiO59+akttQzIysSDnjmsdJgDjdzS53Z+bBByCKxkzVI6KHUgxIzyDipjeq6kHBB4IrnYWYc7s+tWVnwOtZ8xdj5p+ICSeHfF19Z4wv2ncnOP3ecj9CPyr1jW2XU9B1OHIz5RkhweoCgg/mK8v8A2yLS507wwmv6erfaZE+wMV7M33W/AbvyFXvgv4x/4SP4X6feTkyXNnGNPumPVtigAn6rtNcM1yysenTlzI+SdW0XxtN8TLm8JunT7UktoyygWyRDOVZMEHPHX34PGPpEak96tu8yRx5jETLGuBx6/mKTwr4T1Dxl4xTSdJtBeXCuSFLBflU8nk+hr9BtN+Gfhqw06FP7F09XVAG/cIcnH0rqhF1VrpY5ZOOHb63Pz3u7opbCEMVbkAiqkty1qdPhkhLRTZ8wbeAcjFfolJ4P0KM/LpNgf+3dP8KsQ6RpCoYzpNjsPUeQv+FaLD26mbxV+h+dWnQtpWrtAOYp8vn1GOP619o+ALFdK8F6PAFw4tkd/wDeYZP6mvNvjT8ELmHXLrxJpq2sWnLIjfZ4/lKLlQ3GMdf517DbgR20SAABVAwPpV0oOMnc5sTUUoxsLK2TUeKkxk0tbHFc8Wu/CGkXt7DBqF2rz3EojSBH2LvPOOOemT+FWdX8QT+GNKutCsLWX+zhGUtpoPn2ADGGB5+h5r4V8RftNa98Q/2zdJttJU2mg6VqkmmW9snQ7yYHnb1b5iR6DivqqbxPdeFLmSy1eWctkr5oiMi/jgEivm8TU5Wox0PvcFSSi5yd32PP/EfxDeyt57SW6hYpw4kfaR9QeRXF6J8U9R0vVo/sV+Yo+PlDB1b8K6b4lwaN4gtrmRxBO7odsithv8a8d0PwnpumOJQixuORlsEfSuWDkndOzPfvDkfMrp9D7w8A+MpdQ0G0utU8u1aVA3LY/Q10VzrtlMSI7hG+hFfB2m6Bqni/U5I7Ga4ljiIMk007FIx68n9BXtOka5pWhx21nJrCPcxosbHccsQOte3SxvMrOO3U+JxmXwpSvGau+iWx71LcQy9GVqhYhuwNec2vimBQcXfT1yKu23jaCRtqXkLn0DjNaPFxZ531aSO4BAP3RTg/oAK5iHxGT1GR7GrcevIepxWDxCZapNG95hA9foKkSUn3FYia5Cf4wKmTVoieHH51Htx8g/xZ4asvG/hrUNEvhiC7jKb8Z2N1Vh9CAa8U+Bfg+fwR4V8QaPqq/wClrqEokXsFXCAj2IAOfQivcY9TjYfeBI9Kpap4aj1eO6vbO5itblYZJZjKcLIqpz+OFFNy9potzWm1B67HiXwx1bUfB/xvtDZXAjjlma3dgob5W579O1fV+qeONdgjOy/GcHgxKf6e9fD3hPxtZX3i7Qbi1Zt8moQySs3YFxx+tfX2syAMAPXJ/n/SvIxtepSa9nJrTuejClCbvKKZUvPiX4mSVgL9NoJ/5Yr0FLa/EHxDIw334JGOkaj3Nc7doTx3OAfx5NNs3BJY8ZP8z/gK8lY3EN/xH951PDUUvgX3HeR6zqfiVI7G8ujLbu6M6bQMhSG/pXZLLlR61wvg8+Zepk8hSa7UfeOOlfY5bOc6HNN3bZ8xjoxjVtFWsTqcmn1Ei1Lg16h55+OvhzRJPhB+1zJ/a5XyZtUN3bXL/cljkkLKQT6E4PuK+2vinqV1pl6urIjXlpfDzYmRhz6jn3rmviX8DfAnxOMP9szaxtjbdHJFPErRv3CsIs/hmmf8ICdD8L/2BpniW/vtKjbfbR6xEbiSBsY/1iBeCe2D2rwa2CrOXNFXPsMNmNBLlm7HCeJ9UbW7ffPpUkbDnLsh/ka82utTsdGaa5vDDZW6c9cmuy8Q/Cfx1qHmGLxDbRRk9I7JlwPqxNeea7+yP4p8Qamt1e62HAAGwtgDA64PTNEcJVWjRvPM8Or8rufeXwt8BQaX8ALe5UCSe/t/7RZigBAcAqv4Lj9a+b/HOhPFcySRjBR+31r6E8P+PdY0/wCHOneH4NMidobGO080uNpCoFz19q8+v/AWu+IfMLraQB8g5Zq9OpSfKoxWx85Tr3lKdR6tnCeG/Ej26rbXzbV4VXJ6fWp9ciEU4wDiQkBlrqU+BuoyRBJr61OMEEBif5Vzvivw9N8PzZ297dC8gnLbHCn930/TmvPnQqQTk0dsK8JvlT1OOvLi8t8+Td3CF2C/LIRgdP615dq3jrX9AXVVk8Q6nAbYsvF3Jxjp3r2SW3E0sDoQVJyGU5yK8A/aitX0bUrKOFTs1VVY8f3fvfoKKDXPqOum4aGZJ8cPGOh6JJcnxNqcrBcRlrp2y3bqas+Bvjp8SJ4C8/iq/l3YZfMKtj8xXjdzdSazfLbLn7PEoVFHfkZJr1XwbooECiT5EUKpY9B05q3FIhO7PafBvx48YQa7axahrD3FpKuxt6L8pI4bgev6Zro/G3x18Wm3vrKy1BYrS5ieCRPJUsoIIbDYyDjP516xov7Cml3ejQXQ8WSteSRq6OtofK2kA/XvWxP+xJDraeY/iyPOAN8VuSGYcEnnviiWGq3XKiY4ij1/I+VPBN61he6ZNnG2VCfbDD/6xr9CtRm8y3hkB/1kYI/Ef/Xr46+J/wAMPBfwT8RLoWrePEXUPKW48pbQsVU5xnDcZxX1dZXkOpeENFvLSX7TDJaxPHKBjeu0HOPfArw8zozpwi5I9DDVYVJNRZSvJwHY/wC8R+JwP5VDDIAcDp/kf1qG9bbIQ38OAf8AgK5/maggl2gL+H+fzr5yL1PUa0O48Palb6ddWktxOkKu+z52x1B/+tXocTBm4OQeRXy18VpFkGlguA0e9gN2D1H+FfR/hifzNB0xySxa2jOT3+UV9/ldROgoW2/zPkMfH965HQpgVLkelQRnIqXNeyeYj5Rg12eTQCXSJzFOxXcp4JH19zXhXi68ub/QdQsjczQxzXSOzxOQ/HYHsDRRVmgl/wDFDWtOSNIvI2oUYBlY8quB/FWPqX7RvjC1z5T2SY9Lf/69FFMDI039oPxnczpF9uhhTOAsUIGK9L0z4n+JJ7ZGfUXJIznAFFFc027GsUhdS+IfiFbJpBqUobcBwfU1FPdXPiG7RdQuZbhY+VDNRRXmVZSbs2dsIpapE20RSQogCIOgUYAryr9oeJbi60fzAH8lPlyAep5oorBm6PIItItbXUZBHEq4Iwdo/wAK9E0O2jls5EZRtZOeBRRUpjPor4I/tQ+MZNDsNHuI9Nu4LNBapNNA/msifKNxDgE4A5xXtPhTx9deFdBFlplhZW9sryShD5r/ADOxZuWcnqT3oor2INuKuebNK5+b3x78Z6h8RPih8Qta1kRS3qBbeLy1IWFFKouwZOOB79TX6HfAmZ5/gB4Dkdiz/wBkwrk+y4oor57OP4UT1cv/AIki/qB3XDZ7nn8XOf5CqaMSQffNFFfIR3Po+h1kXgHR/FNho76hA8jSyzIzK5U4UKR/OvVbDSbewsYLeEMI4UWNATngDAoor7nA6QjbsfLYrWTuW4FwasUUV7h5Fj//2Q==",
        //            "Andrew received his BTS commercial in 1994 and a Ph.D. in international marketing from the University of Dallas in 2001.  He is fluent in French and Italian and reads German.  He joined the company as a sales representative, was promoted to sales manager in January 2009 and to vice president of sales in March 2010.  Andrew is a member of the Sales Management Roundtable, the Seattle Chamber of Commerce, and the Pacific Rim Importers Association.",
        //            ""
        //        };
        //        dt.Rows.Add(row);
        //    }
        //    DataSet ds = new DataSet();
        //    dt.TableName = "Employees";
        //    ds.Tables.Add(dt);


        //    // ViewBag.WebReport = IPrintService.Report(ds, "Simple List", "NorthWind");
        //    return View();
        //}

        //[HttpGet("testAuth")]
        //public async Task<IActionResult> TestReportOfStore([FromQuery] DetailedMovementOfanItemReqest param)
        //{
        //    var res = _rpt_Store.DetailedMovementOfanItem(param);
        //    return Ok(res);
        //}


        //[HttpPost("TestInvoicesIntegrationService")]
        //public async Task<IActionResult> TestInvoicesIntegrationService([FromBody] PurchasesJournalEntryIntegrationDTO parm)
        //{
        //    var add = await _iInvoicesIntegrationService.InvoiceJournalEntryIntegration(parm);
        //    return Ok(add);
        //}




        [HttpGet("SystemHistoryLogs")]
        public async Task<IActionResult> SystemHistoryLogs(SystemActionEnum systemActionEnum)
        {
            var res = await _systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);

            return Ok(res);
        }
        [HttpGet("GetImage")]
        public IActionResult GetImage()
        {
            Byte[] b = System.IO.File.ReadAllBytes(@"F:\\Screenshot 2022-08-21 102421.png");   // You can use your own method over here.         
            return File(b, "image/jpeg");
        }
        [HttpGet("GetPDF")]
        public IActionResult GetPDF()
        {
            Byte[] b = System.IO.File.ReadAllBytes(@"D:\DeskTop\Received Files\مرتجع مشتريات.pdf");   // You can use your own method over here.         
            return File(b, "application/pdf");
        }
        [HttpGet("DownLoadPDF")]
        public IActionResult DownLoadPDF()
        {
            Byte[] b = System.IO.File.ReadAllBytes(@"D:\DeskTop\Received Files\مرتجع مشتريات.pdf");   // You can use your own method over here.         
            return File(b, "application/pdf", "FileDownloadName.ext");
        }
        [HttpGet("GetDaysCount")]
        public IActionResult GetDaysCount()
        {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var days = (date - firstDayOfMonth).Days;

            return Ok(days);
        }
        [HttpGet("GetOrigin")]
        [AllowAnonymous]
        public IActionResult getOrigin()
        {
            var origin = _httpContext.HttpContext.Request.Headers["Origin"].FirstOrDefault();
            return Ok(origin);

        }
        [AllowAnonymous]
        [HttpGet("print")]
        public IActionResult print()
        {

            using (MemoryStream ms = new MemoryStream())
            {
                
                return File(ms.ToArray(), "application/pdf", "file.pdf");
            }
        }
    }


}
