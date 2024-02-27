using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Shared
{
    public class InputsValidationResult
    {
        public bool Valid { get; set; }
        public List<string> ListOfErrors { get; set; }
    }
}
