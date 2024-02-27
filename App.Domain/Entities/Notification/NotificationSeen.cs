using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Notification
{
    public class NotificationSeen
    {
        public int id { get; set; }
        public int NotificationsMasterId { get; set; }
        public int UserId { get; set; }
        public bool isAdmin { get; set; } // this boolean for 
        public InvEmployees user { get; set; }

    }
}
