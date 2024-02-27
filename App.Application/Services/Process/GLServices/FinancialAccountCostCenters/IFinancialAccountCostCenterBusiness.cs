using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.FinancialAccountCostCenters
{
    public interface IFinancialAccountCostCenterBusiness
    {
        Task<IRepositoryActionResult> AddFinancialAccountCostCenter(FinancialCostParameter parameter);
        Task<IRepositoryActionResult> GetFinancialAccountCostCenter(int FinancialAccountId);
        Task<ResponseResult> RemoveFinancialAccountCostCenter(FinancialCostsParameter parameter);
        Task<IRepositoryActionResult> AddFinancialAccountsForCostCenter(FinancialForCostParameter parameter);
        Task<IRepositoryActionResult> GetFinancialAccountsForCostCenter(int costCenterId, int pageNumber, int pageSize);
        Task<IRepositoryActionResult> GetFinancialAccountsWhichNotFoundForCostCenter(int pageNumber , int pageSize ,int costCenterId);
     //   Task<IRepositoryActionResult> GetFinancialAccountsWhichFoundForCostCenter(int costCenterId);
    }
}
