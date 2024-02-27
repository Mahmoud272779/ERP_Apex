using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Branches;
using App.Application.Services.Process.Employee;
using App.Application.Services.Process.GLServices.Prnters;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.General;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.UserManagementDB;
using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.Wordprocessing;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process.GeneralLedger
{
    public class PrinterController : ApiGeneralLedgerControllerBase
    {
        private readonly IPrinterService printerService;
        private readonly iAuthorizationService _authorizationService;

        public PrinterController(IPrinterService printerService
            , iAuthorizationService authorizationService
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            this.printerService = printerService;
            _authorizationService = authorizationService;
        }
        [HttpPost("AddPrinter")]
        public async Task<ResponseResult> AppPrinter([FromBody] PrinterRequestsDTOs.Add parameter)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Printers, Opretion.Add);
            if (isAutorized != null)
                return isAutorized;
            var result = await printerService.AddPrinter(parameter);
            return result;
        }
        [HttpGet("GetAllPrinters")]
        public async Task<ResponseResult> GetAllPrinters(int PageNumber, int PageSize, string? Name,
            int Status)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Printers, Opretion.Open);
            if (isAutorized != null)
                return isAutorized;
            PrinterRequestsDTOs.Search parameters = new PrinterRequestsDTOs.Search()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchCriteria = Name,
                Status = Status
            };
            var result = await printerService.GetAllPrinterData(parameters);
            return result;

        }
        [HttpGet("GetPrinterById/{id}")]
        public async Task<ResponseResult> GetPrinterById(int id)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Printers, Opretion.Open);
            if (isAutorized != null)
                return isAutorized;
            var result = await printerService.GetPrinterById(id);

            return result;


        }
        [HttpGet("GetAllPrinterByBranchDataDropDown")]
        public async Task<ResponseResult> GetAllPrinterByBranchDataDropDown(int branchId)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Printers, Opretion.Open);
            if (isAutorized != null)
                return isAutorized;
            var result = await printerService.GetAllPrinterByBranchDataDropDown(branchId);

            return result;


        }

        [HttpPut("UpdatePrinter")]
        public async Task<ResponseResult> UpdatePrinter(PrinterRequestsDTOs.Update parameter)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Printers, Opretion.Edit);
            if (isAutorized != null)
                return isAutorized;
            var result = await printerService.UpdatePrinter(parameter);
            return result;

        }


        [HttpDelete("DeletePrinter")]
        public async Task<ResponseResult> DeletePrinter([FromBody] int[] Ids)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Printers, Opretion.Delete);
            if (isAutorized != null)
                return isAutorized;
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            {
                Ids = Ids
            };
            var Delete = await printerService.DeletePrinter(parameter);

            return Delete;

        }

        [HttpGet("GetAllPrinterHistory")]
        public async Task<ResponseResult> GetAllPrinterHistory(int Id)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Printers, Opretion.Open);
            if (isAutorized != null)
                return isAutorized;
            var result = await printerService.GetAllPrinterHistory(Id);
            return result;

        }

        [HttpPut("UpdatePrinterStatus")]
        public async Task<ResponseResult> UpdatePrinterStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Printers, Opretion.Edit);
            if (isAutorized != null)
                return isAutorized;
            var result = await printerService.UpdateStatus(parameter);

            return result;
            return null;

        }

        [HttpGet("GetPrintersByDate")]
        public async Task<ResponseResult> GetPrintersByDate(DateTime date, int PageNumber, int PageSize = 10)
        {

            var branches = await printerService.GetPrinterByDate(date, PageNumber, PageSize);
            return branches;

        }
    }
}
