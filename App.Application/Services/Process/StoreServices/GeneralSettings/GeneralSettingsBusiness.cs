using App.Application.Basic_Process;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using System;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.GeneralSettings
{
    public class GeneralSettingsBusiness : BusinessBase<GLGeneralSetting>, IGeneralSettingsBusiness
    {
        private readonly IRepositoryQuery<GLGeneralSetting> GeneralSettingRepositoryQuery;
        private readonly IRepositoryCommand<GLGeneralSetting> GeneralSettingRepositoryCommand;
        private readonly IPagedList<GLGeneralSetting, GLGeneralSetting> pagedListGeneralSetting;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public GeneralSettingsBusiness(
                                       IRepositoryQuery<GLGeneralSetting> _GeneralSettingRepositoryQuery , 
                                       IRepositoryCommand<GLGeneralSetting> _GeneralSettingRepositoryCommand ,
                                       IPagedList<GLGeneralSetting, GLGeneralSetting> PagedListGeneralSetting,
                                       ISystemHistoryLogsService  systemHistoryLogsService,
                                       IRepositoryActionResult repositoryActionResult ):base (repositoryActionResult)
        {
            GeneralSettingRepositoryQuery = _GeneralSettingRepositoryQuery;
            GeneralSettingRepositoryCommand = _GeneralSettingRepositoryCommand;
            pagedListGeneralSetting = PagedListGeneralSetting;
            _systemHistoryLogsService = systemHistoryLogsService;
        }

        public async Task<IRepositoryActionResult> AddGeneralSettings(GeneralSettingsParameter parameter)
        {
            try
            {
                var dataExist = GeneralSettingRepositoryQuery.GetFirstOrDefault(a => a.Id == 1);
                if (dataExist != null)
                    return repositoryActionResult.GetRepositoryActionResult(null , RepositoryActionStatus.ExistedBefore , message: " Existed before ");

                var data = Mapping.Mapper.Map<GeneralSettingsParameter, GLGeneralSetting>(parameter);
                  GeneralSettingRepositoryCommand.Add(data);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editGeneralSetting);
              return   repositoryActionResult.GetRepositoryActionResult(data.Id, RepositoryActionStatus.Created, message: "Saved successfully");
            }
            catch (Exception ex)
            {
               return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Error , message:"Not saved");
            }

        }

        public async Task<IRepositoryActionResult> UpdateGeneralSettings(UpdateGeneralSettingsParameter parameter)
        {
            try
            {
                var data =await GeneralSettingRepositoryQuery.GetByAsync(a => a.Id == parameter.Id);
               var table = Mapping.Mapper.Map<UpdateGeneralSettingsParameter, GLGeneralSetting>(parameter, data);

                await GeneralSettingRepositoryCommand.UpdateAsyn(table);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editGeneralSetting);
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Updated, message: "Updated successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Error, message: "Not Updated");
            }

        }

        public async Task<IRepositoryActionResult> GetGeneralSettings(PageParameter paramters)
        {
            try
            {
                var Data = GeneralSettingRepositoryQuery.GetAll().ToList();
                if (Data != null)
                {
                    var result = pagedListGeneralSetting.GetGenericPagination(Data, paramters.PageNumber, paramters.PageSize, Mapper);
                    return repositoryActionResult.GetRepositoryActionResult(result, RepositoryActionStatus.Ok, message: "Ok");
                }
                else
                {
                    return repositoryActionResult.GetRepositoryActionResult(null, RepositoryActionStatus.NotFound, message: "There is no data");
                }
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Error, message: " Error");
            }

        }
    }
}
