using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Size
{
    public interface ISizesService
    {
        Task<ResponseResult> AddSize(SizesParameter parameter);
        Task<ResponseResult> GetListOfSizes(SizesSearch parameters);
        Task<ResponseResult> UpdateSizes(UpdateSizesParameter parameters);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters);
        Task<ResponseResult> DeleteSizes(SharedRequestDTOs.Delete parameter);
        Task<ResponseResult> GetSizeHistory(int SizeId);
    }
}
