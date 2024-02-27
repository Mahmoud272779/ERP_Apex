using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class InvoiceSerialize
    {
        // Make serialize of invoices and reciepts
        public int InvoiceSerializeId { get; set; } // PK
        public double Serialize { get; set; }  
        public int InvoiceCode { get; set; }  
        public int InvoiceTypeId { get; set; } 
        public int BranchId { get; set; }
       
    }
}
