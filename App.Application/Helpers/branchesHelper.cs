using FluentValidation.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers
{
    public static class branchesHelper
    {
        public static async Task<ResponseResult> CheckIsBranchExist(int[] branchId,IRepositoryQuery<GLBranch> glBranchQuery)
        {
            if (branchId == null)
                return new ResponseResult()
                {
                    Note = "Branch Id is required",
                    ErrorMessageAr = "يجب ادخل الفرع",
                    ErrorMessageEn = "Branch is Reqired"
                };
            var isBranchExist = glBranchQuery.TableNoTracking.Where(x => branchId.Contains(x.Id)).Any();
            if(!isBranchExist)
                return new ResponseResult()
                {
                    Note = "Branch is not exist",
                    ErrorMessageAr = "هذا الفرع غير موجود",
                    ErrorMessageEn = "Branch is not exist"
                };
            return null;
        }

    }
}
