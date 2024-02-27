using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.Setup.ItemCard.Query;
using App.Application.Handlers.Setup.UsersAndPermissions.GetUsersByDate;
using App.Application.Handlers.Units;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Persons;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Domain.Models.Request.General;
using App.Domain.Models.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Setup.ItemCard.ViewModels;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;
using static Dapper.SqlMapper;

namespace App.Api.Controllers.Setup
{
    
    
    public class UsersManagerController : ApiGeneralControllerBase
    {
        private readonly iUserService _iUserService;
        private readonly iAuthorizationService _iAuthorizationService;

        public UsersManagerController(iUserService iUserService ,iAuthorizationService iAuthorizationService, IActionResultResponseHandler ResponseHandler):base(ResponseHandler)
        {
            _iUserService = iUserService;
            _iAuthorizationService = iAuthorizationService;
        }

        [HttpGet("getAllUsers")]
        public async Task<ResponseResult> getAllUsers([FromQuery]getUsersDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Users, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iUserService.getAllUsers(prm);
        }
        [HttpGet("getUserById/{id}")]
        public async Task<ResponseResult> getUserById(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Users, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iUserService.getUserById(id);
        }
        [HttpPost("addUser")]
        public async Task<ResponseResult> addUser([FromBody]addUsersDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Users, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var add = new ResponseResult();
            //for (int i = 0; i < 1000; i++)
            //{
            //    prm.username = prm.username;
            //    prm.username = prm.username + i.ToString();
            //}
                add  = await _iUserService.addUser(prm);
            return add;
        }
        [HttpPut("editUser")]
        public async Task<ResponseResult> editUser([FromBody]editUsersDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Users, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iUserService.editUser(prm);
        }
        [HttpDelete("deleteUser")]
        public async Task<ResponseResult> deleteUser([FromBody] deleteUsersDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Users, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iUserService.deleteUser(prm);
        }
        [HttpPut("ChangeOtherSettings")]
        public async Task<ResponseResult> ChangeOtherSettings([FromBody] OtherSettingsDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Users, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iUserService.ChangeOtherSettings(prm);    
        }
        [HttpGet("GetOtherSettings/{userId}")]
        public async Task<ResponseResult> GetOtherSettings(int userId)
        {
            //var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Users, Opretion.Open);
            //if (isAuthorized != null)
            //    return isAuthorized;
            return await _iUserService.GetOtherSettings(userId);
        }
        [HttpGet("getUserInfoDropDownList")]
        public async Task<ResponseResult> getUserInfoDropDownList([FromQuery]bool isSession = false)
        {
            return await _iUserService.getUserInfoDropDownList(isSession);
        }

        [HttpGet("GetUsersByDate")]
        public async Task<ResponseResult> GetUsersByDate(DateTime date, int PageNumber, int PageSize = 10)
        {
            GetUsersByDateRequest parameters = new GetUsersByDateRequest()
            {
                date = date,
                PageNumber = PageNumber,
                PageSize = PageSize

            };
            var users = await _iUserService.GetUsersByDate(parameters);
            return users;

        }
    }
}
