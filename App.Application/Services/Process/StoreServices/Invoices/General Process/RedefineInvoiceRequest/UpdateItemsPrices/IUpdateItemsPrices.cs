using App.Domain.Models.Security.Authentication.Request.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public  interface IUpdateItemsPrices
    {
         public Task setItemsPrices(List<InvoiceDetails> invoiceDetails);
    }
}
