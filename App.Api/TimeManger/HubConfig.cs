using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace App.Api
{
    public class HubConfig:Hub
    {
        public const string GROUP_NAME = "progress";
        public override Task OnConnectedAsync()
        {
           
            return Groups.AddToGroupAsync(Context.ConnectionId, "progress");
        }
    }
}
