using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.Store.EInvoice
{
    public class CSID
    {
        public int Id { get; set; }
        public int branchId { get; set; }
        public string cSR { get; set; }
        public string cSIDKey { get; set; }
        public string privateKey { get; set; }
        public string publicKey { get; set; }
        public string secretNumber { get; set; }
        public string error_ar { get; set; }
        public string error_en { get; set; }
        public string otp { get; set; }
        public bool sucess { get; set; }
        public GLBranch branch { get; set; }
    }
}
