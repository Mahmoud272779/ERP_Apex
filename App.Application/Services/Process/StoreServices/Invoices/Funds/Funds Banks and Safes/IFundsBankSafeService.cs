using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Funds_Banks_and_Safes
{
    public interface IFundsBankSafeService
    {
        Task<ResponseResult> AddFundsBankSafe(FundsBankSafeRequest parameter);
        Task<ResponseResult> UpdateFundsBankSafe(UpdateFundsBankSafeRequest parameter);
        Task<ResponseResult> DeleteFundsBankSafe(Delete ListCode);
        Task<ResponseResult> GetListOfFundsBanksSafes(fundsSearch parameters);
        Task<ResponseResult> GetFundsBankSafeHistory(int documentId);
        Task<ResponseResult> GetFundsBanksSafesById(int Id);

        
    }
}
