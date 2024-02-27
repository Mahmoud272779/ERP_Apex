using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.Store.Store.getItemBalanceInStores
{
    public class getItemBalanceInStoresRequest : IRequest<ResponseResult>
    {
        public int itemId { get; set; }
        public bool isPrint { get; set; }
    }
}
