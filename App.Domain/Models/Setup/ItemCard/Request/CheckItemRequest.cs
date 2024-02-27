using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class CheckItemRequest : IRequest<ResponseResult>
    {
        public string Code { get; set; }

        public CheckItemRequest(string code)
        {
            Code = code;
        }
    }
}
