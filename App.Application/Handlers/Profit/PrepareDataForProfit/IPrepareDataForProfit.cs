using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Profit
{
    public interface IPrepareDataForProfit
    {
        public  Task<ResponseResult> PreparingDataForProfit(int BranchId ,bool calcJournals,int? itemId=null);
    }
}
