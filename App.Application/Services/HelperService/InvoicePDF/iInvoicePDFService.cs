using App.Domain.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.HelperService.InvoicePDF
{
    public interface iInvoicePDFService
    {
        public Task<Tuple<string, byte[]>> getInvoicePDF(InvoiceDTO invoiceDto);
    }
}
