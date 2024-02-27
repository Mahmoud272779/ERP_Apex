using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Handlers.currencyConverter.CurrencyEnum;

namespace App.Application.Handlers.currencyConverter
{
    public class currencyConverterRequest : IRequest<CurrencyResponseDTO>
    {
        public double amount { get; set; }
        public Currency from { get; set; }
        public Currency to { get; set; }
    }
}
