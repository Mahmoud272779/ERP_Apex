using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Job;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class JobsController : ApiStoreControllerBase
    {
        private readonly IJobsService JobsService;
        private readonly iAuthorizationService _iAuthorizationService;

        public JobsController(IJobsService _JobsService,iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            JobsService = _JobsService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpPost(nameof(AddJobs))]
        public async Task<ResponseResult> AddJobs(JobsParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Jobs_MainUnits, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await JobsService.AddJob(parameter);
            return add;
        }

        
        [HttpGet("GetListOfJobs")]
        public async Task<ResponseResult> GetListOfJobs(int PageNumber, int PageSize, int Status, string? Name)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Jobs_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            JobsSearch parameters = new JobsSearch()
            {

                PageNumber = PageNumber,
                PageSize = PageSize,
                Status = Status,
                Name = Name

            };
            var add = await JobsService.GetListOfJobs(parameters);
            return add;
        }

        
        [HttpPut(nameof(UpdateJobs))]
        public async Task<ResponseResult> UpdateJobs(UpdateJobsParameter parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Jobs_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await JobsService.UpdateJobs(parameters);
            return add;
        }

        
        [HttpPut(nameof(UpdateActiveJobs))]
        public async Task<ResponseResult> UpdateActiveJobs(SharedRequestDTOs.UpdateStatus parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Jobs_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await JobsService.UpdateStatus(parameters);
            return add;
        }


        
        [HttpDelete("DeleteJobs")]
        public async Task<ResponseResult> DeleteJobs([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Jobs_MainUnits, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete ListCode = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };

            var add = await JobsService.DeleteJobs(ListCode);
            return add;

        }

        
        [HttpGet("GetJobHistory")]
        public async Task<ResponseResult> GetJobHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Jobs_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await JobsService.GetJobHistory(Id);
            return add;

        }
        
        [HttpGet(nameof(GetActiveJobsDropDown))]
        public async Task<ResponseResult> GetActiveJobsDropDown()
        {
            var result = await JobsService.GetJobsDropDownList();
            return result;

        }
    }
}
