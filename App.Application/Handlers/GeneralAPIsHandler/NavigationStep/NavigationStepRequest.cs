using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.NavigationStep
{
    public class NavigationStepRequest : IRequest<ResponseResult>
    {
        public int invoiceTypeId { get; set; }
        public int invoiceCode { get; set; }
        public bool NextCode { get; set; }
    }
}
