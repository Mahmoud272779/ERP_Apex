using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Models.Shared;
using MediatR;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class GetLatestItemCodeRequest:IRequest<ResponseResult>
    {
    }
}
