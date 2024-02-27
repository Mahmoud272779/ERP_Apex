using App.Domain.Models.Security.Authentication.Request;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.FinancialSettings
{
    public interface IFinancialSettingBusiness
    {
        Task<IRepositoryActionResult> AddFinancialSetting(FinancialSettingParameter parameter);
        Task<IRepositoryActionResult> GetFinancialSettingForBank();
        Task<IRepositoryActionResult> GetFinancialSettingForTreasury();
    }
}
