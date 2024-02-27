using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.GeneralSettings
{
   public interface GL_IGeneralSettingsBusiness
    {
        Task<IRepositoryActionResult> UpdateGeneralSettings(UpdateGeneralSettingsParameter parameter);
        Task<IRepositoryActionResult> GetGLGeneralSettings();
        Task<ResponseResult> UpdatePurchaseGeneralSettings(GLsettingInvoicesParameter parameter, int MainType);
        //Task<ResponseResult> setGLsettingforAllAuthorites(AllAuthoritiesParameter parameter);
        Task<ResponseResult> GetPurchaseAndSalesData(int MainType);

        //settings Customer,Suppliers,safes,banks,salesman,OtherAuthorities,employees
        Task<ResponseResult> MainDataIntegration(updateFinancialAccountRelationSettings parameter,SubFormsIds subFormsIds);


        Task<ResponseResult> getFinancialAccountRelationSettings(getFinancialAccountRelationRequest parameter);


        //Task<IRepositoryActionResult> AddGeneralSettings(GeneralSettingsParameter parameter);
        //Task<IRepositoryActionResult> GetGeneralSettings(PageParameter paramters);


    }
}
