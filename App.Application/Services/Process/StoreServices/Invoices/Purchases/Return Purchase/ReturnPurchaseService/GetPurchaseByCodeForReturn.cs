using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;

namespace App.Application.Services.Process.Invoices.Return_Purchase.ReturnPurchaseService
{
    public class GetPurchaseByCodeForReturn : BaseClass, IGetPurchaseByCodeForReturn
    {

        private readonly IGetInvoiceForReturn GeneralProcessGetInvoiceForReturnService;
        public GetPurchaseByCodeForReturn(IGetInvoiceForReturn GeneralProcessGetInvoiceForReturnService ,
                                       IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            this.GeneralProcessGetInvoiceForReturnService = GeneralProcessGetInvoiceForReturnService;
        }

        public async Task<ResponseResult> GetPruchase(string InvoiceCode  ,int mainInvoiceTypeId)
        {
            return await GeneralProcessGetInvoiceForReturnService.GetMainInvoiceForReturn(InvoiceCode , mainInvoiceTypeId);
        }
       
    }
}



















