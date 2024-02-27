using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.GetSessionsOpened
{
    public class GetOpenSessionsPOSRequest : IRequest<ResponseResult>
    {
        public POSSessionStatus sessionStatus { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int employeeId { get; set; } = 0;
    }
}
