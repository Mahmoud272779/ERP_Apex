using System;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.Invoices.Purchase.IPurchasesServices;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.Store.Invoices;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Utils;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process.Store.Invoices.Purchases
{
    public class AccreditInvoiceController : ApiStoreControllerBase
    {
        private readonly IAccrediteInvoice AccrediteInvoice;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly Itestfiles testfilesService;
        private readonly IprintFileService _iPrintFileService;

        public AccreditInvoiceController(IAccrediteInvoice accrediteInvoice, iAuthorizationService iAuthorizationService,

        IActionResultResponseHandler ResponseHandler, IprintFileService iPrintFileService) : base(ResponseHandler)
        {
            AccrediteInvoice = accrediteInvoice;
            _iAuthorizationService = iAuthorizationService;
            _iPrintFileService = iPrintFileService;
        }


        [HttpGet(nameof(GetPymentAccreditInvoice))]
        public async Task<ResponseResult> GetPymentAccreditInvoice([FromQuery] AccreditInvoiceRequest parameter)
        {
            var isAuthorized = await isAuthorizedUser(parameter.InvType);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await AccrediteInvoice.GetAccrediteInvoicPaymentTypeData(parameter);
            return result;
        }
        [HttpGet(nameof(GetAccrediteInvoiceDataCount))]
        public async Task<ResponseResult> GetAccrediteInvoiceDataCount([FromQuery] AccreditInvoiceRequest parameter)
        {
            var isAuthorized = await isAuthorizedUser(parameter.InvType);
            if (isAuthorized != null)
                return isAuthorized;

            
            var result = await AccrediteInvoice.GetAccrediteInvoiceDataCount(parameter);
            return result;
        }

        [HttpGet(nameof(GetAccrediteInvoicSafeBankData))]
        public async Task<ResponseResult> GetAccrediteInvoicSafeBankData([FromQuery] AccreditInvoiceRequest parameter)
        {
            var isAuthorized = await isAuthorizedUser(parameter.InvType);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await AccrediteInvoice.GetAccrediteInvoicSafeBankData(parameter);
            return result;
        }
        [HttpGet(nameof(GetAccrediteInvoiceData))]
        [AllowAnonymous]
        public async Task<ResponseResult> GetAccrediteInvoiceData([FromQuery] AccreditInvoiceRequest parameter)
        {
            var isAuthorized = await isAuthorizedUser(parameter.InvType);
             if (isAuthorized != null)
                return isAuthorized;
            var result = await AccrediteInvoice.GetAccrediteInvoiceData(parameter);
            return result;
        }

        [HttpGet(nameof(AccrediteInvoiceReport))]
        
        public async Task<IActionResult> AccrediteInvoiceReport([FromQuery] AccreditInvoiceRequest parameter, InvoiceClosing invoiceClosing,int screenId,  exportType exportType,bool isArabic,int fileId)
        {
            var isAuthorized = await isAuthorizedUser(parameter.InvType);
            if (isAuthorized != null)
                return Ok(isAuthorized);

            WebReport report = new WebReport();
            report = await AccrediteInvoice.GetAccrediteInvoiceDataReport(parameter,   invoiceClosing, screenId, exportType, isArabic,fileId);
            string reportName = "";
            if ((int)invoiceClosing == (int)InvoiceClosing.ClosingInvoicesPeriod)
            {
                reportName = "Closing_Invoices_Period";
            }
            else if ((int)invoiceClosing == (int)InvoiceClosing.ClosingReturnsPeriod)
            {
                reportName = "Closing_Returns_Period";
            }
            if ((int)invoiceClosing == (int)InvoiceClosing.ClosingPaymentsAndReceipts)
            {
                reportName = "Closing_Payments_And_Receipts";
            }

            return Ok(_iPrintFileService.PrintFile(report, reportName, exportType));
           
        }

        //authintication
        private async Task<ResponseResult> isAuthorizedUser(int invType)
        {
            MainFormsIds mainFormsIds = new MainFormsIds();
            SubFormsIds subFormsIds = new SubFormsIds();
            if (invType == (int)DocumentType.Sales || invType == (int)DocumentType.ReturnSales)
            {
                mainFormsIds = MainFormsIds.Sales;
                subFormsIds = SubFormsIds.SalesClosing_Sales;
            }
            else if (invType == (int)DocumentType.Purchase || invType == (int)DocumentType.ReturnPurchase)
            {
                mainFormsIds = MainFormsIds.Purchases;
                subFormsIds = SubFormsIds.PruchasesClosing_Purchases;
            }
            else if (invType == (int)DocumentType.POS || invType == (int)DocumentType.ReturnPOS)
            {
                mainFormsIds = MainFormsIds.Sales;
                subFormsIds = SubFormsIds.POSClosing_Sales;
            }
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)mainFormsIds, (int)subFormsIds, Opretion.Open);

            return isAuthorized;
        }

        [HttpPost(nameof(updateaccrediteAllInvoice))]
        public async Task<ResponseResult> updateaccrediteAllInvoice([FromQuery] AccreditInvoiceRequest parameter)
        {

            var isAuthorized = await isAuthorizedUser(parameter.InvType);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await AccrediteInvoice.AccrediteInvoiceLast(parameter);
            return result;
        }

        //[HttpPost(nameof(fileTest))]
        //public async Task<ResponseResult> fileTest([FromForm] AccreditInvoiceRequest parameter)
        //{

        //    var result =parameter;
        //    return new ResponseResult();
        //}
        //[HttpGet(nameof(GetTest))]
        //public async Task<ResponseResulttest> GetTest([FromQuery] AccreditInvoiceRequest parameter)
        //{
        //       string filePath = @"C:\Users\Administrator\Music\Group -1.png";
        //    AccreditInvoiceRequest res = new AccreditInvoiceRequest()
        //    {
        //        InvType = 6,
        //        fileb = AccrediteInvoice. ReadAllBytes(filePath)
        //    };
       
        //    return new ResponseResulttest() { Data=res,DataFile = AccrediteInvoice.ReadAllBytes(filePath) };
        //}
        //public byte[] ReadAllBytes(string fileName)
        //{
        //    byte[] buffer = null;
        //    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        //    {
        //        buffer = new byte[fs.Length];
        //        fs.Read(buffer, 0, (int)fs.Length);
        //    }
       //     return new ResponseResulttest() { Data = res,DataFile = new FormFile(stream, 0, arr.Length, "name", "fileName")  };

        //    return buffer;
        //}

    }
}
