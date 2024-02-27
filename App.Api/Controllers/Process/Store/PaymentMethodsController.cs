using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers.MainData.Payment_Methods.GetPaymentMethodByDate;
using App.Application.Handlers.Units;
using App.Application.Services.Process.Payment_methods;
using App.Application.Services.Process.Persons;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class PaymentMethodsController : ApiStoreControllerBase
    {
        private readonly IPaymentMethodsService PaymentMethodsService;
        public PaymentMethodsController(IPaymentMethodsService _PaymentMethodsService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            PaymentMethodsService = _PaymentMethodsService;
        }

        
        [HttpPost(nameof(AddPaymentMethod))]
        public async Task<ResponseResult> AddPaymentMethod(PaymentMethodsRequest parameter)
        {
            var result = await PaymentMethodsService.AddPaymentMethod(parameter);
            return result;
        }


        
        [HttpGet("GetListOfPaymentMethods")]
        public async Task<ResponseResult> GetListOfPaymentMethods(int PageNumber, int PageSize, int Status, string? Name)
        {
            PaymentMethodsSearch parameters = new PaymentMethodsSearch()
            {

                PageNumber = PageNumber,
                PageSize = PageSize,
                Status = Status,
                Name = Name

            };
             var result = await PaymentMethodsService.GetListOfPaymentMethods(parameters);
            return result;
        }

        
        [HttpPut(nameof(UpdatePaymentMethods))]
        public async Task<ResponseResult> UpdatePaymentMethods(UpdatePaymentMethodsRequest parameter)
        {
            var result = await PaymentMethodsService.UpdatePaymentMethods(parameter);
            return result;
        }

        
        [HttpPut(nameof(UpdateActivePaymentMethods))]
        public async Task<ResponseResult> UpdateActivePaymentMethods(SharedRequestDTOs.UpdateStatus parameter)
        {
            var result = await PaymentMethodsService.UpdateStatus(parameter);
            return result;
        }

        
        [HttpDelete("DeletePaymentMethods")]
        public async Task<ResponseResult> DeletePaymentMethods([FromBody] int[] Id)
        {
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var result = await PaymentMethodsService.DeletePaymentMethods(parameter);
            return result;
        }

        
        [HttpGet("GetPaymentMethodHistory")]
        public async Task<ResponseResult> GetPaymentMethodHistory(int Id)
        {
            var result = await PaymentMethodsService.GetPaymentMethodHistory(Id);
            return result;
        }
        [HttpGet("GetPaymentMethodsDropdown")]
        public async Task<ResponseResult> GetPaymentMethodsDropdown(bool isReceipts)
        {
            var result = await PaymentMethodsService.GetPaymentMethodsDropdown(isReceipts);
            return result;
        }
        [HttpPost("GetPaymentMethodsDropdowninvoice")]
        public async Task<ResponseResult> GetPaymentMethodsDropdowninvoice(GetPaymentMethodForInvoice request)
        {
            var result = await PaymentMethodsService.GetPaymentMethodsDropdowninvoic(request.paymentMeyhodID , request.invoiceTypeId.Value);
            return result;
        }
        [HttpGet("GetPaymentMethodByDate")]
        public async Task<ResponseResult> GetPaymentMethodByDate(DateTime date, int PageNumber, int PageSize = 10)
        {
            GetPaymentMethodsByDateRequest parameters = new GetPaymentMethodsByDateRequest()
            {
                date = date,
                PageNumber = PageNumber,
                PageSize = PageSize

            };
            var payment = await PaymentMethodsService.GetPaymentMethodsByDate(parameters);
            return payment;

        }    
        
   

    }
}
