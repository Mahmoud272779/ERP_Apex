using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Job
{
    public interface IJobsService
    {
        Task<ResponseResult> AddJob(JobsParameter parameter);
        Task<ResponseResult> GetListOfJobs(JobsSearch parameters);
        Task<ResponseResult> UpdateJobs(UpdateJobsParameter parameters);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters);
        Task<ResponseResult> DeleteJobs(SharedRequestDTOs.Delete ListCode);
        Task<ResponseResult> GetJobHistory(int JobId);
        Task<ResponseResult> GetJobsDropDownList();
    }
}
