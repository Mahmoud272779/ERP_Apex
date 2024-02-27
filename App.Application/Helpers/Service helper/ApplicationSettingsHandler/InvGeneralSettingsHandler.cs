using App.Domain.Entities.Process;
using App.Infrastructure.Interfaces.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.Service_helper.ApplicationSettingsHandler
{
    public class InvGeneralSettingsHandler : IInvGeneralSettingsHandler
    {
        private readonly IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoyQuery;

        public InvGeneralSettingsHandler(IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoyQuery)
        {
            this.generalSettingsRepositoyQuery = generalSettingsRepositoyQuery;
        }
        public async Task<string> GetDoubleFormat()
        {
            var other_Decimals = (await generalSettingsRepositoyQuery.FindAsync(e => 1 == 1)).Other_Decimals;
            string format = "{0:#.";
            if (other_Decimals > 0)
            {
                for (int i = 0; i < other_Decimals; i++)
                {
                    format += "#";
                }
            }
            
            return format+"}";
        }
    }
}
