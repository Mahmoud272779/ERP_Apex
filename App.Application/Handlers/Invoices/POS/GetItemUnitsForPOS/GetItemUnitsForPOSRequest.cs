using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetItemUnitsForPOS
{
    public class GetItemUnitsForPOSRequest : IRequest<ResponseResult>
    {
        public List<int> itemId { get; set; }
    }
}
