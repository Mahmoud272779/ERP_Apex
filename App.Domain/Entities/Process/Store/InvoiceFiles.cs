using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class InvoiceFiles
    {
        public int InvoiceFileId { get; set; }
        public int InvoiceId { get; set; }
        public string FileLink { get; set; }
        public string FileName { get; set; }
        public string FileExtensions { get; set; }
        //public double  FileSize { get; set; }
        public virtual InvoiceMaster InvoicesMaster { get; set; } //Relation one to many with invoice master

    }
}
