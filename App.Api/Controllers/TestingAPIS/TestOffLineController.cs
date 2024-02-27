using App.Api.Controllers.BaseController;
using App.Application.Handlers;
using App.Application.Handlers.Helper.InvoicePrint;
using App.Application.Handlers.Invoices.OfferPrice.AddOfferPrice;
using App.Application.Handlers.Invoices.OfferPrice.GetOfferPriceById;
using App.Application.Handlers.Invoices.sales;
using App.Application.Handlers.Invoices.sales.DeleteSales;
using App.Application.Handlers.Invoices.sales.GetAllSales;
using App.Application.Handlers.Invoices.sales.UpdateSales;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Reports.StoreReports.Sales;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;
using static System.Net.WebRequestMethods;

namespace App.Api
{
    public class TestOffLineController : ApiStoreControllerBase
    {

        private readonly IMediator _mediator;

        public TestOffLineController(
                                IActionResultResponseHandler ResponseHandler,
                                IMediator mediator) : base(ResponseHandler)
        {

            _mediator = mediator;
        }

        [HttpPost(nameof(ConvertOfflineToCloud))]
        public async Task<Tuple<bool,List<string>>> ConvertOfflineToCloud([FromForm] offlineRequest parameter)
        {
         var res =  await    _mediator.Send(parameter);
            return res;
        }

      


    
    }
}
