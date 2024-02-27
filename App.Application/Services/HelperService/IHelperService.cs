using App.Domain.Entities.Process;
using App.Domain.Models.Shared;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Application.Services.HelperService
{
    public interface IHelperService
    {
        Task<DropdownLists> FillDropDowns(List<int> pagesListID);
        string convertListToString(List<string> ListData);
        List<string> convertStringToList(string strData);
        Task<InvGeneralSettings> GetAllSettings();
        Task<InvGeneralSettings> GetAllSettings(bool dataChanged);
        Task<InvGeneralSettings> GetAllGeneralSettings();
        Task<double> GetFinanicalAccountTotalAmount(int id, string autoCoding, IQueryable<GLJournalEntryDetails> _C_D);

    }
}
