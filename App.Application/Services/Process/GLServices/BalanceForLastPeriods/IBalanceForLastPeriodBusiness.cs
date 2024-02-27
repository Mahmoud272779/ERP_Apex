using App.Domain.Models.Security.Authentication.Request;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.BalanceForLastPeriods
{
    public interface IBalanceForLastPeriodBusiness
    {
        Task<IRepositoryActionResult> AddBalanceForLastPeriod(BalanceForLastPeriodParameter parameter);
        Task<IRepositoryActionResult> GetAllBalanceForLastPeriodDropDown();
    }
}
