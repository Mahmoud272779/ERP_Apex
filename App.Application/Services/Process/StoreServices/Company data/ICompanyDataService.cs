using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Company_data
{
    public interface ICompanyDataService
    {
        Task<ResponseResult> UpdateCompanyData(UpdateCompanyDataRequest parameters);
        Task<ResponseResult> GetCompanyData(bool fromSystem = false);
    }
}
