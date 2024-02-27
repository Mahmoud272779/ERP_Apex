using App.Api.Controllers.BaseController;
using App.Application.Handlers.ERPTool.ReCreateInvoiceJournalEntry;
using App.Application.Handlers.ReCreateCustomersAndSuppliersFA.CustomersFA;
using App.Domain.Models.Shared;
using App.Infrastructure.settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Api.Controllers.Setup.ERPTool
{
    public class ERPToolController : ApiControllerBase
    {
        public ERPToolController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost(nameof(ReCreateSupplierFA))]
        public async Task<ResponseResult> ReCreateSupplierFA([FromQuery] int newParentId, [FromQuery]int OldAccountId, [FromHeader]string key)
        {
            if (key != defultData.userManagmentApplicationSecurityKey)
                return new ResponseResult
                {
                    Result = Domain.Enums.Enums.Result.Failed
                };
            return await CommandAsync(new ReCreateSupplierCustomerFARequest
            {
                newParentId = newParentId,
                OldAccountId = OldAccountId,
                Type = Domain.Enums.PersonTypes.supplier
            });
        }

        [HttpPost(nameof(ReCreateCustomerFA))]
        public async Task<ResponseResult> ReCreateCustomerFA([FromQuery] int newParentId, [FromQuery] int OldAccountId,[FromHeader] string key)
        {
            if (key != defultData.userManagmentApplicationSecurityKey)
                return new ResponseResult
                {
                    Result = Domain.Enums.Enums.Result.Failed
                };
            return await CommandAsync(new ReCreateSupplierCustomerFARequest
            {
                newParentId = newParentId,
                OldAccountId = OldAccountId,
                Type = Domain.Enums.PersonTypes.customer
            });
        }
        [HttpPost(nameof(ReCreateJournalEntry))]
        public async Task<ResponseResult> ReCreateJournalEntry([FromQuery] ReCreateInvoiceJournalEntryRequest request,[FromHeader] string key)
        {
            if (key != defultData.userManagmentApplicationSecurityKey)
                return new ResponseResult
                {
                    Result = Domain.Enums.Enums.Result.Failed
                };
            return await CommandAsync(request);
        }



    }
}
