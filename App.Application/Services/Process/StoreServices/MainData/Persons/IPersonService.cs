using App.Application.Handlers.Persons;
using App.Application.Handlers.Persons.GetPersonsByDate;
using App.Domain.Models.Common;
using DocumentFormat.OpenXml.Spreadsheet;

namespace App.Application.Services.Process.Persons
{
    public interface IPersonService
    {
        Task<ResponseResult> AddPerson(personRequest parameter);
        Task<ResponseResult> GetListOfPersons(PersonsSearch parameters, string ids, bool isSearchData = true, bool isPrint = false);
        Task<ResponseResult> UpdatePersons(UpdatePersonsRequest parameters);
        Task<ResponseResult> UpdateStatus(UpdateStatusRequest parameters);
        Task<ResponseResult> DeletePersons(DeletePersonsRequest ListCode);
        Task<ResponseResult> GetPersonHistory(int Code);
        Task<ResponseResult> GetPersonsDropDown(GetPersonsDropDownRequest request);
        Task<ResponseResult> GetAllPersonsDropDown(GetAllPersonsDropDownRequest request);
        Task<WebReport> SupplierCutomerReport(PersonsSearch parameters, bool isArabic, exportType exportType, string ids,int fileId=0, bool isSearchData=true );

        Task<bool> DisableCanDelete(int personId);
        Task<ResponseResult> GetPersonsByDate(GetPersonsByDateRequest parameter);

        //   Task<IEnumerable<ExportPersonModel>> GetListOfPersonsExport(PersonsSearch parameters);
    }
}
