using System.IO;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Application.Services.Process.Inv_General_Settings;
using static App.Domain.Enums.Enums;
using static App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice.AccrediteInvoice;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using System.Web.Http.Results;
using Microsoft.Extensions.Configuration;
using App.Application.Services.Printing.CompanyData;
using App.Application.Services.Printing.GenralPrint;

namespace App.Application.Services.Printing.InvoicePrint 
{
    public class PrintService : IPrintService
    {
        private readonly IGetInvoiceByIdService GetSalesServiceById;

        readonly private ICompanyDataService _CompanyDataService;
        private readonly IConfiguration _configuration;
        //shora
        private readonly IHistoryInvoiceService HistoryInvoiceService;

        private readonly IInvGeneralSettingsService _generalSttings;
        private readonly IRepositoryQuery<InvGeneralSettings> settingService;
       
        private readonly IGetTempInvoiceByIdService _iGetOfferPriceByIdservice;
        
        private readonly ICreateDataTable _createDataTable;
        public PrintService(IGetInvoiceByIdService GetSalesServiceById,
            ICompanyDataService CompanyDataService,
            IHistoryInvoiceService _HistoryInvoiceService,
            IInvGeneralSettingsService generalSttings,
            IRepositoryQuery<InvGeneralSettings> settingService,
            iUserInformation iUserInformation,
            IGetTempInvoiceByIdService iGetOfferPriceByIdservice,
            IConfiguration configuration
,
            ICreateDataTable createDataTable)
        {
            this.GetSalesServiceById = GetSalesServiceById;

            _CompanyDataService = CompanyDataService;

            HistoryInvoiceService = _HistoryInvoiceService;
            _generalSttings = generalSttings;
            this.settingService = settingService;
           
            _iGetOfferPriceByIdservice = iGetOfferPriceByIdservice;
            _configuration = configuration;
            _createDataTable = createDataTable;
           
        }

        

        public async Task<WebReport> ReportInvoice(byte[] fileContents, int invoiceId, string employeeNameAr, string employeeNameEn, exportType exportType, bool isArabic, bool isPOS = false, bool isPriceOffer = false)
        {
            var invoiceDto = new InvoiceDto();
            if (isPriceOffer)
                invoiceDto = await _iGetOfferPriceByIdservice.GetInvoiceDto(invoiceId, false);
            else
                invoiceDto = await GetSalesServiceById.GetInvoiceDto(invoiceId, false);

            if (invoiceDto != null)
            {
                


                var companydata = await _CompanyDataService.GetCompanyData();
                var mainCompantData = (CompanyDataDto)companydata.Data;
               
                
                    RoundedNumbers(ref invoiceDto);

                
                var settings = await _generalSttings.GetSettingsForprint();
                if (settings.Other_PrintSerials)
                {
                    CheckPrintSerials(ref invoiceDto);
                }


                  

                if (invoiceDto.PaymentsMethods.Count == 0)
                {
                    invoiceDto.PaymentsMethods = new List<PaymentListDto>()
                    {
                        new PaymentListDto()
                    };
                }
                
                var tablesNames = new TablesNames()
                {
                    ObjectName = "InvoiceMaster",
                    FirstListName = "InvoiceDetails",
                    SecondListName = "PaymentMethods"
                };
                ReportOtherData otherData = new ReportOtherData()
                {
                    EmployeeName = employeeNameAr,
                    EmployeeNameEn = employeeNameEn,

                    Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                };
                if (Lists.salesInvoicesList.Contains(invoiceDto.InvoiceTypeId) ||
                    Lists.POSInvoicesList.Contains(invoiceDto.InvoiceTypeId))
                {
                    invoiceDto.QRCode = QRCode.GetQRCode(isArabic ? mainCompantData.ArabicName : mainCompantData.LatinName, mainCompantData.TaxNumber != null ? mainCompantData.TaxNumber : "", invoiceDto.InvoiceDate.ToString(), invoiceDto.Net.ToString(), invoiceDto.TotalVat.ToString(), _configuration: _configuration).URL;

                }
                var dataToCreateDataTable = new DataToCreateDataTable<InvoiceDto, InvoiceDetailsDto, PaymentListDto>()
                {
                    DataObjet = invoiceDto,
                    FirstList = invoiceDto.InvoiceDetails,
                    SecondList = invoiceDto.PaymentsMethods
                };
                 

                var numberToText = ConvertNumberToText.GetText(invoiceDto.Net.ToString(), isArabic);
                var dataTables = await _createDataTable.CreateDataTables(dataToCreateDataTable, tablesNames, otherData, false);
                var report = await RegisterDataInReportFile.Report(dataTables, fileContents, exportType);
                SetPrintToInvoiceHistory(invoiceDto, exportType);

                report.Report.SetParameterValue("Currency", numberToText);

                return report;
            }
            else
            {
                return null;
            }

        }

        public void RoundedNumbers(ref InvoiceDto invoiceDto)
        {
            foreach (var item in invoiceDto.InvoiceDetails)
            {


                item.Price = Math.Round(item.Price, 2);

                item.VatValue = Math.Round(item.VatValue, 2);
                item.Quantity = Math.Round(item.Quantity, 2);
                item.Total = Math.Round(item.Total, 2);





            }

            invoiceDto.Paid = Math.Round(invoiceDto.Paid, 2);
            invoiceDto.Remain = Math.Round(invoiceDto.Remain, 2);
            invoiceDto.VirualPaid = Math.Round(invoiceDto.VirualPaid, 2);
            invoiceDto.Net=Math.Round(invoiceDto.Net,2);
        }
        public void CheckPrintSerials(ref InvoiceDto invoiceDto)
        {
            
            {
                foreach (var item in invoiceDto.InvoiceDetails)
                {
                    if (item.InvoiceSerialDtos.Count() > 0)
                    {
                        foreach (var serial in item.InvoiceSerialDtos)
                        {

                            if (item.SerialNumbers != null)
                            {
                                item.SerialNumbers += " - " + serial.SerialNumber;
                            }
                            else
                            {
                                item.SerialNumbers = serial.SerialNumber;
                            }

                        }
                    }
                    else
                        item.SerialNumbers = null;

                }
            }
        }
        public void SetPrintToInvoiceHistory( InvoiceDto invoiceDto,exportType type)
        {
            try
            {
                bool isPrintWithSave = false;

                if (Lists.purchasesInvoicesList.Contains(invoiceDto.InvoiceTypeId) )
                {
                    isPrintWithSave =  settingService.TableNoTracking.FirstOrDefault().Purchases_PrintWithSave;

                }
                else if (Lists.salesInvoicesList.Contains(invoiceDto.InvoiceTypeId) )
                {
                    isPrintWithSave = settingService.TableNoTracking.FirstOrDefault().Sales_PrintWithSave;

                }
                else if (Lists.POSInvoicesList.Contains(invoiceDto.InvoiceTypeId))
                {
                    isPrintWithSave = settingService.TableNoTracking.FirstOrDefault().Pos_PrintWithEnding;

                }
                //دى عشان لما اجى اعمل مرتجع ميسمعش فى سجل الفاتورة الاصلية
                string parentCode = invoiceDto.ParentInvoiceCode;
                if (Lists.returnInvoiceList.Contains(invoiceDto.InvoiceTypeId) && isPrintWithSave)
                {
                    parentCode = "";
                }


                HistoryInvoiceService.HistoryInvoiceMaster(invoiceDto.BranchId, invoiceDto.Notes, invoiceDto.BrowserName, lastTransactionAction: type.ToString(), null, invoiceDto.BookIndex, invoiceDto.Code
             , Convert.ToDateTime(invoiceDto.InvoiceDate), invoiceDto.InvoiceId, invoiceDto.InvoiceType, invoiceDto.InvoiceTypeId, (int)SubType.Nothing, invoiceDto.IsDeleted, parentCode, invoiceDto.Serialize, invoiceDto.StoreId, invoiceDto.TotalPrice);
               
            }
            catch (Exception e)
            {
               
            }



        }
    }

}
