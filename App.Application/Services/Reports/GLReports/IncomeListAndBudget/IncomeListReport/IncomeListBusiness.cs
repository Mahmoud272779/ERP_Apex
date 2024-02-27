
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;


namespace App.Application
{
    public class IncomeListBusiness  :IIncomeListBusiness
    {


        private readonly IIncomeListAndBudget incomelistAndBudget;

        public IncomeListBusiness( IIncomeListAndBudget incomeListAndBudget)

         {
 
            this.incomelistAndBudget = incomeListAndBudget;
        }

        

        public async Task<ResponseResult> getAllDataIncomeinListById(IncomeListSearchParameter parametr, int ID)
        {

            if (string.IsNullOrEmpty(ID.ToString()))
                return new ResponseResult() { Data = null, Note = "Id Is Required", Result = Result.RequiredData };
            var result =await incomelistAndBudget.getAllDataIncomeinListAndBudgetById(parametr, ID);
            return new ResponseResult() { Data = result.IncomingData };
        }

       


  

        public async Task<ResponseResult> getTopLevelIncomingList(IncomeListSearchParameter parameter)
        {
            var result = await incomelistAndBudget.getTopLevelIncomingListAndBudget(parameter);
            return new ResponseResult() { Data =result.IncomingData , Total = result.totalBalance, Result = Result.Success };

        }

       
    }
}
