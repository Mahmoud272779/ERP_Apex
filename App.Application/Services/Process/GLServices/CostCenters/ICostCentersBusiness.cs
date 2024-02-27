using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.CostCenters
{
    public interface ICostCentersBusiness
    {
        Task<IRepositoryActionResult> AddCostCenter(CostCenterParameter parameter);
        Task<IRepositoryActionResult> UpdateCostCenter(UpdateCostCenterParameter parameter);
        Task<ResponseResult> DeleteCostCenterAsync(SharedRequestDTOs.Delete parameter);
        Task<IRepositoryActionResult> GetCostCenterById(int Id);
        Task<IRepositoryActionResult> GetAllCostCenterData(PageParameter paramters);
        Task<IRepositoryActionResult> GetAllCostCenterDataDropDown(PageParameter paramters);
        Task<ResponseResult> GetAllCostCenterHistory(int costCenterId);
        Task<IRepositoryActionResult> GetAllCostCenterDataDropDown();
        Task<IRepositoryActionResult> GetAllCostCenterDropDown(int type, int? finanncialAccountId);
        Task<IRepositoryActionResult> GetAllCostCenterDataWithOutPage();
      
    }
}
