using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers
{
    public static class  additionsHelper
    {
        public static bool isCanDeleteAdditions(IQueryable<InvPurchaseAdditionalCostsRelation> additionQuery, int additionId)
        { 
            var isUsed = !additionQuery.Where(x => x.AddtionalCostId == additionId).Any();
            return isUsed;
        }
    }
}
