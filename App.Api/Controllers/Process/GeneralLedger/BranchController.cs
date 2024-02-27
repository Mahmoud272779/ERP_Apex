using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.Units;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Branches;
using App.Application.Services.Process.Employee;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class BranchController : ApiGeneralLedgerControllerBase
    {
        private readonly IBranchesBusiness branchBusiness;
        private readonly iAuthorizationService _authorizationService;

        public BranchController(IBranchesBusiness BranchBusiness
            ,iAuthorizationService authorizationService
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            branchBusiness = BranchBusiness;
            _authorizationService = authorizationService;
        }
        [HttpPost(nameof(CreateNewBranch))]
        public async Task<ResponseResult> CreateNewBranch([FromBody] BranchRequestsDTOs.Add parameter)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Branches_MainData, Opretion.Add);
            if (isAutorized != null)
                return isAutorized;
            var result = await branchBusiness.AddBranch(parameter);
            return result;
        }
        [HttpPut(nameof(UpdateBranch))]
        public async Task<ResponseResult> UpdateBranch(BranchRequestsDTOs.Update parameter)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Branches_MainData, Opretion.Edit);
            if (isAutorized != null)
                return isAutorized;
            var result = await branchBusiness.UpdateBranch(parameter);
            return result;
        }
        [HttpPut(nameof(UpdateBranchStatus))]
        public async Task<ResponseResult> UpdateBranchStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Branches_MainData, Opretion.Edit);
            if (isAutorized != null)
                return isAutorized;
            var result = await branchBusiness.UpdateStatus(parameter);

            return result;
        }
        [HttpDelete("DeleteBranch")]
        public async Task<ResponseResult> DeleteBranch([FromBody] int[] Ids)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Branches_MainData, Opretion.Delete);
            if (isAutorized != null)
                return isAutorized;
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            {
                Ids = Ids
            };
            var Delete = await branchBusiness.DeleteBranch(parameter);

            return Delete;
        }
        [HttpGet("GetBranchById/{id}")]
        public async Task<ResponseResult> GetBranchById(int id,int showInvoiceOrJournalEntry=0)
        {
            if (showInvoiceOrJournalEntry==0)
            {
                var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Branches_MainData, Opretion.Open);
                if (isAutorized != null)
                    return isAutorized;
            }
            
            var result = await branchBusiness.GetBranchById(id);

            return result;
        }
        [HttpGet("GetAllBranches")]
        public async Task<ResponseResult> GetAllBranches(int PageSize, int PageNumber, string? Name, int? Status)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Branches_MainData, Opretion.Open);
            if (isAutorized != null)
                return isAutorized;
            BranchRequestsDTOs.Search parameters = new BranchRequestsDTOs.Search()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                Name = Name
            };
            parameters.Status = Status;
            var result = await branchBusiness.GetAllBranchData(parameters);
            return result;
        }
        [HttpGet("GetAllBranchesDropDown")]
        public async Task<ResponseResult> GetAllBranchesDropDown()
        {
            var result = await branchBusiness.GetAllBranchDataDropDown();

            return result;
        }
        [HttpGet("GetAllBranchDataDropDownForPersons")]
        [Authorize]
        public async Task<ResponseResult> GetAllBranchDataDropDownForPersons(bool isSupplier)
        {
            var result = await branchBusiness.GetAllBranchDataDropDownForPersons(isSupplier);

            return result;
        }
        [HttpGet("GetAllBranchHistory")]
        public async Task<ResponseResult> GetAllBranchHistory(int Id)
        {
            var isAutorized = await _authorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Branches_MainData, Opretion.Open);
            if (isAutorized != null)
                return isAutorized;
            var result = await branchBusiness.GetAllBranchHistory(Id);
            return result;
        }

        [HttpGet("GetBranchesByDate")]
        public async Task<ResponseResult> GetBranchesByDate(DateTime date, int PageNumber, int PageSize = 10)
        {
            
            var branches = await branchBusiness.GetBranchesByDate(date,PageNumber,PageSize);
            return branches;

        }
    }
}
