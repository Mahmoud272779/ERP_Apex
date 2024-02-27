using App.Domain.Models.Security.Authentication.Request;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using System.Threading.Tasks;

namespace App.Application.Services.Process.GeneralSettings
{
    public interface IGeneralSettingsBusiness
    {
         Task<IRepositoryActionResult> AddGeneralSettings(GeneralSettingsParameter parameter);
        Task<IRepositoryActionResult> UpdateGeneralSettings(UpdateGeneralSettingsParameter parameter);
        Task<IRepositoryActionResult> GetGeneralSettings(PageParameter paramters);


    }
}
