using App.Api.Controllers.BaseController;
using App.Application.Handlers.Invoices.ItemsFunds.AddItemsFunds;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.StoreServices.Invoices.ItemsFund.IItemsFundsServices;
using App.Application.Services.Process.StoreServices.Invoices.ItemsFund.ItemsFundsServices;
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

namespace App.Api.Controllers.Process.Store.Invoices
{
    public class ItemsFundController : ApiStoreControllerBase
    {
        private readonly IMediator _mediator;
        private readonly iAuthorizationService _iAuthorizationService;

        //private readonly IAddItemsFundService addItemsFundService;
        //private readonly IGetInvoiceByIdService GetAddPermissionServiceByIdService;
        //private readonly IGetAllitemsFundService GetAllItemsFundService;
        //private readonly IUpdateItemsFundService updateItemsFundService;
        //private readonly IDeleteItemsFundService deleteItemsFundService;

        public ItemsFundController(
             //IAddItemsFundService addItemsFundService
              //IGetInvoiceByIdService GetAddPermissionServiceByIdService,
             //, IGetAllitemsFundService GetAllItemsFundService
             //, IUpdateItemsFundService updateItemsFundService
             //, IDeleteItemsFundService deleteItemsFundService
              iAuthorizationService iAuthorizationService,
              IActionResultResponseHandler ResponseHandler,
              IMediator mediator
            ) : base(ResponseHandler)
        {
            //this.addItemsFundService = addItemsFundService;
            //this.GetAddPermissionServiceByIdService = GetAddPermissionServiceByIdService;
            //this.GetAllItemsFundService = GetAllItemsFundService;
            //this.updateItemsFundService = updateItemsFundService;
            //this.deleteItemsFundService = deleteItemsFundService;
            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
        }

        [HttpPost(nameof(AddItemsFund))]
        public async Task<ResponseResult> AddItemsFund([FromForm] AddItemsFundsRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, (int)SubFormsIds.items_Fund, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;

            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {
                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }


            //var result = await addItemsFundService.AddItemsFund(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }


        //[HttpGet("GetItemsFundById")]
        //public async Task<ResponseResult> GetItemsFundById(int InvoiceId, bool isCopy)
        //{
        //    var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, (int)SubFormsIds.items_Fund, Opretion.Open);
        //    if (isAuthorized != null)
        //        return isAuthorized;

        //    var result = await GetAddPermissionServiceByIdService.GetInvoiceById(InvoiceId, isCopy);
        //    return result;
        //}


        [HttpPost(nameof(GetAllItemsFund))]
        public async Task<ResponseResult> GetAllItemsFund(GetAllItemsFundsRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, (int)SubFormsIds.items_Fund, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            //var result = await GetAllItemsFundService.GetAllItemsFund(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }



        [HttpPut(nameof(UpdateItemsFund))]
        public async Task<ResponseResult> UpdateItemsFund([FromForm] UpdateItemsFundRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, (int)SubFormsIds.items_Fund, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }


            //var result = await updateItemsFundService.UpdateItemsFund(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }


        [HttpDelete("DeleteItemsFund")]
        public async Task<ResponseResult> DeleteItemsFund([FromBody] int[] InvoiceIdsList)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, (int)SubFormsIds.items_Fund, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            DeleteItemsFundRequest parameter = new DeleteItemsFundRequest()
            {
                Ids = InvoiceIdsList
            };
            //var result = await deleteItemsFundService.DeleteItemsFund(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }


    }
}
