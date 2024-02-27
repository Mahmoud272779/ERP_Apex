using App.Domain.Models.Security.Authentication.Request;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.FinancialAccountForOpeningBalances
{
    public interface IFinancialAccountForOpeningBalanceBusiness
    {
        Task<IRepositoryActionResult> AddFinancialAccountForOpeningBalance(FinancialAccountForOpeningBalanceParameter parameter);
        Task<IRepositoryActionResult> UpdateFinancialAccountForOpeningBalance(UpdateFinancialAccountForOpeningBalanceParameter parameter);
    }
}
