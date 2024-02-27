using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Additions.AddAdditions
{
    public  class AddAdditionsRequest : PurchasesAdditionalCostsParameter, IRequest<ResponseResult>
    {
    }
}
