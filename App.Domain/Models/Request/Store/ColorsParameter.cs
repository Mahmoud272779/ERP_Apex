using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class ColorsParameter
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Color { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateColorParameter
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Color { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
    }

    

    public class ColorsSearch : GeneralPageSizeParameter
    {
#nullable enable
        public string? Name { get; set; }
        public int? Status { get; set; }
    }

    
}
