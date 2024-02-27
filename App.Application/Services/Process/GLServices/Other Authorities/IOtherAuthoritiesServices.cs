using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process
{
    public interface IOtherAuthoritiesServices
    {
        Task<ResponseResult> AddOtherAuthorities(OtherAuthoritiesParameter parameter);
        Task<ResponseResult> UpdateOtherAuthorities(UpdateOtherAuthoritiesParameter parameter);
        Task<ResponseResult> GetOtherAuthoritiesById(int Id);
        Task<ResponseResult> DeleteOtherAuthoritiesAsync(SharedRequestDTOs.Delete parameter);
        Task<ResponseResult> GetAllOtherAuthoritiesData(PageOtherAuthoritiesParameter paramters);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter);
        Task<ResponseResult> GetAllOtherAuthoritiesDataDropDown(DropDownParameter parameter);
        Task<ResponseResult> GetOtherAuthoritiesHistory(int Id);

    }
}
