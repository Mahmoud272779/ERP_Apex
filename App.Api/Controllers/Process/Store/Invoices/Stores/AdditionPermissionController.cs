using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.AdditionPermission.AddAdditionPermission;
using App.Application.Handlers.AdditionPermission.DeleteAdditionPermission;
using App.Application.Handlers.AdditionPermission.GetAdditionPermissionById;
using App.Application.Handlers.AdditionPermission.GetAllAdditionPermission;
using App.Application.Handlers.AdditionPermission.UpdateAdditionPermission;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.StoreServices.Invoices.AdditionPermission.IAdditionPermissionServices;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.Store.Invoices;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store.Invoices.Stores
{
    public class AdditionPermissionController: ApiStoreControllerBase
    {
        //private readonly IAddAdditionPermissionService addAdditionPermissionService;
        //private readonly IGetInvoiceByIdService GetAddPermissionServiceByIdService;
        //private readonly IGetAllAdditionPermissionService GetAllAdditionPermissionService;
        //private readonly IUpdateAdditionPermissionService updateAdditionPermissionService;
        //private readonly IDeleteAdditionPermissionService deleteAdditionPermissionService;

        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;

        public AdditionPermissionController(
                iAuthorizationService iAuthorizationService,
                IActionResultResponseHandler ResponseHandler,
                IMediator mediator
                //IAddAdditionPermissionService addAdditionPermissionService,
                //IGetInvoiceByIdService GetAddPermissionServiceByIdService,
                //IGetAllAdditionPermissionService GetAllAdditionPermissionService,
                //IUpdateAdditionPermissionService updateAdditionPermissionService,
                //IDeleteAdditionPermissionService deleteAdditionPermissionService,
            ) : base(ResponseHandler)
        {
            //this.addAdditionPermissionService = addAdditionPermissionService;
            //this.GetAddPermissionServiceByIdService = GetAddPermissionServiceByIdService;
            //this.GetAllAdditionPermissionService = GetAllAdditionPermissionService;
            //this.updateAdditionPermissionService = updateAdditionPermissionService;
            //this.deleteAdditionPermissionService = deleteAdditionPermissionService;
            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
        }

        [HttpPost(nameof(AddAdditionPermission))]
        public async Task<ResponseResult> AddAdditionPermission([FromForm] AddAdditionPermissionRequest parameter)
        {


            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.AddPermission_Repository, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var req = Request;

            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }


            //var result = await addAdditionPermissionService.AddAdditionPermission(parameter);
            var result = await _mediator.Send(parameter);
            return result;


        }

        
        [HttpGet("GetAdditionPermissionById")]
        public async Task<ResponseResult> GetAdditionPermissionById(int InvoiceId, bool isCopy)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.AddPermission_Repository, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await GetAddPermissionServiceByIdService.GetInvoiceById(InvoiceId,isCopy);
            var result = await _mediator.Send(new GetAdditionPermissionByIdRequest { InvoiceId = InvoiceId, isCopy = isCopy });
            return result;
        }

        
        [HttpPost(nameof(GetAllAdditionPermission))]
        public async Task<ResponseResult> GetAllAdditionPermission(GetAllAdditionPermissionRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.AddPermission_Repository, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await GetAllAdditionPermissionService.GetAllAdditionPermission(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }

       
        
        [HttpPut(nameof(UpdateAdditionPermission))]
        public async Task<ResponseResult> UpdateAdditionPermission([FromForm] UpdateAdditionPermissionRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.AddPermission_Repository, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }


            //var result = await updateAdditionPermissionService.UpdateAdditionPermission(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }

        
        [HttpDelete("DeleteAdditionPermission")]
        public async Task<ResponseResult> DeleteAdditionPermission([FromBody] int[] InvoiceIdsList)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.AddPermission_Repository, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            DeleteAdditionPermissionRequest parameter = new DeleteAdditionPermissionRequest()
            {
                Ids = InvoiceIdsList
            };
            //var result = await deleteAdditionPermissionService.DeleteAdditionPermission(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }


    }
}
