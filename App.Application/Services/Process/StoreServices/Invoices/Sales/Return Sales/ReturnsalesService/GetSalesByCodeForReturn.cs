using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application
{
    public  class GetSalesByCodeForReturn:BaseClass ,IGetSalesByCodeForReturn
    {
        private readonly IGetInvoiceForReturn GeneralProcessGetInvoiceForReturnService;
        public GetSalesByCodeForReturn(IGetInvoiceForReturn GeneralProcessGetInvoiceForReturnService,
                                       IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            this.GeneralProcessGetInvoiceForReturnService = GeneralProcessGetInvoiceForReturnService;
        }

        public async Task<ResponseResult> GetReturnSalesByCode(string InvoiceCode)
        {
            return await GeneralProcessGetInvoiceForReturnService.GetMainInvoiceForReturn(InvoiceCode, (int)DocumentType.Sales);
        }
    }
}
