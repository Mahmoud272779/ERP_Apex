using App.Api.Controllers.BaseController;
using App.Application.Handlers;
using App.Application.Handlers.listOfUnpaidinvoices;
using App.Application.Helpers.DemandLimitNotificationSystem;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Reports.StoreReports.Sales;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace App.Api
{
    public class InvoicesHelperController : ApiStoreControllerBase
    {
        public InvoicesHelperController(IMediator mediator) : base(mediator)
        {
        }
        [HttpGet(nameof(listOfUnpaidinvoices))]
        public async Task<ResponseResult> listOfUnpaidinvoices([FromQuery] listOfUnpaidinvoicesRequest request)
        {
            var res = await QueryAsync<ResponseResult>(request);
            return res;
        }
    }
}
