using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public  interface IRedefineInvoiceRequestService
    {
        Task<Tuple<InvoiceMasterRequest, string, string>> setInvoiceRequest(InvoiceMasterRequest sentRequest, InvGeneralSettings setting, int invoiceTypeId, UserInformationModel userInfo, string parentInvoiceType , int? salePriceId,int? mainInvoiceId);
    }
}
