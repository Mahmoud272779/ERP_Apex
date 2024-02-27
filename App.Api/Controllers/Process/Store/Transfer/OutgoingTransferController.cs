using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.Transfer;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using App.Domain.Entities;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api
{
    public class OutgoingTransfer: ApiStoreControllerBase
    {
        private readonly IMediator mediator;
        private readonly iAuthorizationService _iAuthorizationService;
        //private readonly IAddSalesService addSalesService;
        //private readonly IGetInvoiceByIdService GetSalesServiceById;
        //private readonly IGetAllSalesServices getAllSalesServices;
        //private readonly IUpdateSalesService updateSalesService;
        //private readonly IGetPOSInvoicesService GetInvoiceService;
        //private readonly IDeleteSalesService deleteSalesInvoice;
        public OutgoingTransfer(
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
            //this.addSalesService = addSalesService;
            //this.GetSalesServiceById = GetSalesServiceById;
            //this.getAllSalesServices = getAllSalesServices;
            //this.updateSalesService = updateSalesService;
            //this.deleteSalesInvoice = deleteSalesInvoice;
            //GetInvoiceService = getInvoiceService;
        }

      

        [HttpPost (nameof(AddOutgoingTransfer))]
        //[Route("")]
        public async Task<ActionResult<ResponseResult>> AddOutgoingTransfer([FromForm]AddOutgoingTransferRequest request)
        
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.OutgoingTransfer, Domain.Enums.Opretion.Add);
            if (isAuthorized != null) return isAuthorized;

            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                request.InvoiceDetails.Add(resReport);
            }


            return await mediator.Send(request);
        }



        [HttpPut(nameof(UpdateOutgoingTransfer))]
        //[Route("")]
        public async Task<ActionResult<ResponseResult>> UpdateOutgoingTransfer([FromForm] UpdateOutgoingTransferRequest request)

        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.OutgoingTransfer, Domain.Enums.Opretion.Edit);
            if (isAuthorized != null) return isAuthorized;
            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                request.InvoiceDetails.Add(resReport);
            }


            return await mediator.Send(request);
        }


        [HttpPost(nameof(GetAllOutgoingTransfer))]
        public async Task<ActionResult<ResponseResult>> GetAllOutgoingTransfer( GetAllOutgoingTransferRequest request)

        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.OutgoingTransfer, Domain.Enums.Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            request.docTypeId = (int)DocumentType.OutgoingTransfer;
            request.DeletedDocTypeId = (int)DocumentType.OutgoingTransfer;
            return await mediator.Send(request);

        }

        
    [HttpGet(nameof(GetByIdOutgoingTransfer))]
    public async Task<ActionResult<ResponseResult>> GetByIdOutgoingTransfer([FromQuery]getByIdTransferRequest request)

    {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.OutgoingTransfer, Domain.Enums.Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await mediator.Send(request);

    }
        [HttpDelete(nameof(DeleteOutgoingTransfer))]
        public async Task<ActionResult<ResponseResult>> DeleteOutgoingTransfer(int[]Ids)

        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.OutgoingTransfer, Domain.Enums.Opretion.Delete);
            if (isAuthorized != null) return isAuthorized;
            DeleteTransferRequest request = new DeleteTransferRequest { Ids = Ids };
            return await mediator.Send(request);

        }
        
    }
}
