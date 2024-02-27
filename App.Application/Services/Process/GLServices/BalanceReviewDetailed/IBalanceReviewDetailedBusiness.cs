using App.Domain.Models.Security.Authentication.Response;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.BalanceReviewDetailed
{
    public interface IBalanceReviewDetailedBusiness
    {
        Task<IRepositoryActionResult> CallRootDetails(BalanceReviewDetailedParameter paramters);
    }
}
