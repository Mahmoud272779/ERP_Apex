using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.Store.Store.DetailedTransactoinsOfItem
{
    public class DetailedTransactoinsOfItemRequest : IRequest<ResponseResult>
    {
        public int itemId { get; set; }
        public int unitId { get; set; }
        public int storeId { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool isPrint { get; set; } = false;
    }
}
