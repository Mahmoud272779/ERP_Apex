using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Transfer.OutgoingTransfer.Command
{
    public class OutGoingTransferResponse
    {
        public int InvoiceId { get; set; }
        public int Code { get; set; }
        public string RecCode { get; set; }
        public string RecNumber { get; set; }
        public DateTime Date { get; set; }
        public int TransStatus { get; set; }
        public int branchId { get; set; }
        public string StoreToAR { get; set; }
        public string StoreToEN { get; set; }
        public string StoreFromAR { get; set; }
        public string StoreFromEN { get; set; }


    }
    public class GetByIdResponse
    { 
    
    
    }
    
}
