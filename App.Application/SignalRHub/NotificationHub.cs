using App.Application.Handlers.NotificationSystem;
using App.Application.Handlers.POS.ActivePOSSession;
using App.Application.Handlers.POS.BindPOSSession;
using App.Application.Handlers.SignalRHandler.ChangeActivityHandler;
using App.Infrastructure;
using App.Infrastructure.settings;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace App.Application.SignalRHub
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediator _mediator;

        public NotificationHub(IHttpContextAccessor httpContext, IMediator mediator, IMemoryCache memoryCache)
        {
            _httpContext = httpContext;
            _mediator = mediator;
            _memoryCache = memoryCache;
        }

        public override async Task OnConnectedAsync()
        {
            var token = _httpContext.HttpContext.Request.Query["access_token"].ToString();
            var cont = Context;
            if (token != null)
            {
                MemoryCashHelper _cashHelper = new MemoryCashHelper(_memoryCache);
                var connectionId = Context.ConnectionId;
                var CompanyLogin = StringEncryption.DecryptString(contextHelper.CompanyLogin(token));
                var DBName = StringEncryption.DecryptString(contextHelper.GetDBName(token));
                var UserID = int.Parse(StringEncryption.DecryptString(contextHelper.GetUserID(token)));
                var EmployeeId = int.Parse(StringEncryption.DecryptString(contextHelper.GetEmployeeId(token)));
                var AllUsers = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey);
                var isTechSupport = int.Parse(contextHelper.checkIsTechnicalSupport(token));
                _cashHelper.AddSignalRCash(new SignalRCash()
                {
                    connectionId = connectionId,
                    CompanyLogin = CompanyLogin.ToLower(),
                    DBName = DBName,
                    UserID = UserID,
                    EmployeeId = EmployeeId,
                    isTechSupport = isTechSupport
                }, defultData.SignalRKey);
                if (isTechSupport == 1)
                {
                    return;
                }
                _mediator.Send(new ChangeActivityHandlerRequest()
                {
                    connectionId = Context.ConnectionId,
                    isActive = true
                });

                //await _mediator.Send(new ActivePOSSessionRequest()
                //{
                //    employeeId = EmployeeId,
                //    databaseName = DBName
                //});
            }


            #region reports Notification
            //try
            //{
            //    await _mediator.Send(new ReportNotificationsRequest { connectionId = Context.ConnectionId,token = token });
            //}
            //catch (Exception)
            //{

            //}
            #endregion
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _mediator.Send(new ChangeActivityHandlerRequest()
            {
                connectionId = Context.ConnectionId,
                isActive = false
            });

            MemoryCashHelper _cashHelper = new MemoryCashHelper(_memoryCache);

            _cashHelper.DeleteSignalRCahedRecored(Context.ConnectionId,defultData.SignalRKey,null);
        }

        public async Task SendMessage(string name, string message)
        {
            await Clients.All.SendAsync("SendMessage", name, message);

        }
    }
}
