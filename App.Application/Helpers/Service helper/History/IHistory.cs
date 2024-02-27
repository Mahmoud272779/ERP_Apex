using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.Service_helper.History
{
   public interface IHistory<T> where T : class
    {
        Task<bool> AddHistory(int EntityId ,string LatinName, string ArabicName, string lastTransactionAction, string addTransactionUser);
        Task<ResponseResult> GetHistory(Expression<Func<T, bool>> predicate);
    }
}
