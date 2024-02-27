using App.Api.Controllers.BaseController;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.GLServices.ReceiptBusiness.ReceiptsPaid;
using App.Domain;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Migrations;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process
{

    public class CollectionReceiptsController : ApiGeneralLedgerControllerBase
    {
        private readonly ICollectionReceipts collection;
        private readonly iUserInformation userinformation;
        private readonly iAuthorizationService authorizationService;

        public CollectionReceiptsController(IActionResultResponseHandler responseHandler,
            ICollectionReceipts collection,
            iUserInformation userinformation) : base(responseHandler)
        {
            this.collection = collection;
            this.userinformation = userinformation;
        }
        [HttpPost(nameof(AddCollectionReceipts))]
        public async Task<IActionResult> AddCollectionReceipts([FromForm] CollectionReceiptsRequest parameter)
        {
            var PaymentsMethods = Request.Form["PaymentMethedIds"];
            foreach (var item in PaymentsMethods)
            {
                var resReport = JsonConvert.DeserializeObject<PaymentMethods>(item);
                parameter.PaymentMethedIds.Add(resReport);
            }
            var res = await collection.AddCollectionReceipts(parameter);

            if (res.Result == Enums.Result.Failed)
                return Unauthorized(res);
            else
                return Ok(res);

        }
    }
}
