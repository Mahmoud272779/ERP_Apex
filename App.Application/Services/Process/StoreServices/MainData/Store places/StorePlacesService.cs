using App.Application.Basic_Process;
using App.Application.Helpers;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
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

namespace App.Application.Services.Process.Store_places
{
     public class StorePlacesService : BaseClass, IStorePlacesService
    {
        private readonly IRepositoryQuery<InvStorePlaces> StorePlacesRepositoryQuery;
        private readonly IRepositoryCommand<InvStorePlaces> StorePlacesRepositoryCommand; 
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardRepository;
        private readonly IHistory<InvStorePlacesHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IHttpContextAccessor httpContext;

        public StorePlacesService(IRepositoryQuery<InvStorePlaces> _StorePlacesRepositoryQuery ,
                                   IRepositoryCommand<InvStorePlaces> _StorePlacesRepositoryCommand, 
                                    IRepositoryQuery<InvStpItemCardMaster> itemCardRepository,
                                    IHistory<InvStorePlacesHistory> history,
                                    ISystemHistoryLogsService systemHistoryLogsService,
                                   IHttpContextAccessor _httpContext): base(_httpContext)
        {
            StorePlacesRepositoryQuery = _StorePlacesRepositoryQuery;
            StorePlacesRepositoryCommand = _StorePlacesRepositoryCommand; 
            this.itemCardRepository = itemCardRepository;
            httpContext = _httpContext;
            this.history = history;
            _systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<ResponseResult> AddStorePlace(StorePlacesParameter parameter)
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

            var ArabicStorePlaceExist = await StorePlacesRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName );
           if(ArabicStorePlaceExist != null )
                return new ResponseResult() { Data = null, Id = ArabicStorePlaceExist.Id, Result = Result.Exist , Note = Actions.ArabicNameExist };

            var LatinStorePlaceExist = await StorePlacesRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName);
            if(LatinStorePlaceExist != null )
                return new ResponseResult() { Data = null, Id = LatinStorePlaceExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };
 
                    int NextCode = StorePlacesRepositoryQuery.GetMaxCode(e => e.Code) + 1;

                     var table = Mapping.Mapper.Map<StorePlacesParameter, InvStorePlaces>(parameter);
                    table.Code = NextCode;   
                    StorePlacesRepositoryCommand.Add(table);
            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addStoresPlace);
            return new ResponseResult() { Data = null , Id = table.Id, Result = Result.Success  };
              
        }
        public async Task<ResponseResult> GetListOfStorePlaces(StorePlacesSearch parameters)
        {
            var resData = await StorePlacesRepositoryQuery.GetAllIncludingAsync(0,0,
                           a => ((a.Code.ToString().Contains(parameters.Name) ||
                           string.IsNullOrEmpty(parameters.Name) ||
                           a.ArabicName.Contains(parameters.Name) ||
                           a.LatinName.Contains(parameters.Name)) &&
                           (parameters.Status == 0 || a.Status == parameters.Status))
                           , e => (string.IsNullOrEmpty( parameters.Name) ? e.OrderByDescending(q => q.Code) : e.OrderBy(a => (a.Code.ToString().Contains(parameters.Name)) ? 0 : 1))
                           , a=>a.Items);


            var items = itemCardRepository.TableNoTracking.Where(a => a.DefaultStoreId != 0).Select(a => a.DefaultStoreId);

            var res = resData.ToList();
      

            var count = 0;
            if (res.Count > 0)
            {
                count = resData.Count();
                if (parameters.PageSize > 0 && parameters.PageNumber > 0)
                {
                    resData = resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
                }
                else
                {
                    return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

                }


                //if (items != null && items.Count() > 0)
                //    resData = resData.Where(e => !items.Contains(e.StorePlaceId) || e.StorePlaceId!=1).Select(a=> { a.CanDelete = true; return a; }).ToList();
                
                for (int k = 0; k < res.Count; k++)
                {
                    var storePlaceExistInItems = items.Contains(res[k].Id);
                    res[k].CanDelete = true && res[k].Id != 1;
                    if (storePlaceExistInItems)
                    {
                        res[k].CanDelete = false;
                    }

                    //}
                    //  count = StorePlacesRepositoryQuery.Count(a => ((a.Code.ToString().Contains(parameters.name) ||
                    //           string.IsNullOrEmpty(parameters.name) ||
                    //           a.ArabicName.Contains(parameters.name) ||
                    //           a.LatinName.Contains(parameters.name)) &&
                    //           (parameters.active == 0 || a.Active == parameters.active)));
                }

            }
          
            return new ResponseResult() { Data = resData, DataCount =count, Id = null, Result = res.Any() ? Result.Success : Result.Failed };
               
        }
        public async Task<ResponseResult> UpdateStorePlaces(UpdateStorePlacesParameter parameters)
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

            var ArabicStorePlaceExist = await StorePlacesRepositoryQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName  && a.Id != parameters.Id);
            if (ArabicStorePlaceExist != null)
                return new ResponseResult() { Data = null, Id = ArabicStorePlaceExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };

            var LatinStorePlaceExist = await StorePlacesRepositoryQuery.GetByAsync(a =>  a.LatinName == parameters.LatinName && a.Id != parameters.Id);
            if (LatinStorePlaceExist != null)
                return new ResponseResult() { Data = null, Id = LatinStorePlaceExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };

                var data = await StorePlacesRepositoryQuery.GetByAsync(a => a.Id == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            var table = Mapping.Mapper.Map<UpdateStorePlacesParameter, InvStorePlaces>(parameters, data);
            if (table.Id == 1)
                table.Status = (int)Status.Active;

            await StorePlacesRepositoryCommand.UpdateAsyn(table);

            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update , Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editStoresPlace);
            return new ResponseResult() { Data = null , Id = data.Id , Result = data== null?  Result.Failed : Result.Success };

        }
        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            var storePlaces = StorePlacesRepositoryQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.Id));
            var StorePlacesList = storePlaces.ToList();

            StorePlacesList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            if (parameters.Id.Contains(1))
                StorePlacesList.Where(q => q.Id == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();


            var result = await StorePlacesRepositoryCommand.UpdateAsyn(StorePlacesList);

            foreach (var storePlace in StorePlacesList)
            {
                history.AddHistory(storePlace.Id, storePlace.LatinName, storePlace.ArabicName, Aliases.HistoryActions.Update , Aliases.TemporaryRequiredData.UserName);

            }

            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editStoresPlace);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        }
        public async Task<ResponseResult> DeleteStorePlaces(SharedRequestDTOs.Delete ListCode)
        {
            var items = itemCardRepository.TableNoTracking.Where(x => ListCode.Ids.Contains(x.DefaultStoreId.Value));
            string itemsCanNotBeDeleted = items.Select(x => x.DefaultStoreId).ToString();

            var idsForDelete = ListCode.Ids.Where(c => !items.Select(x => x.DefaultStoreId).ToArray().Contains(c));
            if (!idsForDelete.Any())
                return new ResponseResult()
                {
                    Note = Actions.CanNotBeDeleted,
                    Result = Result.Failed
                };
            var storeplaces = StorePlacesRepositoryQuery.FindAll(e => idsForDelete.Contains(e.Id)&& e.Id != 1).ToList();
            var count= storeplaces.Count();

            StorePlacesRepositoryCommand.RemoveRange(storeplaces);
            await StorePlacesRepositoryCommand.SaveAsync();

            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteStoresPlace,count);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success,Note = itemsCanNotBeDeleted };   
        }
     
        public async Task<ResponseResult> GetStorePlaceHistory(int StorePlaceId)
        {
            return await history.GetHistory( a=>a.EntityId==StorePlaceId);
        }
        public async Task<ResponseResult> GetStorePlacesDropDown()
        {
            var StorePlacesList = StorePlacesRepositoryQuery.TableNoTracking.Where(e => e.Status == (int)Status.Active).Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName ,a.Status });

            return new ResponseResult() { Data = StorePlacesList, Id = null, Result = StorePlacesList.Any() ? Result.Success : Result.Failed };

        }

        public async Task<ResponseResult> GetAllStorePlacesDropDown()
        {
            var StorePlacesList = StorePlacesRepositoryQuery.TableNoTracking.Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName, a.Status });

            return new ResponseResult() { Data = StorePlacesList, Id = null, Result = StorePlacesList.Any() ? Result.Success : Result.Failed };

        }
    }
}
