using App.Domain.Models.Request.ReportFile;
using App.Domain.Entities.Process.Store.Barcode;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Printing.InvoicePrint;
using Microsoft.Net.Http.Headers;

namespace App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice
{

    public class FilesMangerService : BaseClass, IFilesMangerService
    {

        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryQuery<ReportManger> ReportMangerQuery;
        private readonly IRepositoryCommand<ReportManger> ReportMangerCommand;
        private readonly IRepositoryCommand<ReportFiles> ReportFileCommand;
        private readonly IRepositoryQuery<ReportFiles> ReportFileQuery;
        private readonly iUserInformation _iUserInformation;



        private readonly IPrintService iPrintService;
        private readonly IprintFileService _printFileService;

        private readonly IRepositoryQuery<BarcodePrintFiles> barcodeFilesQuery;
        private readonly IRepositoryCommand<BarcodePrintFiles> barcodeFilesCommand;


        //IPrintService iPrintService,
        //IprintFileService printFileService
        public FilesMangerService(IRepositoryQuery<ReportManger> reportMangerQuery,
                                    IHttpContextAccessor _httpContext,
                                    IPrintService iPrintService,
                                    IprintFileService printFileService,
                                    IRepositoryQuery<BarcodePrintFiles> barcodeFilesQuery,
                                    IRepositoryCommand<BarcodePrintFiles> barcodeFilesCommand,
                                    IRepositoryQuery<ReportFiles> ReportFileQuery,
                                    iUserInformation iUserInformation,
                                    IRepositoryCommand<ReportFiles> reportFileCommand) : base(_httpContext)
        {
            ReportMangerQuery = reportMangerQuery;
            this.iPrintService = iPrintService;
            _printFileService = printFileService;
            httpContext = _httpContext;
            this.barcodeFilesQuery = barcodeFilesQuery;
            this.barcodeFilesCommand = barcodeFilesCommand;
            this.ReportFileQuery = ReportFileQuery;
            _iUserInformation = iUserInformation;
            ReportFileCommand = reportFileCommand;
        }



        public async  Task<ReportFileRequest> GetReportPrintFiles(int ScreenId, bool IsArabic,int ClosingStep=0, int fileId = 0)//first one only
        {
            var BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();         //  httpContext.HttpContext.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());
                                                                                                                 //    BrowserName = "Android";

            string fileName = "";
            if (ClosingStep != 0)
            {
                 fileName = ReturnClosingScreens.ClosingPrintFilesScreens().Where(l => l.ScreenId == ScreenId && l.ClosingStep == ClosingStep).FirstOrDefault().FileName;

            }

            var reportmanger = new ReportManger();
            if (fileId > 0)
            {
              var file= await GetPrintFileById(fileId);
                reportmanger = (ReportManger)file.Data;
            }
            else
            {
                reportmanger = await  ReportMangerQuery.TableNoTracking
                .Include(h => h.ScreenNames)
                .Include(h => h.Files)
                .OrderByDescending(h => h.Files.Id)
               .Where(c => c.IsArabic == IsArabic && c.screenId == ScreenId && c.Files.IsDefault==true  && (ClosingStep > 0 ? c.Files.ReportFileName == fileName : (BrowserName.Contains("Dart")? c.Files.ReportFileName.Contains("Android") : !c.Files.ReportFileName.Contains("Android"))))
                 .FirstOrDefaultAsync();
                if (reportmanger == null)
                {
                    reportmanger = await ReportMangerQuery.TableNoTracking
                .Include(h => h.ScreenNames)
                .Include(h => h.Files)
                .OrderByDescending(h => h.Files.Id)
               .Where(c => c.IsArabic == IsArabic && c.screenId == ScreenId && (ClosingStep > 0 ? c.Files.ReportFileName == fileName : !c.Files.ReportFileName.Contains("Android")))
                 .FirstOrDefaultAsync();
                }
            }
            
            if (reportmanger != null)
            {
                return new ReportFileRequest()
                {
                    Files = reportmanger.Files.Files,
                    ReportFileName = reportmanger.Files.ReportFileName,
                    Id = reportmanger.Id
                };
            }
            return new ReportFileRequest();
        }


        //for invoices
        public async Task<ReportsReponse> GetInviocePrintFile(InvoiceDTO invoiceDto,string employeeNameAr,string employeeNameEn,bool isPOS=false)
        {
            WebReport webReport = new WebReport();

            var fileContents = await GetReportPrintFiles(invoiceDto.screenId, invoiceDto.isArabic,0,invoiceDto.fileId);
            webReport = await iPrintService.ReportInvoice(fileContents.Files, invoiceDto.invoiceId, employeeNameAr, employeeNameEn, invoiceDto.exportType,invoiceDto.isArabic, isPOS, invoiceDto.isPriceOffer);
           
            return await _printFileService.PrintFile(webReport, invoiceDto.invoiceCode, invoiceDto.exportType, isPOS);
        }
        //public string GetFilesNamesForClosing(int screenId,int ClosingStep)
        //{
        //   var FileName= ReturnClosingScreens.ClosingPrintFilesScreens().Where(l => l.ScreenId == screenId && l.ClosingStep == ClosingStep).FirstOrDefault().FileName;
        //   // string FileName = lisNames.Where(l => l.ScreenId == screenId && l.ClosingStep == ClosingStep).FirstOrDefault().FileName;
        //    return FileName;
        //}

        public async Task<ResponseResult> GetAllPrintFiles(int ScreenId)
        {
            var reportmanger = await ReportMangerQuery.TableNoTracking.Include(h => h.ScreenNames)
               .Include(h => h.Files)
               .Where(c => c.screenId == ScreenId)
               .Select(h=> new { latineName= h.Files.ReportFileName,arabicName= h.Files.ReportFileNameAr, fileId= h.ArabicFilenameId ,isArabic=h.IsArabic,isDefualt=h.Files.IsDefault })
               .ToListAsync();
            return new ResponseResult() { Data = reportmanger, Result = Result.Success };
        }

        public async Task<ResponseResult> GetPrintFileById(int FileId)
        {
            var reportmanger = await ReportMangerQuery.TableNoTracking
             .Include(h => h.Files)
             .Where(c => c.ArabicFilenameId == FileId )
             .FirstOrDefaultAsync();

            return new ResponseResult() { Data = reportmanger, Result = Result.Success };
        }

        public async Task<ResponseResult> AddFileToScreen(int fileId, int screenId, bool isArabic)
        {
            var reportManager = new ReportManger
            {
                IsArabic = isArabic,
                Copies = 1,
                ArabicFilenameId = fileId,
                screenId = screenId
            };
           await ReportMangerCommand.AddAsync(reportManager);
            return new ResponseResult() { Data = reportManager, Result = Result.Success };

        }

        public async Task<ResponseResult> SetFileAsDefault(int fileId, int screenId)
        {



            var isArabic = ReportFileQuery.TableNoTracking.Where(f => f.Id == fileId).FirstOrDefault().IsArabic;
                
             //   ReportMangerQuery.TableNoTracking
             // .Include(h => h.ScreenNames)
             // .Include(h => h.Files)
             // .OrderByDescending(h => h.Files.Id)
             //.Where(c => c.screenId == screenId && !c.Files.ReportFileName.Contains("Android") && c.Files.Id == fileId).Select(a => a.Files).FirstOrDefault().IsArabic;
               
            var reportFile = await ReportMangerQuery.TableNoTracking
              .Include(h => h.ScreenNames)
              .Include(h => h.Files)
              .OrderByDescending(h => h.Files.Id)
             .Where(c => c.screenId == screenId && !c.Files.ReportFileName.Contains("Android")&&c.IsArabic==isArabic).Select(a => a.Files).ToListAsync()
               ;
                if (reportFile.Count() > 0)
                {

                    foreach (var item in reportFile)
                    {
                        if(item.Id== fileId)    
                            item.IsDefault= true;
                        else
                        item.IsDefault = false;


                    }

                    //await ReportFileCommand.UpdateAsyn(reportmanger);
                }


            
                //var defaultFile = await ReportFileQuery.TableNoTracking.Where(r => r.Id==fileId).FirstOrDefaultAsync();
                //defaultFile.IsDefault = false;
                //var newDefaultFile = ReportFileQuery.TableNoTracking.Where(r => r.Id == fileId).FirstOrDefault();
                //newDefaultFile.IsDefault = true;
                //reportmanger.Add(newDefaultFile);
                await ReportFileCommand.UpdateAsyn(reportFile);
               // await ReportFileCommand.SaveAsync();
                return new ResponseResult() { Id = fileId, Result = Result.Success };
            



        }

        public async Task<ReportFileRequest> GetBarcodePrintFile(int fileId)
        {
            
              
            var file= barcodeFilesQuery.TableNoTracking.Where(b=>b.Id==fileId).FirstOrDefault();
            return new ReportFileRequest() { Files = file.File };
        }

        public async Task<ResponseResult> BarcodePrintFiles()
        {
            var files = barcodeFilesQuery.TableNoTracking.Select(x=> new
            {
                x.Id,
                x.ArabicName,
                x.LatineName,
                x.IsDefault
            }).ToList();
            return new ResponseResult() { Data = files };
        }

        public async Task<ResponseResult> SetBarcodeDefautFile(int fileId)
        {
           var oldDefaultFile= barcodeFilesQuery.TableNoTracking.Where(x=>x.IsDefault==true).FirstOrDefault();
            var listBarodeFiles = new List<BarcodePrintFiles>();
            if (oldDefaultFile != null)
            {
                if(oldDefaultFile.Id==fileId)
                {
                    return new ResponseResult() { Result = Result.Success };

                }
                    oldDefaultFile.IsDefault = false;
                listBarodeFiles.Add(oldDefaultFile);

            }

            var newDefaultFile = barcodeFilesQuery.TableNoTracking.Where(x => x.Id == fileId).FirstOrDefault();
            newDefaultFile.IsDefault = true;
            listBarodeFiles.Add(newDefaultFile);
            await barcodeFilesCommand.UpdateAsyn(listBarodeFiles);
           // await barcodeFilesCommand.SaveChanges();

            return new ResponseResult() { Result=Result.Success };

        }

        public async  Task<ResponseResult> UserPermisionsForBarcodePrint()
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            if(!userInfo.otherSettings.AllowPrintBarcode)
                return new ResponseResult { Result=Result.UnAuthorized, ErrorMessageAr = "ليس لديك صلاحية", ErrorMessageEn = "You have no permission", Note = "UnAuthorized" };
            else
                return new ResponseResult { Result=Result.Success };
           
        }

        public async Task<ResponseResult> GetPOS_RPOS_PrintFilesByDate(DateTime date)
        {
            var data = await ReportFileQuery.TableNoTracking
                .Where(q => q.uTime >= date && q.reportmanger.All(y => y.screenId == (int)SubFormsIds.POS || y.screenId == (int)SubFormsIds.returnPOS) && q.ReportFileName != "POSInvoiceAndroid")
                .Select(s => new
                {
                    s.Id,
                    s.ReportFileName,
                    s.IsArabic,
                    s.IsReport,
                    s.IsDefault,
                    s.ReportFileNameAr,
                    s.uTime,
                    ScreenIds = s.reportmanger.Select(x => x.screenId),
                    s.Files
                }).ToListAsync();

            return new ResponseResult() { Data = data, Id = null, Result = Result.Success };
        }

        public async Task<ResponseResult> SetFilesArabicNames()
        {
         var filesManager=  GetListOfFile.ReportFilesList();
            var filesFromDb = await ReportFileQuery.TableNoTracking.ToListAsync();
            try
            {
                foreach (var file in filesFromDb)
                {
                    foreach (var fileManger in filesManager)
                    {
                        if (file.ReportFileName == fileManger.reportName)
                        {
                            file.ReportFileNameAr = fileManger.reportNameAr;
                            break;

                        }
                    }
                }
                return new ResponseResult() { Result = Result.Success };

            }
            catch ( Exception e)
            {
                return new ResponseResult() { Result = Result.Failed };

            }


        }
    }

      

      
    }


