using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Store.Invoices
{
    public  class paymentMethodsResponse
    {
        public double paid { get; set; }
        public double remain { get; set; }
    }
}
