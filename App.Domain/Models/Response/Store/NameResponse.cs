using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store
{
    public class NameResponse
    {
        public int code { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
    }
}
