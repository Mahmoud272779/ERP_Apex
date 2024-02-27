using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using MediatR;

namespace App.Application.Handlers.Setup.ItemCard.Query
{
    public class GetLatestItemCode : IRequestHandler<GetLatestItemCodeRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> invStpItemCardMasterRepositoryQuery;

        public GetLatestItemCode(IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery
            , IRepositoryQuery<InvStpItemCardMaster> invStpItemCardMasterRepositoryQuery)
        {
            this.invGeneralSettingsRepositoryQuery = invGeneralSettingsRepositoryQuery;
            this.invStpItemCardMasterRepositoryQuery = invStpItemCardMasterRepositoryQuery;
        }



        public async Task<ResponseResult> Handle(GetLatestItemCodeRequest request, CancellationToken cancellationToken)
        {
            var maxCode = await GetMaxItemCode();
            return new ResponseResult() { Data = maxCode ,Result=Domain.Enums.Enums.Result.Success};
        }
        public async Task<string> GetMaxItemCode()
        {
            var codes = await invStpItemCardMasterRepositoryQuery.Get(e => e.ItemCode);
            List<long> intCodes = codes
                .Select(s => long.TryParse(s, out long n) ? n : 0)
                .ToList();
            return (intCodes.Max(e => e) + 1).ToString();

        }
    }
}
