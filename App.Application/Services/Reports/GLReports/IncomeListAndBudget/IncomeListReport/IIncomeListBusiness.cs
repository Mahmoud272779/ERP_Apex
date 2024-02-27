using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Models.Security.Authentication.Response.Totals;

namespace App.Application
{
    public interface IIncomeListBusiness
    {
        Task<ResponseResult> getTopLevelIncomingList(IncomeListSearchParameter parametr);
        Task<ResponseResult> getAllDataIncomeinListById(IncomeListSearchParameter parametr,int Id);
        
        //Task<totalResponseResult> getTopLevelIncomingListAndBudget(IncomeListSearchParameter parameter);
        //Task<totalResponseResult> getAllDataIncomeinListAndBudgetById(IncomeListSearchParameter parametr, int ID);
    }
}
