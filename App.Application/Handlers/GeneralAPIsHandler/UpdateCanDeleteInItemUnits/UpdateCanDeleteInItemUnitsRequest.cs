using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.UpdateCanDeleteInItemUnits
{
    public class UpdateCanDeleteInItemUnitsRequest : IRequest<ResponseResult>
    {
        public int itemId { get; set; }
        public int? unitId { get; set; }
        public bool delete { get; set; }
    }
}
