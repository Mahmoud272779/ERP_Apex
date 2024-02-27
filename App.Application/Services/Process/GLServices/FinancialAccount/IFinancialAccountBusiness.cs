using App.Domain.Models.Common;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.FinancialAccounts
{
    public interface IFinancialAccountBusiness
    {
        Task<IRepositoryActionResult> AddFinancialAccount(FinancialAccountParameter parameter);
        Task<IRepositoryActionResult> UpdateFinancialAccount(UpdateFinancialAccountParameter parameter);
        Task<IRepositoryActionResult> DeleteFinancialAccountAsync(SharedRequestDTOs.Delete parameter);
        Task<IRepositoryActionResult> GetFinancialAccountById(int Id);
        Task<IRepositoryActionResult> GetAllFinancialAccount(FA_Search paramters, int? id,int? pageNumber,int? pageSize);
        Task<IRepositoryActionResult> GetAllFinancialAccountForOpeningBalance();
        Task<IRepositoryActionResult> index();
        Task<IRepositoryActionResult> GetAllFinancialAccountDropdown();
        Task<IRepositoryActionResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter);
        Task<ResponseResult> GetAllFinancialAccountHistory(int id);
        Task<IRepositoryActionResult> GetAllFinancialAccountDropDown();
        Task<string> AddAutomaticCode();
        Task<IRepositoryActionResult> MoveFinancialAccount(MoveFinancial parameter);
        Task<ResponseResult> GetFinancialAccountDropDown(DropDownRequestForGL request);
        Task<ResponseResult> GetAccountInformation(int id);
        Task<IRepositoryActionResult> RecodingFinancialAccount();
    }
}
