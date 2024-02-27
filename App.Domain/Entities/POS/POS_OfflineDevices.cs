using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.POS
{
    public class POS_OfflineDevices
    {
        public int Id { get; set; }
        public string DeviceSerial { get; set; }
        public int ServiceId { get; set; }
        public bool DeleteWaiting { get; set; }
    }
}
