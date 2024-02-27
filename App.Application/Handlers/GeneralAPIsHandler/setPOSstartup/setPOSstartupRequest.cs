using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.setPOSstartup
{ 
    public class setPOSstartupRequest : IRequest<ResponseResult>
    {
        public int invoiceTypeId { get; set; }
    }
}
