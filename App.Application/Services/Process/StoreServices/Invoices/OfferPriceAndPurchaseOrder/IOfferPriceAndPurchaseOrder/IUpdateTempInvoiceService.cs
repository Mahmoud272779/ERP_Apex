using App.Domain.Models.Security.Authentication.Request.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process
{
    public  interface IUpdateTempInvoiceService
    {
        Task<ResponseResult> UpdateTempInvoice(UpdateInvoiceMasterRequest parameter, int invoiceTypeId, string invoiceType);
    
    }
}
