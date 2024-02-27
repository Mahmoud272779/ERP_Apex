using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Invoices.General_Process
{
    public interface IGetAllInvoicesService
    {
        void GetAllInvoices(List<InvoiceMaster> list, List<AllInvoiceDto> list2 );
  
    }
}
