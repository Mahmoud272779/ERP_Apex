using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
     public interface IDeleteExtractPermissionService
    {
        Task<ResponseResult> DeleteExtractPermission(SharedRequestDTOs.Delete parameter);
    }
}
