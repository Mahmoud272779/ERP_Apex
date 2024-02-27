using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Color
{
   public interface  IColorsService
    {
        Task<ResponseResult> AddColor(ColorsParameter parameter);
        Task<ResponseResult> GetListOfColors(ColorsSearch parameters);
        Task<ResponseResult> UpdateColors(UpdateColorParameter parameters);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters);
        Task<ResponseResult> DeleteColors(SharedRequestDTOs.Delete ListCode);
        Task<ResponseResult> GetColorHistory(int ColorId);
    }
}
