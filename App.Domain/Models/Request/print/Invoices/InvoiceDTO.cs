using App.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request
{
    public class InvoiceDTO : GeneralRequestDTO
    {
        public int invoiceId { get; set; }
        public string? invoiceCode { get; set; }
        //  public int? ScreenId { get; set; }
       public  bool isPriceOffer { get; set; } = false;
        public exportType exportType { get; set; }
       public int fileId { get; set; }

    }
    public class ReportRequestDto : GeneralRequestDTO
    {
      //public string InvoiceTypeReport { get; set; }
        public exportType exportType { get; set; }
    }
}
