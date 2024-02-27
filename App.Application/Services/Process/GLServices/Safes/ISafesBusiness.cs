using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Safes
{
    public interface ISafesBusiness
    {
        Task<ResponseResult> AddTreasury(TreasuryParameter parameter);
        Task<ResponseResult> UpdateTreasury(UpdateTreasuryParameter parameter);
        Task<ResponseResult> GetTreasuryById(int Id);
        Task<ResponseResult> DeleteTreasuryAsync(SharedRequestDTOs.Delete parameter);
        Task<ResponseResult> GetAllTreasuryData(PageTreasuryParameter paramters);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter);
        Task<ResponseResult> GetAllTreasuryDataDropDown();
        Task<ResponseResult> GetAllTreasuryByUserDropDown();
        Task<ResponseResult> GetAllTreasuryWithoutFilterDropDown();
        Task<ResponseResult> GetAllTreasuryHistory(int TreasuryId);
        Task<ResponseResult> GetAllTreasuryDataSetting();
    }
}
