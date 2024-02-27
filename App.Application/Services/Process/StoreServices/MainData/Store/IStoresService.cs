using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Store
{
    public interface IStoresService
    {
        Task<ResponseResult> AddStore(StoresParameter parameter);
        Task<ResponseResult> GetListOfStores(StoresSearch parameters);
        Task<ResponseResult> UpdateStores(UpdateStoresParameter parameters);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters);
        Task<ResponseResult> DeleteStores(SharedRequestDTOs.Delete ListCode);
        Task<ResponseResult> GetStoreHistory(int StoreId);
        Task<ResponseResult> GetActiveStoresDropDown();
        Task<ResponseResult> GetAllStoresDropDown(int? invoiceTypeId);
        Task<ResponseResult> GetAllActiveStoresDropDownForInvoices(int? invoiceId,bool isTransfer = false , int invoiceTypeId=0);
        Task<ResponseResult> GetAllStoreChangesAfterDate(DateTime date, int PageNumber, int PageSize);

    }
}
