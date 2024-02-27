using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Models.Security.Authentication.Response.Totals;

namespace App.Application.Services.Process.BalanceReview
{
    public interface IBalanceReviewBusiness
    {
        //Task<ResponseResult> getAllDataBalanceReview(BalanceReviewParameter paramters);
        Task<ResponseResult> getAllDataBalanceById(BalanceReviewSearchParameter parametr, int ID);
        Task<ResponseResult> getTopLevelDataBalance(BalanceReviewSearchParameter parametr);
        Task<WebReport> DetailedBalanceReviewReport(BalanceReviewSearchParameter parameter,string ids, exportType exportType, bool isArabic, int fileId = 0);


    }
}
