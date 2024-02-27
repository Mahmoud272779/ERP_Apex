using App.Domain.Entities.Process.Store;
using App.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.getBranches
{
    public class getBranchesHandler : IRequestHandler<getBranchesRequest, ResponseResult>
    {
        private readonly iBranchsService _iBranchsService;

        public getBranchesHandler(iBranchsService iBranchsService)
        {
            _iBranchsService = iBranchsService;
        }

        public async Task<ResponseResult> Handle(getBranchesRequest request, CancellationToken cancellationToken)
        {
            return await _iBranchsService.getBranches();
        }
    }
}
