using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Invoices
{
    public class SettingsOfInvoice
    {
     
        public bool ActiveVat { get; set; }
        public bool PriceIncludeVat { get; set; }
        public bool ActiveDiscount { get; set; }
        public int setDecimal { get; set; }

    }

 

}
