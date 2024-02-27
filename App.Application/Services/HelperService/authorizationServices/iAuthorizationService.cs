using App.Domain.Enums;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.HelperService.authorizationServices
{
    public interface iAuthorizationService
    {
        public Task<ResponseResult> isAuthorized(int mainFormCode,int subFormCode, Opretion opretion);
    }
}
