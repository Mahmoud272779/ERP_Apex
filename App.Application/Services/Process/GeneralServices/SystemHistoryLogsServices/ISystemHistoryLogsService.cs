using App.Domain.Enums;
using App.Domain.Models.Request.Store.Reports.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.returnAPISList;

namespace App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices
{
    public interface ISystemHistoryLogsService
    {
        public Task<bool> SystemHistoryLogsService(SystemActionEnum systemActionEnum,int count=1);
        public Task<bool> SystemHistoryLogsServiceLogin(int userId, int CurrentBranch, bool isTechincalSupport);
    }
}
