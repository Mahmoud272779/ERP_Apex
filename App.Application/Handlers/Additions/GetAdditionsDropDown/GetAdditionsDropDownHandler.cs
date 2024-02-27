using App.Application.Handlers.Units;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Additions
{
    public class GetAdditionsDropDownHandler : IRequestHandler<GetAdditionsDropDownRequest,ResponseResult>
    {
        private readonly IRepositoryQuery<InvPurchasesAdditionalCosts> additionsQuery;

        public GetAdditionsDropDownHandler(IRepositoryQuery<InvPurchasesAdditionalCosts> additionsQuery)
        {
            this.additionsQuery = additionsQuery;
        }
        public async Task<ResponseResult> Handle(GetAdditionsDropDownRequest request, CancellationToken cancellationToken)
        {
            var dropdownlist = await additionsQuery.Get(e => new { e.PurchasesAdditionalCostsId, e.ArabicName, e.LatinName, e.AdditionalType ,e.Status } );
            return new ResponseResult() { Data = dropdownlist, DataCount = dropdownlist.Count, Result = dropdownlist.Any() ? Result.Success : Result.Failed };
        }
    }
}
