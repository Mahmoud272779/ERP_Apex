using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Test._1
{
    public class SnedHello_HandlerService : IRequestHandler<SendHello_HandlerRequest, ResponseResult>
    {
        public async Task<ResponseResult> Handle(SendHello_HandlerRequest request, CancellationToken cancellationToken)
        {
            return new ResponseResult()
            {
                Note = $"Hello, {request.Name}"
            };
        }
    }
}
