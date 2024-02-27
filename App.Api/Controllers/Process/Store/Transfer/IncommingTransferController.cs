using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.Transfer;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using App.Domain;
using App.Domain.Entities;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

using static App.Domain.Enums.Enums;

namespace App.Api
{
    public class IncommingTransferController : ApiStoreControllerBase
    {
        private readonly IMediator mediator;
        private readonly iAuthorizationService _iAuthorizationService;

        //private readonly IAddSalesService addSalesService;
        //private readonly IGetInvoiceByIdService GetSalesServiceById;
        //private readonly IGetAllSalesServices getAllSalesServices;
        //private readonly IUpdateSalesService updateSalesService;
        //private readonly IGetPOSInvoicesService GetInvoiceService;
        //private readonly IDeleteSalesService deleteSalesInvoice;
        public IncommingTransferController(
            IMediator mediator,
            iAuthorizationService iAuthorizationService, 
            IActionResultResponseHandler ResponseHandler
            //IAddSalesService addSalesService, 
            //IGetInvoiceByIdService GetSalesServiceById,
            //IGetAllSalesServices getAllSalesServices, 
            //IUpdateSalesService updateSalesService,
            //IGetPOSInvoicesService getInvoiceService,
            //IDeleteSalesService deleteSalesInvoice,
            ) : base(ResponseHandler)
        {
            this.mediator = mediator;
            _iAuthorizationService = iAuthorizationService;
            //GetInvoiceService = getInvoiceService;
            //this.addSalesService = addSalesService;
            //this.GetSalesServiceById = GetSalesServiceById;
            //this.getAllSalesServices = getAllSalesServices;
            //this.updateSalesService = updateSalesService;
            //this.deleteSalesInvoice = deleteSalesInvoice;
        }

      

        [HttpPost (nameof(AddIncommingTransfer))]
        //[Route("")]
        public async Task<ActionResult<ResponseResult>> AddIncommingTransfer([FromForm] AddIncommingTransferRequest request)
        
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.IncomingTransfer, Domain.Enums.Opretion.Add);
            if (isAuthorized != null) return isAuthorized;
            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                request.InvoiceDetails.Add(resReport);
            }


            return await mediator.Send(request);
        }


        [HttpPost(nameof(GetAllIncommingTransfer))]
        public async Task<ActionResult<ResponseResult>> GetAllIncommingTransfer(GetAllOutgoingTransferRequest request)

        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.IncomingTransfer, Domain.Enums.Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            request.docTypeId = (int)DocumentType.IncomingTransfer;
            request.DeletedDocTypeId = (int)DocumentType.DeletedIncommingTransfer;
            return await mediator.Send(request);

        }
        [HttpPost(nameof(GetAllTransferByStore))]
        public async Task<ActionResult<ResponseResult>> GetAllTransferByStore(GetAllOutgoingByStoreID request)

        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.IncomingTransfer, Domain.Enums.Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await mediator.Send(request);

        }
        [HttpGet(nameof(GetByIdIncommingTransfer))]
        public async Task<ActionResult<ResponseResult>> GetByIdIncommingTransfer([FromQuery] getByIdTransferRequest request)

        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.IncomingTransfer, Domain.Enums.Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await mediator.Send(request);

        }
    }
}
