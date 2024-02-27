using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Test._1
{
    public class SendHello_HandlerRequest : IRequest<ResponseResult>
    {
        public string Name { get; set; }
    }
}
