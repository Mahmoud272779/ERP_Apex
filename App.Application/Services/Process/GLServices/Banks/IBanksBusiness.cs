
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Bank
{
    public interface iBanksService
    {
        Task<ResponseResult> AddBanks(BankRequestsDTOs.Add parameter);
        Task<ResponseResult> UpdateBanks(BankRequestsDTOs.Update parameter);
        Task<ResponseResult> GetBankById(int Id);
        Task<ResponseResult> DeleteBank(SharedRequestDTOs.Delete parameter);
        Task<ResponseResult> GetAllBanksData(BankRequestsDTOs.Search paramters);
        Task<ResponseResult> GetAllBanksDataDropDown();
        Task<ResponseResult> GetAllBanksByUserDropDown();

        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter);
        Task<ResponseResult> GetAllBankHistory(int BankId);
        Task<ResponseResult> GetAllBanksSetting();
    }
}
