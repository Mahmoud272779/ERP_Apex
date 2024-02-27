using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.CashingService.GetFromCache
{
    public class GetFromCacheRequest : IRequest<object>
    {
        public string key { get; set; }
    }
}
