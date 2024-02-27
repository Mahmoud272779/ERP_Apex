using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class AccountTreeParameter
    {
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int ParentId { get; set; }
    }
    public class UpdateAccountTreeParameter
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
    }
}
