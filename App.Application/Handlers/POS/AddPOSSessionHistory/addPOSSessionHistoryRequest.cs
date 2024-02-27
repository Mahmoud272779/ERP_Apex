using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.AddPOSSessionHistory
{
    public class addPOSSessionHistoryRequest : IRequest<bool>
    {
        public int POSSessionId { get; set; }
        public string actionAr { get; set; }
        public string actionEn { get; set; }
    }
}
