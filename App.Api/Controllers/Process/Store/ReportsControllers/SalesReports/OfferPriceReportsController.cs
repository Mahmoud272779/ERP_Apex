using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Domain.Entities;
using App.Domain.Enums;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using App.Application.Handlers.Reports.SalesReports.OfferPriceReport;
using FastReport;
using App.Application.Services.Printing.PrintFile;

namespace App.Api.Controllers.Process.Store.ReportsControllers
{
    public class OfferPriceReportsController : ApiStoreControllerBase
    {

        private readonly IMediator _mediator;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IPricesOffersPrint pricesOffersPrint;
        private readonly IprintFileService _printFileService;

        public OfferPriceReportsController(IMediator mediator,
                                        IActionResultResponseHandler ResponseHandler,
                                        iAuthorizationService iAuthorizationService,
                                        IPricesOffersPrint pricesOffersPrint,
                                        IprintFileService printFileService) : base(ResponseHandler)
        {
            _mediator = mediator;
            _iAuthorizationService = iAuthorizationService;
            this.pricesOffersPrint = pricesOffersPrint;
            _printFileService = printFileService;
        }


        [HttpGet("OfferPrices")]
        public async Task<IActionResult> OfferPrices([FromQuery] offerpriceReportRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.offerPriceReport, Opretion.Open);
            if (isAuthorized != null) return Ok(isAuthorized);
            var res = await _mediator.Send(request);
            return Ok(res);
        }
        [HttpGet("OfferPricesReport")]
        public async Task<IActionResult> OfferPricesReport([FromQuery] offerpriceReportRequest request,exportType exportType, bool isArabic,int fileId=0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.offerPriceReport, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);

            var report = await pricesOffersPrint.PricesOffersReportPrint(request, exportType, isArabic, fileId);
            return Ok(_printFileService.PrintFile(report, "PricesOffersReport", exportType));
        }


    }
}
