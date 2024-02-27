using App.Domain.Models.Setup.ItemCard.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.SalesReports.OfferPriceReport
{
    public interface IPricesOffersPrint
    {

        Task<WebReport> PricesOffersReportPrint(offerpriceReportRequest parameter, exportType exportType, bool isArabic, int fileId = 0);
    }
}
