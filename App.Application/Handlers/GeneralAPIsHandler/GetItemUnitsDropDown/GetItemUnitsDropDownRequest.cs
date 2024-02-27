using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.GetItemUnitsDropDown
{
    public class GetItemUnitsDropDownRequest : IRequest<ResponseResult>
    {
        public int itemId { get; set; }
        public string? barcode { get; set; }
    }
}
