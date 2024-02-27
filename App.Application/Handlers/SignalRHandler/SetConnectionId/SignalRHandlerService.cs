using App.Domain.Entities.Process.General;
using App.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.SignalRHandler
{
    public class SignalRHandlerService : IRequestHandler<SignalRHandlerRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<signalR> _signalRQuery;
        private readonly IRepositoryCommand<signalR> _signalRCommand;
        private readonly IHttpContextAccessor _httpContext;

        public SignalRHandlerService(IRepositoryQuery<signalR> signalRQuery, IRepositoryCommand<signalR> signalRCommand, IHttpContextAccessor httpContext)
        {
            _signalRQuery = signalRQuery;
            _signalRCommand = signalRCommand;
            _httpContext = httpContext;
        }
        public async Task<ResponseResult> Handle(SignalRHandlerRequest request, CancellationToken cancellationToken)
        {
            var token = await _httpContext.HttpContext.GetTokenAsync("access_token");
            var employeeId = int.Parse(StringEncryption.DecryptString(contextHelper.GetEmployeeId(token)));
            var isEmpExist = _signalRQuery.TableNoTracking.Where(x => x.InvEmployeesId == employeeId);
            if (isEmpExist.Any())
            {
                await _signalRCommand.UpdateAsyn(new signalR
                {
                    Id = isEmpExist.FirstOrDefault().Id,
                    connectionId = request.connectionId,
                    InvEmployeesId = employeeId,
                });
                return new ResponseResult()
                {
                    Result = Result.Success
                };
            }
            else
            {
                _signalRCommand.Add(new signalR
                {
                    connectionId = request.connectionId,
                    InvEmployeesId = employeeId,
                });
                await _signalRCommand.SaveAsync();
                return new ResponseResult()
                {
                    Result = Result.Success
                };
            }
            return new ResponseResult()
            {
                Result = Result.Failed
            };
        }
    }
}
