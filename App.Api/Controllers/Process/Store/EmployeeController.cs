using System;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers.Units;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.Employee;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class EmployeeController : ApiStoreControllerBase
    {
        private readonly IEmployeeServices EmployeesService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _printFileService;

        public EmployeeController(IEmployeeServices _EmployeesService, iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler, IprintFileService printFileService) : base(ResponseHandler)
        {
            EmployeesService = _EmployeesService;
            _iAuthorizationService = iAuthorizationService;
            _printFileService = printFileService;
        }


        [HttpPost(nameof(AddEmployee))]
        public async Task<ResponseResult> AddEmployee([FromForm] EmployeesRequestDTOs.Add parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData,(int)SubFormsIds.Employees_MainUnits,Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            return await EmployeesService.AddEmployee(parameter);
        }

        
        [HttpGet("GetListOfEmployees")]
        public async Task<ResponseResult> GetListOfEmployees(int PageNumber, int PageSize, string? Name,
            int Status, string? JobList, string? BranchList, int EmployeeId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Employees_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            int[] branchesIds = null, jobsIds = null;
            if (!string.IsNullOrEmpty(BranchList))
                branchesIds = Array.ConvertAll(BranchList.Split(','), s => int.Parse(s));
            if (!string.IsNullOrEmpty(JobList))
                jobsIds = Array.ConvertAll(JobList.Split(','), s => int.Parse(s));
            
            EmployeesRequestDTOs.Search paramters = new EmployeesRequestDTOs.Search()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                Name = Name,
                Status = Status,
                JobList = jobsIds,
                BranchList = branchesIds,
                EmployeeId = EmployeeId
            };
            var result = await EmployeesService.GetListOfEmployees(paramters);

            return result;
        }

        [HttpGet("RaoufTest")]
        public async Task<ResponseResult> RaoufTest(int PageNumber, int PageSize, string? Name,
            int Status, string? JobList, string? BranchList, int EmployeeId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Employees_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            int[] branchesIds = null, jobsIds = null;
            if (!string.IsNullOrEmpty(BranchList))
                branchesIds = Array.ConvertAll(BranchList.Split(','), s => int.Parse(s));
            if (!string.IsNullOrEmpty(JobList))
                jobsIds = Array.ConvertAll(JobList.Split(','), s => int.Parse(s));

            EmployeesRequestDTOs.Search paramters = new EmployeesRequestDTOs.Search()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                Name = Name,
                Status = Status,
                JobList = jobsIds,
                BranchList = branchesIds,
                EmployeeId = EmployeeId
            };
            var result = await EmployeesService.RaoufTest(paramters);

            return result;
        }


        [HttpGet("EmployeesReport")]
        public async Task<IActionResult> EmployeesReport(int PageNumber, int PageSize, string? Name,
            int Status, string? JobList, string? BranchList, int EmployeeId,string? ids,bool isSearchData,exportType exportType,bool isArabic)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Employees_MainUnits, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            int[] branchesIds = null, jobsIds = null;
            if (!string.IsNullOrEmpty(BranchList) && BranchList !="0")
                branchesIds = Array.ConvertAll(BranchList.Split(','), s => int.Parse(s));
            if (!string.IsNullOrEmpty(JobList) && JobList !="0")
                jobsIds = Array.ConvertAll(JobList.Split(','), s => int.Parse(s));

            EmployeesRequestDTOs.Search paramters = new EmployeesRequestDTOs.Search()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                Name = Name,
                Status = Status,
                JobList = jobsIds,
                BranchList = branchesIds,
                EmployeeId = EmployeeId,
                Ids=ids,
                IsSearcheData=isSearchData
            };
            WebReport report = new WebReport();
            report = await EmployeesService.EmployeesReport(paramters,exportType, isArabic);

            return Ok(_printFileService.PrintFile(report, "Employees", exportType));

        }

        [HttpPut(nameof(UpdateActiveEmployee))]
        public async Task<ResponseResult> UpdateActiveEmployee(SharedRequestDTOs.UpdateStatus parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Employees_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await EmployeesService.UpdateStatus(parameters);
            return result;
        }

        
        [HttpPut(nameof(UpdateEmployee))]
        public async Task<ResponseResult> UpdateEmployee([FromForm] EmployeesRequestDTOs.Update parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Employees_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await EmployeesService.UpdateEmployees(parameters);
            return result;
        }

        
        [HttpDelete("DeleteEmployees")]
        public async Task<ResponseResult> DeleteEmployees([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Employees_MainUnits, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete ListCode = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var result = await EmployeesService.DeleteEmployees(ListCode);
            return result;

        }

        
        [HttpGet("GetEmployeeHistory")]
        public async Task<ResponseResult> GetEmployeeHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Employees_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await EmployeesService.GetEmployeeHistory(Id);
            return result;
        }


        
        [HttpGet("GetActiveEmployeesDropDown")]
        public async Task<ResponseResult> GetActiveEmployeesDropDown([FromQuery]bool isReport = false, [FromQuery]bool isInUserPage = false, [FromQuery] int? employeeId = 0, [FromQuery]string? SearchCriteria = "",[FromQuery]int pageSize = 5, [FromQuery]int pageNumber = 1)
        {
            var result = await EmployeesService.GetEmployeeDropDown(isInUserPage, employeeId,SearchCriteria,pageSize,pageNumber,isReport);
            return result;
        }

        [HttpGet("GetEmployeesByDate")]
        public async Task<ResponseResult> GetEmployeesByDate(DateTime date, int PageNumber, int PageSize = 10)
        {
            
            var employees = await EmployeesService.GetEmployeesByDate(date, PageNumber, PageSize);
            return employees;

        }
    }
}
