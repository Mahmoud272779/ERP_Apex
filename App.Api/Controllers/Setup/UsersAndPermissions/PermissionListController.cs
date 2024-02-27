using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.Setup.ItemCard.Query;
using App.Application.Handlers.Units;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Persons;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
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
using DocumentFormat.OpenXml.Wordprocessing;
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
    
    
    public class PermissionListController : ApiGeneralControllerBase
    {
        private readonly iPermissionService _iPermissionService;
        private readonly iAuthorizationService _iAuthorizationService;

        public PermissionListController(iPermissionService iPermissionService,iAuthorizationService iAuthorizationService, IActionResultResponseHandler ResponseHandler):base(ResponseHandler)
        {
            _iPermissionService = iPermissionService;
            _iAuthorizationService = iAuthorizationService;
        }

        [HttpPost("addPermissionList")]
        public async Task<ResponseResult> addPermissionList([FromBody]addPermissionRequestDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Permission_Users, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.addPermissionList(prm);
        }

        [HttpPut("EditPermissionList")]
        public async Task<ResponseResult> EditPermissionList([FromBody] editPermissionRequestDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Permission_Users, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.EditPermissionList(prm);
        }
        [HttpGet("GetAllPermissionLists")]
        public async Task<ResponseResult> GetAllPermissionLists([FromQuery] getPermissionRequestDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Permission_Users, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.GetAllPermissionLists(prm);
        }
        [HttpDelete("DeletePermissionLists")]
        public async Task<ResponseResult> DeletePermissionLists([FromBody] deletePermissionRequestDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Permission_Users ,Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.DeletePermissionLists(prm);
        }
        [HttpPost("AddUsersToPermissionList")]
        public async Task<ResponseResult> AddUsersToPermissionList([FromBody] addUsersToPermissionListRequestDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Permission_Users, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.EditUsersToPermissionList(prm);
        }
        [HttpPost("UpdateSubForms")]
        public async Task<ResponseResult> UpdateSubForms()
        {
            //var isAuthorized = await _iAuthorizationService.isAuthorized(9, 45, Opretion.Edit);
            //if (isAuthorized != null)
            //    return isAuthorized;
            return await _iPermissionService.UpdateSubForms();
        }
        [HttpPut("EditRules")]
        public async Task<ResponseResult> EditRules([FromBody] editRulesRequestDto prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Permission_Users, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.EditRules(prm);
        }
        [HttpGet("GetPermissionListUsers")]
        public async Task<ResponseResult> GetPermissionListUsers([FromQuery] getPermissionListUsers prm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Permission_Users, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.GetPermissionListUsers(prm);
        }
        [HttpGet("GetUsersHaveNotPermissions")]
        public async Task<ResponseResult> GetUsersHaveNotPermissions()
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Permission_Users, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.GetUsersHaveNotPermissions();
        }
        [HttpDelete("DeletePermissionListUsers")]
        public async Task<ResponseResult> DeletePermissionListUsers([FromBody] deletePermissionRequestDto Ids)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Permission_Users, Opretion.Delete);
            if(isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.DeletePermissionListUsers(Ids.Ids);
        }
        [HttpGet("GetMainForms/{permissionListId}")]
        public async Task<ResponseResult> GetMainForms(int permissionListId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Permission_Users, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.GetMainForms(permissionListId);
        }
        [HttpGet("GetSubForms")]
        public async Task<ResponseResult> GetSubForms([FromQuery]int permissionListId, [FromQuery] int MainFormCode)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.Permission_Users, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await _iPermissionService.GetSubForms(permissionListId, MainFormCode);
        }

        [HttpGet("GetPermissionListByDate")]
        public async Task<ResponseResult> GetPermissionListByDate(DateTime date, int PageNumber, int PageSize = 10)
        {
            
            var users = await _iPermissionService.GetPermissionListByDate(date,PageNumber,PageSize);
            return users;

        }
        //[HttpGet("GetUsersToPermissionListByDate")]
        //public async Task<ResponseResult> GetUsersToPermissionListByDate(DateTime date)
        //{
        //    var users = await _iPermissionService.GetUsersToPermissionListByDate(date);
        //    return users;

        //}



    }
}
