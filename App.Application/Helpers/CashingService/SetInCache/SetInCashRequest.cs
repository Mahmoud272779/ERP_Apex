using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.CashingService.SetInCash
{
    public class SetInCashRequest : IRequest<bool>
    {
        public object data { get; set; }
        public string key { get; set; }
        public TimeSpan expirationTime { get; set; }
    }
}
