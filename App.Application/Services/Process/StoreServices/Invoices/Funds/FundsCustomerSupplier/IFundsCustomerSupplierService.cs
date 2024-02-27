using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.FundsCustomerSupplier
{
    public interface IFundsCustomerSupplierService
    {
        Task<ResponseResult> GetListOfFundsCustomer(FundsCustomerandSupplierSearch parameters);
        Task<ResponseResult> GetListOfFundsSuppliers(FundsCustomerandSupplierSearch parameters);
        Task<ResponseResult> UpdateFundsSuppliersAndCustomers(FundsCustomerandSupplierParameter parameters,bool isCustomer);
        Task<ResponseResult> GetFundsById(int Id);
        Task<WebReport> PersonsReport(FundsCustomerandSupplierSearch parameters, bool isSupplier, string ids, bool isSearchData, exportType exportType, bool isArabic, int fileId = 0);

    }
}
