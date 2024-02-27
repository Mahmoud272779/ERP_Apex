using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.General
{
    public class updateOhterGeneralSettingsRequest
    {
        public int Other_Decimals { get; set; }
        public bool Other_UseRoundNumber { get; set; }
        public int autoLogoutInMints { get; set; }

        public int other_ExpireNotificationValue {get;set;}
        public bool other_ExpireNotificationFlag {get;set;}
        public bool other_DemandLimitNotification { get; set; }
    
    }
}
