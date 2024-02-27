using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.OfflinePOSControlling.DeviceRegistrationService
{
    public class DeviceRegistrationRequest : IRequest<ResponseResult>
    {
        [Required]
        public int serverId { get; set; }
        [Required]
        public string deviceSerial { get; set; }
    }
}
