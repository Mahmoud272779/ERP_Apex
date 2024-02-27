using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Entities.Process;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public bool IsDefault { get; set; }
    }
    
}
