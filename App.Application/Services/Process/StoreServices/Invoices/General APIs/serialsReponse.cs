using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.General_APIs
{
   public class serialsReponse
    {
       public List<string> serials { get; set; }
        public List<InvoiceSerialDto> InvoiceSerialDtos { get; set; }
        public int serialsCount { get; set; }
        public SerialsStatus serialsStatus { get; set; }
        public string ErrorMessageAr { get; set; }
        public string ErrorMessageEn { get; set; }
    }
}
