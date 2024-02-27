using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing.OfferPriceLanding
{
    public interface IOfferPriceLanding
    {
        Task<WebReport> OfferPriceLandingPrint();
    }
}
