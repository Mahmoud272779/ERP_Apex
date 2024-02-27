using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.HomeData
{
    public class HomeDataRequest : IRequest<ResponseResult>
    {
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
    }
}
