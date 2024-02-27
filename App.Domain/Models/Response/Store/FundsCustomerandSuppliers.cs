using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public  class FundsCustomerandSuppliersDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int PersonId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public bool IsCustomer { get; set; }
        public int Type { get; set; }
    }
}
