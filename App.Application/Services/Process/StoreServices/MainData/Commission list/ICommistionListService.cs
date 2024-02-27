using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Commission_list
{
    public interface ICommistionListService 
    {
        Task<ResponseResult> AddCommissionList(CommissionListRequest parameter);
        Task<ResponseResult> GetCommissionList(CommissionListSearch parameters);
        Task<ResponseResult> UpdateCommissionList(UpdateCommissionListRequest parameters);
     //   Task<ResponseResult> UpdateActiveCommissionList(UpdateActiveCommissionList parameters);
        Task<ResponseResult> DeleteCommissionList(SharedRequestDTOs.Delete ListCode);
        Task<ResponseResult> GetCommissionListHistory(int CommissionId);
        Task<ResponseResult> GetCommissionListDropDown();
    }
}
