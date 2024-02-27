using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.updatedSelectedBranch
{
    public class updatedSelectedBranchHandler : IRequestHandler<updatedSelectedBranchRequest, ResponseResult>
    {
        private readonly iBranchsService _branchsService;

        public updatedSelectedBranchHandler(iBranchsService branchsService)
        {
            _branchsService = branchsService;
        }

        public async Task<ResponseResult> Handle(updatedSelectedBranchRequest request, CancellationToken cancellationToken)
        {
            return await _branchsService.updatedSelectedBranch(request.branchId);
        }
    }
}
