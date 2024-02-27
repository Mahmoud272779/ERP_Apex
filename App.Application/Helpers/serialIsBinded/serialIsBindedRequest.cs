using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Helper.serialIsBinded
{
    public class serialIsBindedRequest : IRequest<List<string>>
    {
        public string itemCode { get; set; }
        public List<string>? serialsRequest { get; set; }
        public string invoiceType { get; set; }

    }
}
