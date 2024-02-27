using App.Application.Basic_Process;
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
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Size
{
    public class SizesService : BaseClass, ISizesService
    {
        private readonly IRepositoryQuery<InvSizes> SizesRepositoryQuery;
        private readonly IRepositoryCommand<InvSizes> SizesRepositoryCommand; 
        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<InvSizesHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public SizesService(IRepositoryQuery<InvSizes> _SizesRepositoryQuery,
                                   IRepositoryCommand<InvSizes> _SizesRepositoryCommand, 
                                   IHistory<InvSizesHistory> history,
                                   ISystemHistoryLogsService systemHistoryLogsService,
                                   IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            SizesRepositoryQuery = _SizesRepositoryQuery;
            SizesRepositoryCommand = _SizesRepositoryCommand; 
            httpContext = _httpContext;
            this.history = history;
            _systemHistoryLogsService = systemHistoryLogsService;
        }

        public async Task<ResponseResult> AddSize(SizesParameter parameter)
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

            var SizeExist = await SizesRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName);
            if (SizeExist != null)
            {
                return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.ArabicNameExist };
            }
            SizeExist = await SizesRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName);
            if (SizeExist != null)
            {
                return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.EnglishNameExist };
            }
           
            int NextCode = SizesRepositoryQuery.GetMaxCode(e => e.Code) + 1;

            var table = Mapping.Mapper.Map<SizesParameter, InvSizes>(parameter);
            table.Code = NextCode;
            SizesRepositoryCommand.Add(table);
            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add , Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addSize);
            return new ResponseResult() { Data = null, Id = table.Id, Result = Result.Success };

        }



        public async Task<ResponseResult> GetListOfSizes(SizesSearch parameters)
        { 
            var resData = await SizesRepositoryQuery.GetAllIncludingAsync(0, 0,
                a => ((a.Code.ToString().Contains(parameters.Name) || string.IsNullOrEmpty(parameters.Name)
                || a.ArabicName.Contains(parameters.Name) || a.LatinName.Contains(parameters.Name))
                && (parameters.Status == 0 || a.Status == parameters.Status))
                , e => (string.IsNullOrEmpty(parameters.Name) ? e.OrderByDescending(q => q.Code) : e.OrderBy(a => (a.Code.ToString().Contains(parameters.Name)) ? 0 : 1))
                ,a=>a.Items);

            // var resData = await SizesRepositoryQuery.FindByAsyn(a => a.Code == parameters.Code || a.ArabicName == parameters.name || a.LatinName == parameters.name || a.Active == parameters.active);
            var count = resData.Count();

            if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            {
                resData = resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            resData.Where(a => a.Id != 1 && a.Items.Count()==0).Select(a => { a.CanDelete = true; return a; }).ToList();



            return new ResponseResult() { Data = resData, DataCount = count, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }

        public async Task<ResponseResult> UpdateSizes(UpdateSizesParameter parameters)
        {
            if (parameters.Id == 0)
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

            var SizeExist = await SizesRepositoryQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName && a.Id != parameters.Id);
            if (SizeExist != null)
            {
                return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.ArabicNameExist };
            }
            SizeExist = await SizesRepositoryQuery.GetByAsync(a => a.LatinName == parameters.LatinName && a.Id != parameters.Id);
            if (SizeExist != null)
            {
                return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.EnglishNameExist };
            }
             

            var data = await SizesRepositoryQuery.GetByAsync(a => a.Id == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound, Note = Aliases.Actions.NotFound };

            var table = Mapping.Mapper.Map<UpdateSizesParameter, InvSizes>(parameters, data);
            if (table.Id == 1)
                table.Status = (int)Status.Active;

            await SizesRepositoryCommand.UpdateAsyn(table);

            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editSize);
            return new ResponseResult() { Data = null, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };

        }

        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.InvalidStatus };
            }

            var sizes = SizesRepositoryQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.Id));
            var SizesList = sizes.ToList();

            SizesList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            if (parameters.Id.Contains(1))
                SizesList.Where(q => q.Id == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();


            var result = await SizesRepositoryCommand.UpdateAsyn(SizesList);

            foreach (var size in SizesList)
            {
                history.AddHistory(size.Id, size.LatinName, size.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            }
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editSize);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        }



        public async Task<ResponseResult> DeleteSizes(SharedRequestDTOs.Delete parameter)
        {

            var Sizes = SizesRepositoryQuery.FindAll(e => parameter.Ids.Contains(e.Id)
       && e.Id != 1 && !e.Items.Select(a => a.ColorId).Contains(e.Id)).ToList();

            
            SizesRepositoryCommand.RemoveRange(Sizes);
            await SizesRepositoryCommand.SaveAsync();
            
        
            if(Sizes.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.CanNotBeDeleted, Note = Aliases.Actions.CanNotBeDeleted };

            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteSize);

            return new ResponseResult() { Data = null, Id = null, Result = Result.Success, Note = Aliases.Actions.DeletedSuccessfully };
        }

       

        public async Task<ResponseResult> GetSizeHistory(int SizeId)
        {
            return await history.GetHistory(a=>a.EntityId== SizeId);
        }
    }
}
