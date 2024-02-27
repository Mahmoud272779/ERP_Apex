using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using App.Application.Handlers.Test._1;
using App.Application.Helpers;
using Microsoft.Extensions.Caching.Memory;
using App.Domain.Models.Shared;
using App.Application.Handlers.Setup.ItemCard.Query.FillItemCardQuery;
using App.Application.Helpers.DataBasesMigraiton;
using App.Application.Services.Printing;
using static App.Domain.Enums.BarcodeEnums;
using App.Domain.Enums;
using App.Infrastructure.Persistence.Seed;
using App.Domain.Entities.Process;
using App.Application.Handlers.currencyConverter;
using Microsoft.AspNetCore.SignalR;
using App.Application.Handlers.EInvoice.CSID;
using App.Infrastructure.settings;

namespace App.Api.Controllers.TestingAPIS
{
    [Route("api/[controller]")]

    public class TestingHandlerController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        private readonly IHubContext<NotificationHub> _hub;
        private readonly IMediator _mediator;
        private readonly iUpdateMigrationForCompanies iUpdateMigrationForCompanies;
        private readonly IConfiguration _configuration;
        private readonly ClientSqlDbContext _clientSqlDbContext;
        private readonly IErpInitilizerData _erpInitilizerData;
        public TestingHandlerController(IMediator mediator, IMemoryCache memoryCache, iUpdateMigrationForCompanies iUpdateMigrationForCompanies, IConfiguration configuration, ClientSqlDbContext clientSqlDbContext, IErpInitilizerData erpInitilizerData, IHubContext<NotificationHub> hub)
        {
            _mediator = mediator;
            _memoryCache = memoryCache;
            this.iUpdateMigrationForCompanies = iUpdateMigrationForCompanies;
            _configuration = configuration;
            _clientSqlDbContext = clientSqlDbContext;
            _erpInitilizerData = erpInitilizerData;
            _hub = hub;
        }

        [AllowAnonymous]
        [HttpGet("currencyConverter")]
        public async Task<IActionResult> currencyConverter([FromQuery] currencyConverterRequest request)
        {
            var res = await _mediator.Send(request);
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpGet("GetCurrencyDropdownList")]
        public async Task<IActionResult> GetCurrencyDropdownList()
        {
            return Ok(CurrencyEnum.CurrenciesList);
        }

        [HttpGet("companydataJson")]
        public async Task<IActionResult> companydataJson([FromQuery]int Id, [FromQuery] string otp)
        {
            var res = await _mediator.Send(new CSIDRequest() { branchId = Id, OTP = otp});
            return Ok(res);
        }

    }
    public class migtations
    {
        public object migrations { get; set; }
        public object Appliedmigrations { get; set; }
        public object Pandingmigrations { get; set; }
    }


}
