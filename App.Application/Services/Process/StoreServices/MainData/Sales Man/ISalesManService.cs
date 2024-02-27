using App.Domain.Entities.Process;
using App.Domain.Models.Response.Store;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Models.Security.Authentication.Request.SharedRequestDTOs;

namespace App.Application.Services.Process.Sales_Man
{
    public interface ISalesManService
    {
        Task<ResponseResult> AddSalesMan(SalesManRequest parameter);
        Task<ResponseResult> GetListOfSalesMan(SalesManSearch parameters);
        Task<listOfSalesmanResponse> ListOfSalesMan(SalesManSearch parameters, string ids, bool isSearchData=true, bool isPrint = false);
        Task<ResponseResult> GetSalesManHistory(int Code);
        Task<ResponseResult> UpdateSalesMan(UpdateSalesManRequest parameters);
        Task<ResponseResult> DeleteSalesMan(SharedRequestDTOs.Delete ListCode);
      //  List<SalesManDto> GetBranchesData(List<InvSalesMan> salesManData);
        Task<ResponseResult> GetSalesManDropDown(getDropDownlist param);
        Task<WebReport> SalesManReport(SalesManSearch parameters, string ids, bool isSearchData, exportType exportType, bool isArabic, int fileId = 0);

    }
}
