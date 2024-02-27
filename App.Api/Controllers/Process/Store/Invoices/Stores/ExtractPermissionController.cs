using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.ExtractPermission;
using App.Application.Handlers.ExtractPermission.DeleteExtractPermission;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.SalesServices;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.Store.Invoices;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store.Invoices.Stores
{
    public class ExtractPermissionController : ApiStoreControllerBase
    {
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;
        //private readonly IAddExtractPermissionService addExtractPermissionService;
        //private readonly IGetAllExtractPermissionService getAllExtractPermissionServices;
        //private readonly IUpdateExtractPermissionService updateExtractPermissionServices;
        //private readonly IDeleteExtractPermissionService deleteExtractPermissionServices;
     
        public ExtractPermissionController(
                                    IActionResultResponseHandler ResponseHandler, 
                                    iAuthorizationService iAuthorizationService, 
                                    IMediator mediator
            //                        IGetAllExtractPermissionService getAllExtractPermissionServices,
            //                        IAddExtractPermissionService addExtractPermissionService, 
            //                        IUpdateExtractPermissionService updateExtractPermissionServices,
            //                        IDeleteExtractPermissionService deleteExtractPermissionServices,
            ) : base(ResponseHandler)
        {
            _mediator = mediator;
            _iAuthorizationService = iAuthorizationService;
            //this.addExtractPermissionService = addExtractPermissionService;
            //this.getAllExtractPermissionServices = getAllExtractPermissionServices;
            //this.updateExtractPermissionServices = updateExtractPermissionServices;
            //this.deleteExtractPermissionServices = deleteExtractPermissionServices;
        }

        [HttpPost(nameof(AddExtractPermission))]
        public async Task<ResponseResult> AddExtractPermission([FromForm] AddExtractPermissionRequest parameter)
        {
            try
            {

                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.pay_permission, Opretion.Add);
                if (isAuthorized != null)
                    return isAuthorized;
                var req = Request;

                var InvoicesDetails_ = Request.Form["InvoiceDetails"];
                foreach (var item in InvoicesDetails_)
                {

                    var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                    parameter.InvoiceDetails.Add(resReport);
                }

                var result = await _mediator.Send(parameter);
                return result;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [HttpPost(nameof(GetAllExtractPermission))]
        public async Task<ResponseResult> GetAllExtractPermission(GetAllExtractPermissionReqeust parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.pay_permission, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await _mediator.Send(parameter);
            return result;
        }

        [HttpPut(nameof(UpdateExtractPermission))]
        public async Task<ResponseResult> UpdateExtractPermission([FromForm] UpdateExtractPermissionRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.pay_permission, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }

           

            var result = await _mediator.Send(parameter);
            return result;
        }

        [HttpDelete("DeleteExtractPermission")]
        public async Task<ResponseResult> DeleteExtractPermission([FromBody] int[] InvoiceIdsList)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.pay_permission, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            DeleteExtractPermissionRequest parameter = new DeleteExtractPermissionRequest()
            {
                Ids = InvoiceIdsList
            };
            var result = await _mediator.Send(parameter);
            return result;
        }

    }
}
