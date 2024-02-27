using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetMobileAppVersion
{
    public class MobileAppVersionDTO
    {
        public string VersionNumber { get; set; }
        public string AndroidPath { get; set; }
        public string iOSPath { get; set; }


    }
}
