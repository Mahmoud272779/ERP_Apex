using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.PosPrintWithEnding
{
    public class PosPrintWithEndingHandler : IRequestHandler<PosPrintWithEndingRequest, bool>
    {
        private readonly IRepositoryQuery<InvGeneralSettings> settingService;

        public PosPrintWithEndingHandler(IRepositoryQuery<InvGeneralSettings> settingService)
        {
            this.settingService = settingService;
        }

        public async Task<bool> Handle(PosPrintWithEndingRequest request, CancellationToken cancellationToken)
        {
            return settingService.TableNoTracking.FirstOrDefault().Pos_PrintWithEnding;
        }
    }
}
