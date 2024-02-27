using System;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers.Units;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Store;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class StoresController : ApiStoreControllerBase
    {
        private readonly IStoresService StoresService;
        private readonly iAuthorizationService _iAuthorizationService;

        public StoresController(IStoresService _StoresService,iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            StoresService = _StoresService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpPost(nameof(AddStores))]
        public async Task<ResponseResult> AddStores(StoresParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Stores_MainUnits, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await StoresService.AddStore(parameter);
            return add;
        }



        
        [HttpGet("GetListOfStores")]

        public async Task<ResponseResult> GetListOfStores(int PageNumber, int PageSize, string? Name, int Status, string? BranchList)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Stores_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            int[] branchesIds = null;
            if (!string.IsNullOrEmpty(BranchList))
                branchesIds = Array.ConvertAll(BranchList.Split(','), s => int.Parse(s));
            StoresSearch parameters = new StoresSearch()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                Name = Name,
                Status = Status,
                BranchList = branchesIds
            };
            var add = await StoresService.GetListOfStores(parameters);
            return add;
        }

        
        [HttpPut(nameof(UpdateStores))]
        public async Task<ResponseResult> UpdateStores(UpdateStoresParameter parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Stores_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await StoresService.UpdateStores(parameters);
            return add;
        }

        
        [HttpPut(nameof(UpdateActiveStores))]
        public async Task<ResponseResult> UpdateActiveStores(SharedRequestDTOs.UpdateStatus parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Stores_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await StoresService.UpdateStatus(parameters);
            return add;
        }




        
        [HttpDelete("DeleteStores")]
        public async Task<ResponseResult> DeleteStores(int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Stores_MainUnits, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete ListCode = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var add = await StoresService.DeleteStores(ListCode);
            return add;

        }

        
        [HttpGet("GetStoreHistory")]
        public async Task<ResponseResult> GetStoreHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Stores_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await StoresService.GetStoreHistory(Id);
            return add;

        }

        
        [HttpGet("GetActiveStoresDropDown")]
        public async Task<ResponseResult> GetActiveStoresDropDown()
        {
            var add = await StoresService.GetActiveStoresDropDown();
            return add;

        }

        
        [HttpGet("GetAllStoresDropDown")]
        public async Task<ResponseResult> GetAllStoresDropDown(int? invoiceTypeId)
        {
            var add = await StoresService.GetAllStoresDropDown(invoiceTypeId);
            return add;

        }
        [HttpGet("GetAllActiveStoresDropDownForInvoices")]
        public async Task<ResponseResult> GetAllActiveStoresDropDownForInvoices(int? invoiceId, bool isTransfer = false , int? invoiceTypeId = 0)
        {
            if (invoiceTypeId == null)
                invoiceTypeId = 0;
            var add = await StoresService.GetAllActiveStoresDropDownForInvoices(invoiceId, isTransfer, invoiceTypeId.Value);
            return add;

        }

        [HttpGet("GetAllStoreChangesAfterDate")]
        public async Task<ResponseResult> GetAllStoreChangesAfterDate(DateTime date, int PageNumber, int PageSize = 10)
        {
            
            var changes = await StoresService.GetAllStoreChangesAfterDate(date,PageNumber,PageSize); 
            return changes;

        }

    }
}
