using App.Infrastructure;
using App.Infrastructure.settings;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.GetPrintWithSave
{
    public class GetPrintWithSaveHandler : IRequestHandler<GetPrintWithSaveRequest, bool>
    {
        private readonly IRepositoryQuery<InvGeneralSettings> settingService;
        public GetPrintWithSaveHandler(IRepositoryQuery<InvGeneralSettings> settingService)
        {
            this.settingService = settingService;
        }

        public async Task<bool> Handle(GetPrintWithSaveRequest request, CancellationToken cancellationToken)
        {
            var print = await settingService.GetByIdAsync(1);
            return print.Purchases_PrintWithSave;
        }
    }
}
