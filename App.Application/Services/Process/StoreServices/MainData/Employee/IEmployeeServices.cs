using App.Application.Handlers.Setup.UsersAndPermissions.GetUsersByDate;
using App.Application.Handlers.Units;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Employee
{
    public interface IEmployeeServices
    {
        Task<ResponseResult> AddEmployee(EmployeesRequestDTOs.Add parameter);
        Task<ResponseResult> GetListOfEmployees(EmployeesRequestDTOs.Search parameters,bool isPrint=false);
         Task<ResponseResult> RaoufTest(EmployeesRequestDTOs.Search parameters, bool isPrint = false);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters);
        Task<ResponseResult> UpdateEmployees(EmployeesRequestDTOs.Update parameters);
        Task<ResponseResult> DeleteEmployees(SharedRequestDTOs.Delete ListCode);
        Task<ResponseResult> GetEmployeeHistory(int EmployeeId);
        Task<ResponseResult> GetEmployeeDropDown(bool isInUserPage,int? employeeId,string SearchCriteria,int pageSize,int pageNumber,bool isReport = false);
        Task<WebReport> EmployeesReport(EmployeesRequestDTOs.Search parameters, exportType exportType, bool isArabic, int fileId = 0);
        Task<ResponseResult> GetEmployeesByDate(DateTime date, int PageNumber, int PageSize);

        //Task<ResponseResult>GetActiveEmployeesDropDown()
    }
}
