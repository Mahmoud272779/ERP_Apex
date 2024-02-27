using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.ConvertOffline
{
    public class CodeOfPOSOfflineReq
    {
        public int InvoiceId { get; set; }
        public string OnlineCode { get; set; }
        public string OfflineCode { get; set; }
    }
}
