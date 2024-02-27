using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.PurchasesAdditionalCosts
{
    public interface IPurchasesAdditionalCostsService
    {
        Task<ResponseResult> AddPurchasesAdditionalCosts(PurchasesAdditionalCostsParameter parameter);
        Task<ResponseResult> GetListOfPurchasesAdditionalCosts(PurchasesAdditionalCostsSearch parameters);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters);
        Task<ResponseResult> UpdatePurchasesAdditionalCosts(UpdatePurchasesAdditionalCostsParameter parameters);
        Task<ResponseResult> DeletePurchasesAdditionalCosts(SharedRequestDTOs.Delete ListCode);
        Task<ResponseResult> GetAllPurchasesAdditionalCostsDropDown();

    }
}
