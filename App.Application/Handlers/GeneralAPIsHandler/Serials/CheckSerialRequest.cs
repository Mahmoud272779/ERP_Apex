using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.Serials
{
    public class CheckSerialRequest : IRequest<serialsReponse>
    {
        public bool isDiffNumbers { get; set; }  // ارقام السيريال مختلفة ولا متتابعه
        public string? stratPattern { get; set; } // مقطع في بداية الرقم
        public string? endPattern { get; set; }  // مقطع فى نهاية السيريال
        public int? fromNumber { get; set; }  // رقم البداية
        public int? toNumber { get; set; }    // رقم النهايه
        public string? serial { get; set; }   // رقم السيريال
        public string invoiceType { get; set; } // 1-p-5
        public List<InvoiceSerialDto> newEnteredSerials { get; set; } = new List<InvoiceSerialDto>(); // الارقام المسلسلة المدخله فى الفاتورة
        public string serialsInTheSameInvoice { get; set; }
        public bool? serialRemovedInEdit { get; set; }

    }
}
