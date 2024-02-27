using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Store.Invoices
{
    public class calcPaymentMethodsRequest
    {
        public double[] values { get; set; }
        public double net { get; set; }
    }
  
}
