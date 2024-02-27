using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class CheckStoreRequest : IRequest<ResponseResult>
    {
        public int Code { get; set; }

        public CheckStoreRequest(int code)
        {
            Code = code;
        }
    }
}
