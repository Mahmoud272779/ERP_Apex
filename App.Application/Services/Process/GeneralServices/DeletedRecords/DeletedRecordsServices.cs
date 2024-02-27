using MediatR;
using App.Application.Handlers.GeneralAPIsHandler;
using App.Application.Handlers.GeneralAPIsHandler.GetDeletedRecors;
using Hangfire.States;
using System.Web.Http.Results;
using App.Domain.Models.Response.Store.Invoices;
using App.Application.Handlers.GeneralAPIsHandler.SetDeletedRecords;

namespace App.Application.Services.Process.GeneralServices.DeletedRecords
{
    public class DeletedRecordsServices : BaseClass, IDeletedRecordsServices
    {
        private readonly IMediator _mediator;

        public DeletedRecordsServices(IHttpContextAccessor _httpContext, IMediator mediator) : base(_httpContext)
        {
            _mediator = mediator;
        }

        public async Task<ResponseResult> GetDeletedRecordsByDate(DateTime date)
        {
            return await _mediator.Send(new GetDeletedRecordsRequest { date = date });
        }
        public async Task<ResponseResult> SetDeletedRecord(List<int> Ids, int type)
        {
            return await _mediator.Send(new SetDeletedRecordsRequest { _Ids = Ids , _type = type});
        }
    }
}
