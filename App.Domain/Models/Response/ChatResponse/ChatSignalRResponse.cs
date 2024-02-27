using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.ChatResponse
{
    public class ChatSignalRResponse
    {
        public int IdFrom { get; set; }
        public string imgFrom { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string message { get; set; }
        public string date { get; set; }
    }
}
