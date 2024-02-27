using App.Domain.Common;
using System.Collections.Generic;

namespace App.Domain.Entities.Process.Barcode
{
    public class InvBarcodeTemplate 
    {
        
        public int BarcodeId { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public bool IsDefault { get; set; }
        public bool CanDelete { get; set; }
        public string BrowserName { get; set; }
        public virtual ICollection<InvBarcodeItems> BarcodeItems { get; set; }

    }
}
