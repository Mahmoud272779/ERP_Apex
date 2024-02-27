using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain
{
    public class loginReqDTO
    {
        public string companyName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool isPOS { get; set; } = false;
        public string DeviceId { get; set; }
    }
}
