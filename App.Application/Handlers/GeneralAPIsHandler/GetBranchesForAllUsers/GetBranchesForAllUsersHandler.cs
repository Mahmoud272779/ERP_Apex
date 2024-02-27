using App.Application.Handlers.InvoicesHelper.getBranches;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetBranchesForAllUsers
{
    public class GetBranchesForAllUsersHandler : IRequestHandler<GetBranchesForAllUsersRequest, ResponseResult>
    {
        private readonly iBranchsService _iBranchsService;
        private readonly IGeneralAPIsService generalAPIsService;

        public GetBranchesForAllUsersHandler(iBranchsService iBranchsService,IGeneralAPIsService generalAPIsService)
        {
            _iBranchsService = iBranchsService;
            this.generalAPIsService = generalAPIsService;
        }

        public async Task<ResponseResult> Handle(GetBranchesForAllUsersRequest request, CancellationToken cancellationToken)
        {
            return await _iBranchsService.GetBranchesForAllUsers(request.PageNumber,request.PageSize);
        }
    }
}
