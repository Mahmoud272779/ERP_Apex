using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.ForgetPassword;
using App.Application.Handlers.Helper;
using App.Application.Handlers.Login.LockAccount;
using App.Application.Handlers.Login.ResumeSessionService;
using App.Application.Helpers;
using App.Application.Services.HelperService.CookiesAppend;
using App.Domain;
using App.Domain.Models.Response.General;
using App.Domain.Models.Shared;
using App.Infrastructure.settings;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace App.Api.Controllers.Setup
{
    public class login : ApiControllerBase
    {
        private readonly iLoginService _iLoginService;
        private readonly ICookiesService _cookiesService;
        private readonly IMediator mediator;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IMemoryCache _memoryCache;

        public login(IActionResultResponseHandler responseHandler,
                     iLoginService iLoginService, IMemoryCache memoryCache,
                     ICookiesService cookiesService, IMediator mediator, IHubContext<NotificationHub> hub) : base(responseHandler)
        {
            _iLoginService = iLoginService;
            _cookiesService = cookiesService;
            this.mediator = mediator;
            _hub = hub;
            _memoryCache = memoryCache;
        }

        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] loginReqDTO loginDTO)
        {
            var res = await _iLoginService.login(loginDTO);
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpPost(nameof(ForgetPassword))]
        public async Task<IActionResult> ForgetPassword([FromBody] forgetPasswordRequest parm)
        {
            var res = await mediator.Send(parm);
            return Ok(res);
        }

        [Authorize]
        [HttpPost(nameof(ResumeSession))]
        public async Task<IActionResult> ResumeSession([FromBody] ResumeSessionRequest parm)
        {
            var res = await mediator.Send(parm);
            return Ok(res);
        }
        [Authorize]
        [HttpPost(nameof(lockAccount))]
        public async Task<IActionResult> lockAccount([FromQuery] lockAccountRequest parm)
        {
            var res = await mediator.Send(parm);
            return Ok(res);
        }
        [Authorize]
        [HttpGet(nameof(GetCompanySubscriptionInformation))]
        public async Task<IActionResult> GetCompanySubscriptionInformation()
        {
            var res = await mediator.Send(new Application.Handlers.Settings.companyInformationRequest());
            return Ok(res);
        }
        [Authorize]
        [HttpGet(nameof(currentUserSettings))]
        public async Task<IActionResult> currentUserSettings()
        {
            var res = await mediator.Send(new currentUserSettingsRequest());
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpGet("deployingSystem")]
        public async Task<IActionResult> deployingSystem([FromQuery] Alart request, [FromHeader] string key)
        {
            if (key != defultData.userManagmentApplicationSecurityKey)
                return BadRequest();
            await _hub.Clients.All.SendAsync(defultData.deployingSystem, request);
            return Ok();
        }
        [AllowAnonymous]
        [HttpGet("forceLogout")]
        public async Task<IActionResult> forceLogout([FromHeader] string key)
        {
            if (key != defultData.userManagmentApplicationSecurityKey)
                return BadRequest();
            await _hub.Clients.All.SendAsync(defultData.LogoutNotification);
            return Ok();
        }
        [AllowAnonymous]
        [HttpGet("getTotalOnlineUsers")]
        public async Task<IActionResult> getTotalOnlineUsers([FromHeader] string key)
        {
            if (key != defultData.userManagmentApplicationSecurityKey)
                return BadRequest();

            var users = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey);
            int count = 0;
            if (users != null)
                count = users.Count;
            return Ok(count);
        }
        [AllowAnonymous]
        [HttpGet("getTotalOnlineUsersInfo")]
        public async Task<IActionResult> getTotalOnlineUsersInfo([FromHeader] string key)
        {
            if (key != defultData.userManagmentApplicationSecurityKey)
                return BadRequest();
            List<OnlineUsersResponseDTO> res = new List<OnlineUsersResponseDTO>();
            var users = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey);
            if (users == null)
                return Ok(res);
            res = users.GroupBy(c => c.CompanyLogin)
                .Select(c => new OnlineUsersResponseDTO
                {
                    companyLogin = c.First().CompanyLogin,
                    databaseName = c.First().DBName,
                    onlineCount = c.Count(),
                    users = c.Select(x => new OnlineUsers
                    {
                        EmployeeId = x.EmployeeId,
                        UserID = x.UserID,
                        isTechSupport = x.isTechSupport
                    }).ToList()
                }).ToList();
            return Ok(res);
        }

    }
}
