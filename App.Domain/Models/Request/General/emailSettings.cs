using App.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.General
{
    public class emailSettingsDTO
    {
        public string email { get; set; }
        public string password { get; set; }
        public string host { get; set; }
        public string displayName { get; set; }
        public int port { get; set; }
        public SecureSocketOptionsEnum secureSocketOptions { get; set; }
    }


}
