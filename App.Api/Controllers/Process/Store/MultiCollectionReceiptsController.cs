using App.Api.Controllers.BaseController;
using App.Application.Handlers.MultiCollectionReceipts.AddMultiCollectionReceipts;
using App.Application.Handlers.MultiCollectionReceipts.DeleteMultiCollectionReceipts;
using App.Application.Handlers.MultiCollectionReceipts.EditMultiCollectionReceipts;
using App.Application.Handlers.MultiCollectionReceipts.GetAllMultiCollectionReceipts;
using App.Application.Handlers.MultiCollectionReceipts.GetByIdMultiCollectionReceipts;
using App.Application.Services.HelperService.authorizationServices;
using App.Domain;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store
{
    public class MultiCollectionReceiptsController : ApiStoreControllerBase
    {
        private readonly iAuthorizationService _iAuthorizationService;

        public MultiCollectionReceiptsController(IMediator mediator, iAuthorizationService iAuthorizationService) : base(mediator)
        {
            _iAuthorizationService = iAuthorizationService;
        }

        /// <summary>
        /// this a simple doc for add multi collection rec
        /// </summary>
        /// <param personId ="1"></param>
        /// <remarks> api doc </remarks>
        /// <returns></returns>
        [HttpPost(nameof(addMultiCollectionReceipts))]
        public async Task<ResponseResult> addMultiCollectionReceipts([FromForm] AddMultiCollectionReceiptsRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, request.isSafe ?  (int)SubFormsIds.SafeMultiCollectionReceipt : (int)SubFormsIds.BankMultiCollectionReceipt, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;

            request.invoices = JsonConvert.DeserializeObject<List<MultiCollectionReceiptsInvocesRequestDTO>>(Request.Form[nameof(request.invoices)]);
            return await CommandAsync<ResponseResult>(request);
        }


        [HttpGet(nameof(GetAllMultiCollectionReceipts_Banks))]
        public async Task<ResponseResult> GetAllMultiCollectionReceipts_Banks([FromQuery] Get_MultiCollectionReceiptsRequestDTO  request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.BankMultiCollectionReceipt, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            return await QueryAsync<ResponseResult>(new GetAllMultiCollectionReceiptsRequest
            {
                Authority = request.Authority,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                personId = request.personId,
                searchCriteria = request.searchCriteria,
                isBank = true
            });
        }


        [HttpGet(nameof(GetAllMultiCollectionReceipts_Safes))]
        public async Task<ResponseResult> GetAllMultiCollectionReceipts_Safes([FromQuery] Get_MultiCollectionReceiptsRequestDTO request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.SafeMultiCollectionReceipt, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await QueryAsync<ResponseResult>(new GetAllMultiCollectionReceiptsRequest
            {
                Authority = request.Authority,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                personId = request.personId,
                searchCriteria = request.searchCriteria,
                isBank = false
            });
        }
        [HttpGet(nameof(GetByIdMultiCollectionReceipts))]
        public async Task<ResponseResult> GetByIdMultiCollectionReceipts([FromQuery] GetByIdMultiCollectionReceiptsRequest request)
        {
            return await QueryAsync<ResponseResult>(request);
        }
        
       
        [HttpPut(nameof(EditMultiCollectionReceipts))]
        public async Task<ResponseResult> EditMultiCollectionReceipts([FromForm] EditMultiCollectionReceiptsRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, request.isSafe ? (int)SubFormsIds.SafeMultiCollectionReceipt: (int)SubFormsIds.BankMultiCollectionReceipt, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;

            request.invoices = JsonConvert.DeserializeObject<List<MultiCollectionReceiptsInvocesRequestDTO>>(Request.Form[nameof(request.invoices)]);
            return await CommandAsync<ResponseResult>(request);
        }
        [HttpDelete(nameof(DeleteMultiCollectionReceipts))]
        public async Task<ResponseResult> DeleteMultiCollectionReceipts([FromQuery] DeleteMultiCollectionReceiptsRequest request)
        {
            return await CommandAsync<ResponseResult>(request);
        }
    }
}
