using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.GeneralLedger
{
    public class TransferReceiptsRequest
    {
        public int ReceiptTypeId { get; set; } // TransferSafe = 45 & TransferBank = 46
        public int TransferFromId { get; set; }
        public int TransferToId { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public DateTime RecDate { get; set; }
        public IFormFile[] AttachedFile { get; set; }
    }
}
