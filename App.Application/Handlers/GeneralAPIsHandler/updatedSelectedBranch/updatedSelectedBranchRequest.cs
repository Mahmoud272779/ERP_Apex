using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.updatedSelectedBranch
{
    public class updatedSelectedBranchRequest : IRequest<ResponseResult>
    {
        public int branchId { get; set; }
    }
}
