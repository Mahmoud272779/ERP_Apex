using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.Service_helper.ApplicationSettingsHandler
{
    public interface IInvGeneralSettingsHandler
    {
        Task<string> GetDoubleFormat();
    }
}
