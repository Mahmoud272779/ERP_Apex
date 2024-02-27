using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.GeneralLedger
{
    public class CollectionReceiptsRequest
    {
        public List<PaymentMethods> PaymentMethedIds { get; set; }

        public int invoiceId {  get; set; }
        public string PaperNumber { get; set; }
        public string Notes { get; set; }
        public int SafeId { get; set; } 
        public DateTime RecDate { get; set; }
        public IFormFile[] AttachedFile { get; set; }

    }
    public class PaymentMethods
    {
        public int PaymentMethodId { get; set; }
        public double Value { get; set; }
        public string Cheque { get; set; }

    }
}
