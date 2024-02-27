using MediatR;
using System.ComponentModel.DataAnnotations;

namespace App.Application.Handlers.OfflinePOSControlling.GetRegisteredDevicesService
{
    public class GetRegisteredDevicesRequest : IRequest<ResponseResult>
    {
        [Required]
        public int serverId { get; set; }
    }
}
