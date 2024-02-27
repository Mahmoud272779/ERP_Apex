using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class userManagerOtherSettingsResponse
    {
        public int settingId { get; set; }
        public settings Settings { get; set; }
        public object stores { get; set; }
        public object banks { get; set; }
        public object safes { get; set; }
    }
    public class settings
    {
        public object posOtherSettings { get; set; }
        public object salesOtherSettings { get; set; }
        public object purchasesOtherSettings { get; set; }
        public object generalOtherSettings { get; set; }
        public object branchOtherSettings { get; set; }    
    }


}
