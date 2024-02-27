using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Application.Helpers;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Job
{
    public class JobsService : BaseClass, IJobsService
    {
        private readonly IRepositoryQuery<InvJobs> JobsRepositoryQuery;
        private readonly IRepositoryCommand<InvJobs> JobsRepositoryCommand;
         
        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<InvJobsHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public JobsService(IRepositoryQuery<InvJobs> _JobsRepositoryQuery,
                                   IRepositoryCommand<InvJobs> _JobsRepositoryCommand, 
                                   IHistory<InvJobsHistory> history,
                                   ISystemHistoryLogsService systemHistoryLogsService,
                                   IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            JobsRepositoryQuery = _JobsRepositoryQuery;
            JobsRepositoryCommand = _JobsRepositoryCommand;
          
            httpContext = _httpContext;
            this.history = history;
            _systemHistoryLogsService = systemHistoryLogsService;
        }

        public async Task<ResponseResult> AddJob(JobsParameter parameter)
        {
            parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
            parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
            if (string.IsNullOrEmpty(parameter.LatinName))
                parameter.LatinName = parameter.ArabicName;

            if (string.IsNullOrEmpty(parameter.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            var arabicJobExist = await JobsRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName);
            if (arabicJobExist != null)
                return new ResponseResult() { Data = null, Id = arabicJobExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };

            var LatinJobExist = await JobsRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName);
            if (LatinJobExist != null)
                return new ResponseResult() { Data = null, Id = LatinJobExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };


            int NextCode = JobsRepositoryQuery.GetMaxCode(e => e.Code) + 1;

            var table = Mapping.Mapper.Map<JobsParameter, InvJobs>(parameter);
            table.Code = NextCode;
            JobsRepositoryCommand.Add(table);
            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addJobs);
            return new ResponseResult() { Data = null, Id = table.Id, Result = Result.Success };

        }



        public async Task<ResponseResult> GetListOfJobs(JobsSearch parameters)
        {
            var resData = await JobsRepositoryQuery.GetAllIncludingAsync(0, 0,
                a => (a.Code.ToString().Contains(parameters.Name) || string.IsNullOrEmpty(parameters.Name)
                || a.ArabicName.Contains(parameters.Name) || a.LatinName.Contains(parameters.Name))
                && (parameters.Status == 0 || a.Status == parameters.Status),
                e => (string.IsNullOrEmpty(parameters.Name) ? e.OrderByDescending(q => q.Code) : e.OrderBy(a => (a.Code.ToString().Contains(parameters.Name)) ? 0 : 1)), a => a.Employees);

            resData.Where(a => a.Employees.Count == 0 && a.Id != 1).Select(a => { a.CanDelete = true; return a; }).ToList();

            resData.Select(a => { a.Employees = null; return a; }).ToList();
            
            var count = resData.Count();
            
            if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            {
                resData = resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            return new ResponseResult() { Data = resData, DataCount = count, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }

        public async Task<ResponseResult> UpdateJobs(UpdateJobsParameter parameters)
        {
            if (parameters.Id== 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            parameters.LatinName = Helpers.Helpers.IsNullString(parameters.LatinName);
            parameters.ArabicName = Helpers.Helpers.IsNullString(parameters.ArabicName);
            if (string.IsNullOrEmpty(parameters.LatinName))
                parameters.LatinName = parameters.ArabicName;

            if (string.IsNullOrEmpty(parameters.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }

            var arabicJobExist = await JobsRepositoryQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName && a.Id != parameters.Id);
            if (arabicJobExist != null)
                return new ResponseResult() { Data = null, Id = arabicJobExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };

            var LatinJobExist = await JobsRepositoryQuery.GetByAsync(a => a.LatinName == parameters.LatinName && a.Id != parameters.Id);
            if (LatinJobExist != null)
                return new ResponseResult() { Data = null, Id = LatinJobExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };


            var data = await JobsRepositoryQuery.GetByAsync(a => a.Id == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            var table = Mapping.Mapper.Map<UpdateJobsParameter, InvJobs>(parameters, data);
            if (table.Id == 1)
                table.Status = (int)Status.Active;

            await JobsRepositoryCommand.UpdateAsyn(table);

            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update , Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editJobs);
            return new ResponseResult() { Data = null, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };

        }

        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var Jobs = JobsRepositoryQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.Id));
            var JobsList = Jobs.ToList();

            JobsList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            if (parameters.Id.Contains(1))
                JobsList.Where(q => q.Id == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();
            var rssult = await JobsRepositoryCommand.UpdateAsyn(JobsList);
            foreach (var Job in JobsList)
            {
                history.AddHistory(Job.Id, Job.LatinName, Job.ArabicName, Aliases.HistoryActions.Update , Aliases.TemporaryRequiredData.UserName);

            }
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editJobs);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        }



        public async Task<ResponseResult> DeleteJobs(SharedRequestDTOs.Delete ListCode)
        {
            var Jobs = await JobsRepositoryQuery.GetAllIncludingAsync(0, 0,
                e => ListCode.Ids.Contains(e.Id) && e.Id != 1,w=>w.Employees);
            var listOfCanDelete = new List<InvJobs>();
            foreach (var item in Jobs)
            {
                if (!item.Employees.Any())
                    listOfCanDelete.Add(item);
            }
            JobsRepositoryCommand.RemoveRange(listOfCanDelete);
            bool deleted = await JobsRepositoryCommand.SaveAsync();
            int count = Jobs.Count();
            if(deleted)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteJobs,count);
            return new ResponseResult() { Data = null, Id = null, Result = deleted ?  Result.Success : Result.Failed };
        }

       

        public async Task<ResponseResult> GetJobHistory(int JobId)
        {
            return await history.GetHistory(a=>a.EntityId== JobId);
        }

        public async Task<ResponseResult> GetJobsDropDownList()
        {
            var jobs = JobsRepositoryQuery
                .TableNoTracking
                .Where(e => e.Status == (int)Status.Active)
                .Select(a => new { a.Id, a.LatinName, a.ArabicName });
            return new ResponseResult() { Data = jobs, DataCount = jobs.Count() };
        }
    }

}
