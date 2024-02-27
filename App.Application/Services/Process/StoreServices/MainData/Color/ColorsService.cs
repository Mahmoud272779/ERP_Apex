using App.Application.Basic_Process;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Infrastructure.Interfaces.Repository;
using Attendleave.Erp.Core.APIUtilities;
using System.Threading.Tasks;
using App.Infrastructure.Mapping;
using static App.Domain.Enums.Enums;
using System;
using System.Collections.Generic;
using Attendleave.Erp.ServiceLayer.Abstraction;
using System.Linq;
using App.Domain.Models.Security.Authentication.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using App.Infrastructure.Helpers;
using App.Domain.Models.Shared;
using App.Application.Helpers;
using static App.Application.Helpers.Aliases;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;

namespace App.Application.Services.Process.Color
{
    public class ColorsService : BaseClass, IColorsService
    {
        private readonly IRepositoryQuery<InvColors> ColorsRepositoryQuery;
        private readonly IRepositoryCommand<InvColors> ColorsRepositoryCommand;


        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<InvColorsHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public ColorsService(IRepositoryQuery<InvColors> _ColorsRepositoryQuery,
                                   IRepositoryCommand<InvColors> _ColorsRepositoryCommand, 
                                   IHistory<InvColorsHistory> history,
                                   ISystemHistoryLogsService systemHistoryLogsService,
                                   IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            ColorsRepositoryQuery = _ColorsRepositoryQuery;
            ColorsRepositoryCommand = _ColorsRepositoryCommand;
            this.history = history;
            _systemHistoryLogsService = systemHistoryLogsService;
            httpContext = _httpContext;
             
        }

        public async Task<ResponseResult> AddColor(ColorsParameter parameter)
        {
            parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
            parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
            if (string.IsNullOrEmpty(parameter.LatinName))
                parameter.LatinName = parameter.ArabicName;
            if (string.IsNullOrEmpty(parameter.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.Exist, Note = Actions.InvalidStatus };
            }
           
            var ColorExist = await ColorsRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName);
            if (ColorExist != null)
                return new ResponseResult() { Data = null, Id = ColorExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };
            ColorExist = await ColorsRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName);
            if (ColorExist != null)
                return new ResponseResult() { Data = null, Id = ColorExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };
            if (ColorExist == null)
            {

                var table = Mapping.Mapper.Map<ColorsParameter, InvColors>(parameter);
                int NextCode = ColorsRepositoryQuery.GetMaxCode(e => e.Code) + 1;
                table.Code = NextCode;
                ColorsRepositoryCommand.Add(table);

                 history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);

                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addColor);
                 return new ResponseResult() { Data = null, Id = table.Id, Result = Result.Success };
            }
            else
            {
                return new ResponseResult() { Data = null, Id = ColorExist.Id, Result = Result.Exist, Note = Aliases.Actions.Exist };
            }
        }




        public async Task<ResponseResult> GetListOfColors(ColorsSearch parameters)
        {
            var resData = await ColorsRepositoryQuery.GetAllIncludingAsync(0, 0,
                a => ((a.Code.ToString().Contains(parameters.Name) || string.IsNullOrEmpty(parameters.Name)
                || a.ArabicName.Contains(parameters.Name) || a.LatinName.Contains(parameters.Name))
                && (parameters.Status == 0 || a.Status == parameters.Status))
                , e => (string.IsNullOrEmpty(parameters.Name) ?
                e.OrderByDescending(q => q.Code) :
                e.OrderBy(a => (a.Code.ToString().Contains(parameters.Name)) ? 0 : 1)),
                w=>w.Items);

            resData.ToList().ForEach(c =>
            {
                if (c.Id == 1)
                {
                    c.CanDelete = false;
                }
                else if (c.Items.Count==0)
                {
                    c.CanDelete = true;
                }
            });

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

        public async Task<ResponseResult> UpdateColors(UpdateColorParameter parameters)
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
                return new ResponseResult() { Data = null, Id = null, Result = Result.Exist, Note = Actions.InvalidStatus };
            }

         
            var ColorsExist = await ColorsRepositoryQuery.GetByAsync(a => (a.ArabicName == parameters.ArabicName) && a.Id != parameters.Id);

            if (ColorsExist != null)
                return new ResponseResult() { Data = null, Id = ColorsExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };
            var latinColorsExist = await ColorsRepositoryQuery.GetByAsync(a => (a.LatinName == parameters.LatinName) && a.Id != parameters.Id);

            if (latinColorsExist != null)
                return new ResponseResult() { Data = null, Id = ColorsExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };

            var data = await ColorsRepositoryQuery.GetByAsync(a => a.Id == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            var table = Mapping.Mapper.Map<UpdateColorParameter, InvColors>(parameters, data);
            if (table.Id == 1)
                table.Status = (int)Status.Active;

            await ColorsRepositoryCommand.UpdateAsyn(table);

            string browserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
                history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            if(data!=null)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editColor);
            return new ResponseResult() { Data = null, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };

        }
        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var Colors = ColorsRepositoryQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.Id));
            var ColorsList = Colors.ToList();

            ColorsList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            if (parameters.Id.Contains(1))
                ColorsList.Where(q => q.Id == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();
            var rssult = await ColorsRepositoryCommand.UpdateAsyn(ColorsList);
            foreach (var Color in ColorsList)
            {
                history.AddHistory(Color.Id, Color.LatinName, Color.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            }
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editColor);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        }
        public async Task<ResponseResult> DeleteColors(SharedRequestDTOs.Delete ListCode)
        {
            
            var Colors = ColorsRepositoryQuery.FindAll(e => ListCode.Ids.Contains(e.Id)
          && e.Id != 1 && !e.Items.Select(a => a.ColorId).Contains(e.Id)).ToList();

            ColorsRepositoryCommand.RemoveRange(Colors);
            await ColorsRepositoryCommand.SaveAsync();
            if(Colors.Count()==0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.CanNotBeDeleted , Note = Actions.CanNotBeDeleted };
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteColor);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success , Note = Actions.DeletedSuccessfully};
        }
       
        public async Task<ResponseResult> GetColorHistory(int ColorId)
        {
            return await history.GetHistory( a=>a.EntityId==ColorId);
        }



    }
}
