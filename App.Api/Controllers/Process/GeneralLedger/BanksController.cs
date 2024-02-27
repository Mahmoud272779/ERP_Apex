using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Bank;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class BanksController : ApiGeneralLedgerControllerBase
    {
        private readonly iBanksService banksBusiness;
        private readonly iAuthorizationService _iAuthorizationService;

        public BanksController(iBanksService BanksBusiness
            ,iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            banksBusiness = BanksBusiness;
            _iAuthorizationService = iAuthorizationService;
        }
        //create new bank with multiple branches
        
        [HttpPost(nameof(CreateNewBanks))]
        public async Task<ResponseResult> CreateNewBanks([FromBody] BankRequestsDTOs.Add parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.banks, (int)SubFormsIds.Banks_MainData, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await banksBusiness.AddBanks(parameter);
            //  var result = ResponseHandler.GetResult(add);
            return add;
        }
        //update  bank with multiple branches
        [HttpPut(nameof(UpdateBanks))]
        public async Task<ResponseResult> UpdateBanks(BankRequestsDTOs.Update parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.banks, (int)SubFormsIds.Banks_MainData, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await banksBusiness.UpdateBanks(parameter);
            // var result = ResponseHandler.GetResult(add);
            return add;
        }
        //update status for banks
        
        [HttpPut(nameof(UpdateBankStatus))]
        public async Task<ResponseResult> UpdateBankStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.banks, (int)SubFormsIds.Banks_MainData, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await banksBusiness.UpdateStatus(parameter);
            //  var result = ResponseHandler.GetResult(add);
            return add;
        }
        //delete banks
        [HttpDelete("DeleteBanks")]
        public async Task<ResponseResult> DeleteBanks([FromBody] int[] Ids)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.banks, (int)SubFormsIds.Banks_MainData, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            {
                Ids = Ids
            };
            var Delete = await banksBusiness.DeleteBank(parameter);
            // var result = ResponseHandler.GetResult(Delete);
            return Delete;
        }
        // get banks by Id
        [HttpGet("GetBanksById/{id}")]
        public async Task<ResponseResult> GetBanksById(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.banks, (int)SubFormsIds.Banks_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //return Ok(accountTree.GetById(id).Result);
            var account = await banksBusiness.GetBankById(id);
            //  var result = ResponseHandler.GetResult(account);
            return account;
        }
        //get all banks 
        [HttpGet("GetAllBanks")]
        public async Task<ResponseResult> GetAllBanks(int PageSize, int PageNumber, string? Name, int Status)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.banks, (int)SubFormsIds.Banks_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            BankRequestsDTOs.Search parameters = new BankRequestsDTOs.Search()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchCriteria = Name
            };
            parameters.Status = Status;
            var account = await banksBusiness.GetAllBanksData(parameters);
            return account;
        }
        // dropdown for banks By User and the Current Branch
        [HttpGet("GetAllBanksDropDown")]
        public async Task<ResponseResult> GetAllBanksDropDown()
        {

            var account = await banksBusiness.GetAllBanksDataDropDown();
            return account;
        }

        // dropdown for banks By User in all Branchs has authority on it
        [HttpGet("GetAllBanksByUserDropDown")]
        public async Task<ResponseResult> GetAllBanksByUserDropDown()
        {

            var account = await banksBusiness.GetAllBanksByUserDropDown();
            return account;
        }

        //get all history for banks
        [HttpGet("GetAllBankHistory")]
        public async Task<ResponseResult> GetAllBankHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.banks, (int)SubFormsIds.Banks_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var account = await banksBusiness.GetAllBankHistory(Id);
            //   var result = ResponseHandler.GetResult(account);
            return account;
        }
        [HttpGet("GetAllBankSetting")]
        public async Task<ResponseResult> GetAllBankSetting()
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.banks, (int)SubFormsIds.Banks_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var account = await banksBusiness.GetAllBanksSetting();
            //var result = ResponseHandler.GetResult(account);
            return account;
        }
    }
}
