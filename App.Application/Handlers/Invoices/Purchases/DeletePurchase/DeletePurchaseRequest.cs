using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.DeletePurchase
{
    public class DeletePurchaseRequest : SharedRequestDTOs.Delete,IRequest<ResponseResult>
    {
    }
}
