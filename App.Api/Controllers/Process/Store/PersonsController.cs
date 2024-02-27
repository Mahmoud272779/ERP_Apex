using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Persons;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Common;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using App.Application.Handlers.Persons;
using DocumentFormat.OpenXml.Spreadsheet;
using App.Application.Services.Process.Unit;
using App.Application.Services.Printing.PrintFile;
using App.Application.Handlers.Persons.GetPersonsByDate;

namespace App.Api.Controllers.Process
{
    public class PersonsController : ApiStoreControllerBase
    {
        private readonly IPersonService PersonService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _printFileService;

        public PersonsController(IPersonService _PersonService, iAuthorizationService iAuthorizationService,
                       IActionResultResponseHandler ResponseHandler, IprintFileService printFileService) : base(ResponseHandler)
        {
            PersonService = _PersonService;
            _iAuthorizationService = iAuthorizationService;
            _printFileService = printFileService;
        }


        [HttpPost(nameof(AddPerson))]
        public async Task<ResponseResult> AddPerson(personRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(parameter.IsSupplier? (int)MainFormsIds.Purchases : (int)MainFormsIds.Sales, parameter.IsSupplier ? (int)SubFormsIds.Suppliers_Purchases : (int)SubFormsIds.Customers_Sales, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            return await PersonService.AddPerson(parameter);            
        }


        [HttpGet("GetListOfPersons")]

        public async Task<ResponseResult> GetListOfPersons([FromQuery] string? Type, [FromQuery] PersonsSearch parameters, string? ids)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(parameters.IsSupplier.Value ? (int)MainFormsIds.Purchases : (int)MainFormsIds.Sales, parameters.IsSupplier.Value ? (int)SubFormsIds.Suppliers_Purchases : (int)SubFormsIds.Customers_Sales, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            int[] supplierTypes = null;
            if (!string.IsNullOrEmpty(Type))
                supplierTypes = Array.ConvertAll(Type.Split(','), s => int.Parse(s));

            parameters.TypeArr = supplierTypes;
            var add = await PersonService.GetListOfPersons(parameters,ids);
            return add;
        }

        [HttpGet("SupplierCustomerReport")]

        public async Task<IActionResult> SupplierCustomerReport([FromQuery] string? Type, [FromQuery] PersonsSearch parameters, string? ids, exportType exportType, bool isArabic,int fileId, bool isSearchData = true)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(parameters.IsSupplier.Value ? (int)MainFormsIds.Purchases : (int)MainFormsIds.Sales, parameters.IsSupplier.Value ? (int)SubFormsIds.Suppliers_Purchases : (int)SubFormsIds.Customers_Sales, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            int[] supplierTypes = null;
            if (!string.IsNullOrEmpty(Type) && Type != "0")
                supplierTypes = Array.ConvertAll(Type.Split(','), s => int.Parse(s));

            parameters.TypeArr = supplierTypes;
            WebReport report = new WebReport();
            report = await PersonService.SupplierCutomerReport(parameters, isArabic, exportType, ids,fileId, isSearchData);
            string reportName = "";
            if (parameters.IsSupplier == true)
            {
                reportName = "Supplier-Purchases-Report";
            }
            else
                reportName = "Customer-Sales-Report";

            return Ok(_printFileService.PrintFile(report, reportName, exportType));
        }



        


        [HttpPut(nameof(UpdatePersons))]
        public async Task<ResponseResult> UpdatePersons(UpdatePersonsRequest parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(parameters.IsSupplier ? (int)MainFormsIds.Purchases : (int)MainFormsIds.Sales, parameters.IsSupplier ? (int)SubFormsIds.Suppliers_Purchases : (int)SubFormsIds.Customers_Sales, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await PersonService.UpdatePersons(parameters);
            return add;
        }


        [HttpPut(nameof(UpdateActivePersons))]
        public async Task<ResponseResult> UpdateActivePersons(UpdateStatusRequest parameters)
        {
            var add = await PersonService.UpdateStatus(parameters);
            return add;
        }


        [HttpDelete("DeletePersons")]
        public async Task<ResponseResult> DeletePersons([FromBody] DeletePersonsRequest prm)
        {
            
            var add = await PersonService.DeletePersons(prm);
            return add;

        }


        [HttpGet("GetPersonHistory")]
        public async Task<ResponseResult> GetPersonHistory(int Id)
        {
            var add = await PersonService.GetPersonHistory(Id);
            return add;
        }


        [HttpGet("GetPersonsDropDown")]
        public async Task<ResponseResult> GetPersonsDropDown([FromQuery] int? personId, [FromQuery] string? SearchCriteria, [FromQuery] int PageSize, [FromQuery] int PageNumber, [FromQuery] bool IsSupplier, [FromQuery] string? code, [FromQuery] int? invoiceTypeId, [FromQuery] bool isReport = false)
        {
            GetPersonsDropDownRequest request = new GetPersonsDropDownRequest()
            {
                SearchCriteria = SearchCriteria,
                PageNumber = PageNumber,
                PageSize = PageSize,
                IsSupplier = IsSupplier,
                personId = personId ,
                Code= code,
                invoiceTypeId = invoiceTypeId,
                isReport = isReport
            };
            var add = await PersonService.GetPersonsDropDown(request);
            return add;
        }


        [HttpGet("GetAllPersonsDropDown")]
        public async Task<ResponseResult> GetAllPersonsDropDown(bool isHaveSalesman,string? Name, int PageSize, int PageNumber, bool IsSupplier)
        {
            GetAllPersonsDropDownRequest request = new GetAllPersonsDropDownRequest()
            {
                SearchCriteria = Name,
                PageNumber = PageNumber,
                PageSize = PageSize,
                IsSupplier = IsSupplier,
                isHaveSalesman = isHaveSalesman,
                
            };
            var add = await PersonService.GetAllPersonsDropDown(request);
            return add;
        }

        [HttpGet("GetPersonsByDate")]
        public async Task<ResponseResult> GetPersonsByDate(DateTime date, int PageNumber, int PageSize = 10)
        {
            GetPersonsByDateRequest parameters = new GetPersonsByDateRequest()
            {
                date = date,
                PageNumber = PageNumber,
                PageSize = PageSize

            };
            var users = await PersonService.GetPersonsByDate(parameters);
            return users;

        }


    }
}
